using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Bots
{
    public sealed class BotMessageQueue
    {
        private BufferBlock<BotMessage> _block;

        public BotMessageQueue( BufferBlock<BotMessage> block )
        {
            _block = block;
        }

        public Task<BotMessage> DequeueAsync( CancellationToken token )
        {
            return _block.ReceiveAsync( token );
        }
    }
}