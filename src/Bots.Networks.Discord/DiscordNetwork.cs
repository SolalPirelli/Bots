using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using D = Discord;

namespace Bots.Networks.Discord
{
    public sealed class DiscordNetwork : INetwork
    {
        private readonly DiscordNetworkConfig _config;
        private readonly D.DiscordClient _client;
        private readonly BufferBlock<BotMessage> _messages;
        private readonly Dictionary<D.User, DiscordUser> _users;
        private readonly TaskCompletionSource<D.Channel> _channelSource;

        public string Name { get; private set; }
        public IReadOnlyList<IUser> Users { get; }
        public BotMessageQueue Messages { get; }


        public DiscordNetwork( DiscordNetworkConfig config )
        {
            _config = config;
            _client = new D.DiscordClient();
            _messages = new BufferBlock<BotMessage>();
            _users = new Dictionary<D.User, DiscordUser>();
            _channelSource = new TaskCompletionSource<D.Channel>();

            Name = "<Unknown>";
            Users = new List<IUser>( _users.Values );
            Messages = new BotMessageQueue( _messages );

            _client.JoinedServer += ( _, e ) =>
            {
                _client.SetGame( _config.BotDescription );

                if( _config.BotAvatar != null )
                {
                    _client.CurrentUser.Edit( avatar: _config.BotAvatar );
                }
            };

            _client.ServerAvailable += ( _, e ) =>
           {
               Name = e.Server.Name;
           };

            _client.MessageReceived += ( _, e ) =>
            {
                if( e.Message.IsAuthor )
                {
                    // Ignore our own messages
                    return;
                }

                // HACK
                if( e.Channel.Id.ToString() == _config.ChannelId )
                {
                    _channelSource.TrySetResult( e.Channel );
                }
                else
                {
                    // Ignore all messages on other channels
                    return;
                }

                DiscordUser user;
                if( !_users.TryGetValue( e.User, out user ) )
                {
                    user = new DiscordUser( e.User );
                    _users.Add( e.User, user );
                }

                var kind = e.Message.Channel == null ? BotMessageKind.PrivateMessage : BotMessageKind.PublicMessage;

                _messages.Post( new BotMessage( user, kind, e.Message.Text ) );
            };

            _client.UserJoined += ( _, e ) =>
            {
                var user = new DiscordUser( e.User );
                _users.Add( e.User, user );
                _messages.Post( new BotMessage( user, BotMessageKind.Join ) );
            };

            _client.UserLeft += ( _, e ) =>
            {
                DiscordUser user;
                if( _users.TryGetValue( e.User, out user ) )
                {
                    _messages.Post( new BotMessage( user, BotMessageKind.Leave ) );
                    _users.Remove( e.User );
                }
            };

            _client.ChannelCreated += ( _, e ) =>
            {
                System.Diagnostics.Debugger.Break();
            };
        }


        public Task JoinAsync()
        {
            return _client.Connect( _config.AuthenticationToken, D.TokenType.Bot );
        }

        public Task LeaveAsync()
        {
            _client.Dispose();
            return Task.CompletedTask;
        }

        public async Task SendMessageAsync( string message )
        {
            if( string.IsNullOrWhiteSpace( message ) )
            {
                return;
            }

            var channel = await _channelSource.Task;

            try
            {
                await channel.SendMessage( message );
            }
            catch
            {
                await SendMessageAsync( message );
            }
        }
    }
}