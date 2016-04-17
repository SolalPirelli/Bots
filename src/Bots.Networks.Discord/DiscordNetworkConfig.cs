namespace Bots.Networks.Discord
{
    public sealed class DiscordNetworkConfig
    {
        public string AuthenticationToken { get; }
        public string BotDescription { get; }
        public string ChannelId { get; }

        public DiscordNetworkConfig( string authenticationToken, string botDescription, string channelId )
        {
            AuthenticationToken = authenticationToken;
            BotDescription = botDescription;
            ChannelId = channelId;
        }
    }
}