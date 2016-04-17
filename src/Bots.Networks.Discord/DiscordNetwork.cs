using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using DiscordSharp;
using DiscordSharp.Objects;

namespace Bots.Networks.Discord
{
    public sealed class DiscordNetwork : INetwork
    {
        private readonly DiscordNetworkConfig _config;
        private readonly DiscordClient _client;
        private readonly BufferBlock<BotMessage> _messages;
        private readonly Dictionary<DiscordMember, DiscordUser> _users;
        private readonly TaskCompletionSource<DiscordChannel> _channelSource;

        public string Name { get; private set; }
        public IReadOnlyList<IUser> Users { get; }
        public BotMessageQueue Messages { get; }


        public DiscordNetwork( DiscordNetworkConfig config )
        {
            _config = config;
            _client = new DiscordClient( tokenOverride: config.AuthenticationToken, isBotAccount: true );
            _messages = new BufferBlock<BotMessage>();
            _users = new Dictionary<DiscordMember, DiscordUser>();
            _channelSource = new TaskCompletionSource<DiscordChannel>();

            Name = "<Unknown>";
            Users = new List<IUser>( _users.Values );
            Messages = new BotMessageQueue( _messages );

            _client.Connected += ( _, e ) =>
            {
                _client.UpdateCurrentGame( _config.BotDescription );
            };

             _client.GuildAvailable += ( _, e ) =>
            {
                Name = e.Server.Name;
            };

            _client.MessageReceived += ( _, e ) =>
            {
                // HACK
                if( e.Channel.ID == _config.ChannelId )
                {
                    _channelSource.TrySetResult( e.Channel );
                }
                else
                {
                    // Ignore all messages on other channels
                    return;
                }

                DiscordUser user;
                if( !_users.TryGetValue( e.Author, out user ) )
                {
                    user = new DiscordUser( e.Author );
                    _users.Add( e.Author, user );
                }

                _messages.Post( new BotMessage( user, BotMessageKind.PublicMessage, e.Message.Content ) );
            };

            _client.PrivateMessageReceived += ( _, e ) =>
            {
                DiscordUser user;
                if( !_users.TryGetValue( e.Author, out user ) )
                {
                    user = new DiscordUser( e.Author );
                    _users.Add( e.Author, user );
                }

                _messages.Post( new BotMessage( user, BotMessageKind.PrivateMessage, e.Message ) );
            };

            _client.UserAddedToServer += ( _, e ) =>
            {
                var user = new DiscordUser( e.AddedMember );
                _users.Add( e.AddedMember, user );
                _messages.Post( new BotMessage( user, BotMessageKind.Join ) );
            };

            _client.UserRemovedFromServer += ( _, e ) =>
            {
                DiscordUser user;
                if( _users.TryGetValue( e.MemberRemoved, out user ) )
                {
                    _messages.Post( new BotMessage( user, BotMessageKind.Leave ) );
                    _users.Remove( e.MemberRemoved );
                }
            };

            _client.ChannelCreated += ( _, e ) =>
            {
                System.Diagnostics.Debugger.Break();
            };
        }


        public Task JoinAsync()
        {
            Task.Run( () => _client.Connect() );
            return Task.CompletedTask;
        }

        public Task LeaveAsync()
        {
            _client.Dispose();
            return Task.CompletedTask;
        }

        public async Task SendMessageAsync( string message )
        {
            var channel = await _channelSource.Task;
            channel.SendMessage( message );
        }
    }
}