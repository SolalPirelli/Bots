namespace Bots
{
    // TODO arguments!
    public sealed class BotCommand
    {
        public IUser Sender { get; }
        public string Action { get; }
        public bool IsPublic { get; }


        public BotCommand( IUser sender, string action, bool isPublic )
        {
            Sender = sender;
            Action = action;
            IsPublic = isPublic;
        }


        public static bool TryParse( BotMessage message, out BotCommand command )
        {
            if( message.Kind != BotMessageKind.PublicMessage && message.Kind != BotMessageKind.PrivateMessage )
            {
                command = null;
                return false;
            }

            int index = 0;
            while( index < message.Text.Length && char.IsWhiteSpace( message.Text[index] ) )
            {
                index++;
            }

            if( message.Text[index] != '!' )
            {
                command = null;
                return false;
            }
            index++;

            int commandStart = index;
            while( index < message.Text.Length && !char.IsWhiteSpace( message.Text[index] ) )
            {
                index++;
            }

            var action = message.Text.Substring( commandStart, index - commandStart );

            command = new BotCommand( message.Sender, action, message.Kind == BotMessageKind.PublicMessage );
            return true;
        }
    }
}