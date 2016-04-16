using System.Threading.Tasks;

namespace Bots.Tests.Infrastructure
{
    public sealed class FakeUser : IUser
    {
        private readonly FakeNetwork _network;

        public string Id { get; }
        public string Name { get; set; }

        public FakeUser( FakeNetwork network, string id )
        {
            _network = network;
            Id = id;
            Name = id;
        }

        #region IUser explicit implementation
        string IUser.Id => Id;

        string IUser.Name => Name;

        Task IUser.SendMessageAsync( string message )
        {
            _network.SentMessages.Enqueue( new FakeMessage( message, this ) );
            return Task.CompletedTask;
        }
        #endregion
    }
}