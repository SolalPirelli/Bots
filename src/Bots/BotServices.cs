using System;
using System.Threading.Tasks;

namespace Bots
{
    public class BotServices
    {
        public INetwork Network { get; }
        public ILogger Logger { get; }
        public IScheduler Scheduler { get; }

        public BotServices( INetwork network, ILogger logger = null, IScheduler scheduler = null )
        {
            Network = network;
            Logger = logger ?? new NullLogger();
            Scheduler = scheduler ?? new DefaultScheduler();
        }


        private sealed class NullLogger : ILogger
        {
            public void Log( string text )
            {
                // Nothing
            }
        }

        private sealed class DefaultScheduler : IScheduler
        {
            public Task Delay( string id, TimeSpan time )
            {
                return Task.Delay( time );
            }
        }
    }
}