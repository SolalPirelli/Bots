using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bots
{
    public interface INetwork
    {
        string Name { get; }
        IReadOnlyList<IUser> KnownUsers { get; }

        event TypedEventHandler<INetwork, UserEventArgs> UserJoined;
        event TypedEventHandler<INetwork, UserEventArgs> UserLeft;

        Task ConnectAsync();
        Task DisconnectAsync();
        Task SendMessageAsync( string message );
    }
}