using System.Threading.Tasks;
using DiscordSharp.Objects;

namespace Bots.Networks.Discord
{
    public sealed class DiscordUser : IUser
    {
        private readonly DiscordMember _user;

        public string Id => _user.ID;
        public string Name => _user.Username;


        public DiscordUser( DiscordMember user )
        {
            _user = user;
        }


        public Task SendMessageAsync( string message )
        {
            _user.SendMessage( message );
            return Task.CompletedTask;
        }
    }
}