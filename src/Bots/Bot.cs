using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bots
{
    public abstract class Bot
    {
        private readonly BotServices _services;
        private readonly BotResources _resources;

        private TaskCompletionSource<bool> _completionSource;

        protected bool IsAlive => !_completionSource.Task.IsCompleted && !_completionSource.Task.IsFaulted;


        protected Bot( BotServices services, BotResources resources )
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

            _services.Logger.Log( "Connecting" );

            await _services.Network.JoinAsync();
            await _services.Network.SendMessageAsync( _resources.Started() );

            await InitializeAsync();

            var messageProcessingTask = Task.Run( async () =>
            {
                try
                {
                    while( true )
                    {
                        // TODO
                        var message = await _services.Network.Messages.DequeueAsync( default( CancellationToken ) );
                        await ProcessRawMessageAsync( message );
                    }
                }
                catch( OperationCanceledException )
                {
                    return;
                }
            } );

            await Task.WhenAny( _completionSource.Task, messageProcessingTask );
        }

        public async Task StopAsync()
        {
            if( _completionSource == null )
            {
                throw new InvalidOperationException( "The bot is not running." );
            }

            _services.Logger.Log( "Disconnecting" );

            await _services.Network.SendMessageAsync( _resources.Stopped() );

            await DisposeAsync();

            await _services.Network.LeaveAsync();

            _completionSource.SetResult( true );
            _completionSource = null;
        }


        protected virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        protected virtual Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        protected virtual Task ProcessCommandAsync( BotCommand command )
        {
            return Task.CompletedTask;
        }

        protected virtual Task ProcessMessageAsync( BotMessage message )
        {
            return Task.CompletedTask;
        }


        private async Task ProcessRawMessageAsync( BotMessage message )
        {
            _services.Logger.Log( $"{message.Sender.Id}: {message.Kind}{( message.Text == null ? "" : " -> " + message.Text )}" );

            BotCommand command;
            if( BotCommand.TryParse( message, out command ) )
            {
                _services.Logger.Log( $"Executing command {command.Action}" );

                switch( command.Action )
                {
                    case "help":
                        await message.Sender.SendMessageAsync( _resources.Help() );
                        break;

                    case "info":
                        await message.Sender.SendMessageAsync( _resources.Info() );
                        break;

                    default:
                        await ProcessCommandAsync( command );
                        break;
                }
            }
            else
            {
                await ProcessMessageAsync( message );
            }
        }
    }
}