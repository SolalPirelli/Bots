using System.Threading.Tasks;
using D = Discord;

namespace Bots.Networks.Discord
{
    public sealed class DiscordUser : IUser
    {
        private readonly D.User _user;

        public string Id => _user.Id.ToString();
        public string Name => _user.Name;


        public DiscordUser( D.User user )
        {
            _user = user;
        }


        public Task SendMessageAsync( string message )
        {
            if( string.IsNullOrWhiteSpace( message ) )
            {
                return Task.CompletedTask;
            }

            return _user.SendMessage( message );
        }
    }
}