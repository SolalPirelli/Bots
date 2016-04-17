using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bots
{
    public abstract class StateBasedBot : Bot
    {
        protected delegate Task AsyncAction( CancellationToken token );
        protected delegate Task MessageHandler( BotMessage message, CancellationToken token );
        protected delegate Task CommandHandler( BotCommand command, CancellationToken token );

        private readonly State _globalState;
        private CancellationTokenSource _cancellationSource;
        private State _currentState;
        private int _version;


        protected StateBasedBot( BotServices services, BotResources resources )
            : base( services, resources )
        {
            _globalState = new State( "Global" );
        }


        protected override sealed Task InitializeAsync()
        {
            if( _currentState == null )
            {
                throw new InvalidOperationException( "SetInitialState must be called." );
            }
            _cancellationSource = new CancellationTokenSource();
            return SwitchToAsync( _currentState );
        }

        protected override Task DisposeAsync()
        {
            _cancellationSource.Cancel();
            return Task.CompletedTask;
        }

        protected override sealed async Task ProcessCommandAsync( BotCommand command )
        {
            var handled = await _globalState.HandleCommandAsync( command, default( CancellationToken ) );

            if( !handled )
            {
                await _currentState.HandleCommandAsync( command, _cancellationSource.Token );
            }
        }

        protected override sealed Task ProcessMessageAsync( BotMessage message )
        {
            return _currentState.HandleMessageAsync( message, _cancellationSource.Token );
        }


        protected void SetInitialState( State state )
        {
            if( state == null )
            {
                throw new ArgumentNullException( nameof( state ) );
            }
            if( _currentState != null )
            {
                throw new InvalidOperationException( "The initial state has already been set." );
            }

            _currentState = state;
        }

        protected async Task SwitchToAsync( State state )
        {
            if( !IsAlive )
            {
                return;
            }

            _cancellationSource.Cancel();
            _cancellationSource = new CancellationTokenSource();

            _version++;
            int lastVersion = _version;
            System.Diagnostics.Debug.WriteLine( "Switching to " + state );

            await state.InitializeAsync( _cancellationSource.Token );

            // It is possible for a state to ask for a switch or stop during the initialization
            if( _version == lastVersion && IsAlive )
            {
                _currentState = state;
                state.RunBackgroundAction( _cancellationSource.Token );
            }
        }

        protected void AddGlobalCommandHandler( string commandName, CommandHandler commandHandler )
        {
            _globalState.AddCommandHandler( commandName, commandHandler );
        }

        protected sealed class State
        {
            private readonly string _name;
            private AsyncAction _initializer;
            private AsyncAction _backgroundAction;
            private MessageHandler _messageHandler;
            private IDictionary<string, CommandHandler> _commandHandlers;

            public State( string name )
            {
                _name = name;
            }


            public State SetInitializer( AsyncAction initializer )
            {
                if( initializer == null )
                {
                    throw new ArgumentNullException( nameof( initializer ) );
                }
                if( _initializer != null )
                {
                    throw new InvalidOperationException( "The initializer has already been set." );
                }

                _initializer = initializer;

                return this;
            }

            public State SetBackgroundAction( AsyncAction backgroundAction )
            {
                if( backgroundAction == null )
                {
                    throw new ArgumentNullException( nameof( backgroundAction ) );
                }
                if( _backgroundAction != null )
                {
                    throw new InvalidOperationException( "The background action has already been set." );
                }

                _backgroundAction = backgroundAction;

                return this;
            }

            public State SetMessageHandler( MessageHandler messageHandler )
            {
                if( messageHandler == null )
                {
                    throw new ArgumentNullException( nameof( messageHandler ) );
                }
                if( _messageHandler != null )
                {
                    throw new InvalidOperationException( "The message handler has already been set." );
                }

                _messageHandler = messageHandler;

                return this;
            }

            public State AddCommandHandler( string commandName, CommandHandler commandHandler )
            {
                if( commandName == null )
                {
                    throw new ArgumentNullException( nameof( commandName ) );
                }
                if( commandHandler == null )
                {
                    throw new ArgumentNullException( nameof( commandHandler ) );
                }

                if( _commandHandlers == null )
                {
                    _commandHandlers = new Dictionary<string, CommandHandler>();
                }

                if( _commandHandlers.ContainsKey( commandName ) )
                {
                    throw new InvalidOperationException( $"The '{commandName}' command handler has already been set." );
                }

                _commandHandlers.Add( commandName, commandHandler );

                return this;
            }


            internal Task InitializeAsync( CancellationToken token )
            {
                return _initializer?.Invoke( token ) ?? Task.CompletedTask;
            }

            internal void RunBackgroundAction( CancellationToken token )
            {
                if( _backgroundAction == null )
                {
                    return;
                }

                _backgroundAction( token );
            }

            internal Task HandleMessageAsync( BotMessage message, CancellationToken token )
            {
                return _messageHandler?.Invoke( message, token ) ?? Task.CompletedTask;
            }

            internal async Task<bool> HandleCommandAsync( BotCommand command, CancellationToken token )
            {
                CommandHandler handler;
                if( _commandHandlers != null && _commandHandlers.TryGetValue( command.Action, out handler ) )
                {
                    await handler( command, token );
                    return true;
                }

                return false;
            }


            public override string ToString()
            {
                return _name;
            }
        }
    }
}