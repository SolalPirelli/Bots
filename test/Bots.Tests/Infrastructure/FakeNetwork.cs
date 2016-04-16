using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Bots.Tests.Infrastructure
{
    public sealed class FakeNetwork : INetwork, IDisposable
    {
        private readonly Dictionary<string, FakeUser> _users = new Dictionary<string, FakeUser>();
        private readonly BufferBlock<BotMessage> _messages = new BufferBlock<BotMessage>();

        public Queue<FakeMessage> SentMessages { get; } = new Queue<FakeMessage>();

        public FakeUser GetUser( string id )
        {
            FakeUser user;
            if( _users.TryGetValue( id, out user ) )
            {
                return user;
            }

            user = new FakeUser( this, id );
            _users.Add( user.Id, user );
            _messages.Post( new BotMessage( user, BotMessageKind.Join ) );
            return user;
        }

        public void RemoveUser( FakeUser user )
        {
            _users.Remove( user.Id );
            _messages.Post( new BotMessage( user, BotMessageKind.Leave ) );
        }

        public void SendMessage( IUser sender, string message, bool isPublic )
        {
            _messages.Post( new BotMessage( sender, isPublic ? BotMessageKind.PublicMessage : BotMessageKind.PrivateMessage, message ) );
        }

        public void Dispose()
        {
            _messages.Complete();
        }

        #region INetwork explicit implementation
        string INetwork.Name => "FakeNetwork";

        IReadOnlyList<IUser> INetwork.Users => new List<IUser>( _users.Values );

        BotMessageQueue INetwork.Messages => new BotMessageQueue( _messages );

        Task INetwork.JoinAsync()
        {
            return Task.CompletedTask;
        }

        Task INetwork.LeaveAsync()
        {
            return Task.CompletedTask;
        }

        Task INetwork.SendMessageAsync( string message )
        {
            SentMessages.Enqueue( new FakeMessage( message ) );
            return Task.CompletedTask;
        }
        #endregion
    }
}