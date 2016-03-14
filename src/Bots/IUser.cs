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
        /// Triggered when the user sends a message.
        /// </summary>
        event TypedEventHandler<IUser, MessageEventArgs> MessageReceived;

        /// <summary>
        /// Asynchronously sends a private message to the user.
        /// </summary>
        Task SendMessageAsync( string message );
    }
}