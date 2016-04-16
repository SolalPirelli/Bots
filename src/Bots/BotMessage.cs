namespace Bots
{
    public class BotMessage
    {
        public IUser Sender { get; }
        public BotMessageKind Kind { get; }
        public string Text { get; }


        public BotMessage( IUser sender, BotMessageKind kind, string text = null )
        {
            Sender = sender;
            Kind = kind;
            Text = text;
        }
    }
}