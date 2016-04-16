using System.Threading.Tasks;

namespace Bots
{
    public interface IUser
    {
        /// <summary>
        /// Unique identifier, guaranteed not to change.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Name, which may change.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Asynchronously sends a private message to the user.
        /// </summary>
        Task SendMessageAsync( string message );
    }
}