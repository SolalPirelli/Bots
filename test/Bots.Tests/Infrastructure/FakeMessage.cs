namespace Bots.Tests.Infrastructure
{
    public sealed class FakeMessage
    {
        public string Text { get; }
        public FakeUser Target { get; }

        public FakeMessage( string text, FakeUser target = null )
        {
            Text = text;
            Target = target;
        }
    }
}