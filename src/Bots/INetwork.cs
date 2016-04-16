using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bots
{
    public interface INetwork
    {
        string Name { get; }
        IReadOnlyList<IUser> Users { get; }
        BotMessageQueue Messages { get; }

        Task JoinAsync();
        Task LeaveAsync();

        Task SendMessageAsync( string message );
    }
}