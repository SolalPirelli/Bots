namespace Bots
{
    public class BotMessage
    {
        public IUser Sender { get; }
        public string Text { get; }
        public MessageKind Kind { get; }


        public BotMessage( IUser sender, string text, MessageKind kind )
        {
            Sender = sender;
            Text = text;
            Kind = kind;
        }


        public static BotMessage Parse( IUser sender, MessageEventArgs args )
        {
            return new BotMessage( sender, args.Text.Trim(), args.Kind );
        }
    }
}