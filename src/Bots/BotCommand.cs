using System.Collections.Generic;

namespace Bots
{
    public sealed class BotCommand
    {
        public IUser Sender { get; }
        public string Action { get; }
        public IReadOnlyList<string> Arguments { get; }
        public MessageKind MessageKind { get; }


        public BotCommand( IUser sender, string action, IReadOnlyList<string> arguments, MessageKind messageKind )
        {
            Sender = sender;
            Action = action;
            Arguments = arguments;
            MessageKind = messageKind;
        }


        public static bool TryParse( IUser sender, MessageEventArgs args, out BotCommand command )
        {
            // Semi-efficient parsing because I'm bored, so why not?
            int index = 0;
            while( index < args.Text.Length && char.IsWhiteSpace( args.Text[index] ) )
            {
                index++;
            }

            if( args.Text[index] != '!' )
            {
                command = null;
                return false;
            }
            index++;

            int commandStart = index;
            while( index < args.Text.Length && !char.IsWhiteSpace( args.Text[index] ) )
            {
                index++;
            }

            var action = args.Text.Substring( commandStart, index - commandStart );

            var arguments = new List<string>();
            while( index < args.Text.Length )
            {
                while( index < args.Text.Length && char.IsWhiteSpace( args.Text[index] ) )
                {
                    index++;
                }
                if( index >= args.Text.Length )
                {
                    break;
                }

                int argStart = index;
                while( index < args.Text.Length && !char.IsWhiteSpace( args.Text[index] ) )
                {
                    index++;
                }
                arguments.Add( args.Text.Substring( argStart, index - argStart ) );
            }

            command = new BotCommand( sender, action, arguments, args.Kind );
            return true;
        }
    }
}