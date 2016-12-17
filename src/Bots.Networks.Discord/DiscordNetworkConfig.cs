using System.IO;

namespace Bots.Networks.Discord
{
    public sealed class DiscordNetworkConfig
    {
        public string AuthenticationToken { get; }
        public string BotDescription { get; }
        public string ChannelId { get; }
        public Stream BotAvatar { get; }

        public DiscordNetworkConfig( string authenticationToken, string botDescription, string channelId, Stream botAvatar = null )
        {
            AuthenticationToken = authenticationToken;
            BotDescription = botDescription;
            ChannelId = channelId;
            BotAvatar = botAvatar;
        }
    }
}