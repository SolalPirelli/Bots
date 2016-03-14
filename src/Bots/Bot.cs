using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bots
{
    public abstract class Bot<TServices, TResources, TInitialState>
        where TServices : IBotServices
        where TResources : IBotResources
        where TInitialState : Bot<TServices, TResources, TInitialState>.State, new()
    {
        private readonly TServices _services;
        private readonly TResources _resources;
        private TaskCompletionSource<bool> _completionSource;
        private CancellationTokenSource _cancellationSource;
        private State _currentState;


        protected Bot( TServices services, TResources resources )
        {
            _services = services;
            _resources = resources;
        }


        public async Task RunAsync()
        {
            if( _completionSource != null )
            {
                throw new InvalidOperationException( "The bot is already running." );
            }

            _completionSource = new TaskCompletionSource<bool>();
            _cancellationSource = new CancellationTokenSource();

            _services.Network.UserJoined += OnUserJoined;
            _services.Network.UserLeft += OnUserLeft;

            await ExecuteAsync( "Connecting", () => _services.Network.ConnectAsync() );
            await ExecuteAsync( "Sending welcome message", () => _services.Network.SendMessageAsync( _resources.Started() ) );

            await SwitchToAsync<TInitialState>();

            await _completionSource.Task;
        }

        public async Task StopAsync()
        {
            if( _completionSource == null )
            {
                throw new InvalidOperationException( "The bot is not running." );
            }

            await ExecuteAsync( "Sending goodbye message", () => _services.Network.SendMessageAsync( _resources.Stopped() ) );

            await DisconnectAsync();

            _completionSource.SetResult( true );
            _completionSource = null;
        }


        private async Task DisconnectAsync()
        {
            _services.Network.UserJoined -= OnUserJoined;
            _services.Network.UserLeft -= OnUserLeft;

            foreach( var user in _services.Network.KnownUsers )
            {
                user.MessageReceived -= OnUserMessageReceived;
            }

            _cancellationSource.Cancel();

            await ExecuteAsync( "Disconnecting", () => _services.Network.DisconnectAsync() );
        }

        private void OnUserJoined( INetwork network, UserEventArgs args )
        {
            _services.Logger.Log( $"User {args.User.Id} joined" );
            args.User.MessageReceived += OnUserMessageReceived;
        }

        private void OnUserLeft( INetwork network, UserEventArgs args )
        {
            _services.Logger.Log( $"User {args.User.Id} left" );
            args.User.MessageReceived -= OnUserMessageReceived;
        }

        private async void OnUserMessageReceived( IUser sender, MessageEventArgs args )
        {
            _services.Logger.Log( $"{args.Kind} message from {sender.Id}: {args.Text}" );

            BotCommand command;
            if( BotCommand.TryParse( sender, args, out command ) )
            {
                _services.Logger.Log( $"Executing command {command.Action}" );

                switch( command.Action )
                {
                    case "help":
                        await ExecuteAsync( "Displaying help messsage", () => sender.SendMessageAsync( _resources.Help() ) );
                        break;

                    default:
                        await _currentState.ProcessCommandAsync( command );
                        break;
                }
            }
            else
            {
                var message = BotMessage.Parse( sender, args );
                await _currentState.ProcessMessageAsync( message );
            }
        }


        private Task ExecuteAsync( string actionName, Func<Task> action )
        {
            return ExecuteAsync( actionName, async () => { await action(); return 0; } );
        }

        private async Task<T> ExecuteAsync<T>( string actionName, Func<Task<T>> action )
        {
            _services.Logger.Log( actionName );
            try
            {
                return await action();
            }
            catch( Exception e )
            {
                try
                {
                    await DisconnectAsync();
                }
                catch
                {
                    // Nothing.
                }

                _completionSource.SetException( e );
                throw;
            }
        }

        private Task SwitchToAsync<TState>()
            where TState : State, new()
        {
            if( _completionSource.Task.IsCompleted )
            {
                return Task.CompletedTask;
            }

            _cancellationSource.Cancel();
            _cancellationSource = new CancellationTokenSource();

            _currentState = new TState();
            return _currentState.InitializeAsync( this, _cancellationSource.Token );
        }


        public abstract class State
        {
            private Bot<TServices, TResources, TInitialState> _bot;

            protected TServices Services => _bot._services;
            protected TResources Resources => _bot._resources;
            protected CancellationToken CancellationToken { get; private set; }


            public Task InitializeAsync( Bot<TServices, TResources, TInitialState> bot, CancellationToken cancellationToken )
            {
                _bot = bot;
                CancellationToken = cancellationToken;

                return InitializeAsync();
            }


            protected virtual Task InitializeAsync()
            {
                return Task.CompletedTask;
            }

            public virtual Task ProcessCommandAsync( BotCommand command )
            {
                return Task.CompletedTask;
            }

            public virtual Task ProcessMessageAsync( BotMessage message )
            {
                return Task.CompletedTask;
            }
            

            protected Task SwitchToAsync<TState>() where TState : State, new() => _bot.SwitchToAsync<TState>();

            protected Task StopAsync() => _bot.StopAsync();

            protected Task ExecuteAsync( string actionName, Func<Task> action ) => _bot.ExecuteAsync( actionName, action );

            protected Task<T> ExecuteAsync<T>( string actionName, Func<Task<T>> action ) => _bot.ExecuteAsync( actionName, action );
        }
    }
}