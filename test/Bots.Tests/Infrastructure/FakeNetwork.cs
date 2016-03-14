using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bots.Tests.Infrastructure
{
    public sealed class FakeNetwork : INetwork
    {
        private event TypedEventHandler<INetwork, UserEventArgs> _userJoined;
        private event TypedEventHandler<INetwork, UserEventArgs> _userLeft;
        private readonly Dictionary<string, FakeUser> _users = new Dictionary<string, FakeUser>();

        public Func<Task> ConnectProcessor { get; set; }
        public Func<Task> DisconnectProcessor { get; set; }
        public Func<string, Task> MessageProcessor { get; set; }

        public FakeUser GetUser( string id )
        {
            FakeUser user;
            if( _users.TryGetValue( id, out user ) )
            {
                return user;
            }

            user = new FakeUser( id );
            _users.Add( user.Id, user );
            _userJoined?.Invoke( this, new UserEventArgs( user ) );
            return user;
        }

        public void RemoveUser( FakeUser user )
        {
            _users.Remove( user.Id );
            _userLeft?.Invoke( this, new UserEventArgs( user ) );
        }

        #region INetwork explicit implementation
        string INetwork.Name => "FakeNetwork";

        IReadOnlyList<IUser> INetwork.KnownUsers => new List<IUser>( _users.Values );

        event TypedEventHandler<INetwork, UserEventArgs> INetwork.UserJoined
        {
            add { _userJoined += value; }
            remove { _userJoined -= value; }
        }
        event TypedEventHandler<INetwork, UserEventArgs> INetwork.UserLeft
        {
            add { _userLeft += value; }
            remove { _userLeft -= value; }
        }

        Task INetwork.ConnectAsync()
        {
            return ConnectProcessor?.Invoke() ?? Task.CompletedTask;
        }

        Task INetwork.DisconnectAsync()
        {
            return DisconnectProcessor?.Invoke() ?? Task.CompletedTask;
        }

        Task INetwork.SendMessageAsync( string message )
        {
            return MessageProcessor?.Invoke( message ) ?? Task.CompletedTask;
        }
        #endregion
    }
}