using System;
using System.Threading.Tasks;
using Bots;

namespace Bots.Tests.Infrastructure
{
    public sealed class FakeUser : IUser
    {
        private TypedEventHandler<IUser, MessageEventArgs> _messageReceived;

        public string Id { get; set; }

        public string Name { get; set; }

        public Func<string, Task> MessageProcessor { get; set; }

        public FakeUser( string id )
        {
            Id = id;
            Name = id;
        }

        public void SendMessage( string message, MessageKind kind )
        {
            _messageReceived?.Invoke( this, new MessageEventArgs( message, kind ) );
        }

        #region IUser explicit implementation
        string IUser.Id => Id;

        string IUser.Name => Name;

        event TypedEventHandler<IUser, MessageEventArgs> IUser.MessageReceived
        {
            add { _messageReceived += value; }
            remove { _messageReceived -= value; }
        }

        Task IUser.SendMessageAsync( string message )
        {
            return MessageProcessor?.Invoke( message ) ?? Task.CompletedTask;
        }
        #endregion
    }
}