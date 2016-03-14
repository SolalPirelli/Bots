using System;

namespace Bots
{
    public sealed class MessageEventArgs : EventArgs
    {
        public string Text { get; }
        public MessageKind Kind { get; }

        public MessageEventArgs( string text, MessageKind kind )
        {
            Text = text;
            Kind = kind;
        }
    }
}