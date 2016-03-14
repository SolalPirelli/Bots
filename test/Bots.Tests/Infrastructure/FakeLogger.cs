namespace Bots.Tests.Infrastructure
{
    // DESIGN: Should the logs be tested?
    public sealed class FakeLogger : ILogger
    {
        public void Log( string text )
        {
            // Nothing
        }
    }
}