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
        private CancellationTokenSource _cancellationSource;

        protected bool IsAlive => _cancellationSource != null && !_cancellationSource.IsCancellationRequested;


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
            _cancellationSource = new CancellationTokenSource();

            _services.Logger.Log( "Connecting" );

            await _services.Network.JoinAsync();
            await _services.Network.SendMessageAsync( _resources.Started() );

            await InitializeAsync();

            await Task.WhenAny( _completionSource.Task, ReceiveMessagesAsync() );
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
            _cancellationSource.Cancel();
            _cancellationSource = null;
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


        private async Task ReceiveMessagesAsync()
        {
            try
            {
                while( true )
                {
                    var message = await _services.Network.Messages.DequeueAsync( _cancellationSource.Token );
                    await ProcessRawMessageAsync( message );
                }
            }
            catch( OperationCanceledException )
            {
                // Nothing
            }
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