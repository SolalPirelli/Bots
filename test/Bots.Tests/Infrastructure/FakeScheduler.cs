using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bots.Tests.Infrastructure
{
    public sealed class FakeScheduler : IScheduler
    {
        private readonly Dictionary<string, Queue<TaskCompletionSource<bool>>> _delaysById
            = new Dictionary<string, Queue<TaskCompletionSource<bool>>>();


        public Task Delay( string id, TimeSpan time )
        {
            if( !_delaysById.ContainsKey( id ) )
            {
                _delaysById.Add( id, new Queue<TaskCompletionSource<bool>>() );
            }

            var source = new TaskCompletionSource<bool>();
            _delaysById[id].Enqueue( source );

            return source.Task;
        }


        public void Advance( string id )
        {
            _delaysById[id].Dequeue().SetResult( true );
        }
    }
}