using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Bots.Quiz.Tests.Infrastructure
{
    public sealed class FakeQuizScoreboard : IQuizScoreboard
    {
        private readonly Dictionary<string, Tuple<string, long>> _values = new Dictionary<string, Tuple<string, long>>();

        public Task<long> IncreaseScoreAsync( string userId, string userName, long increment )
        {
            if( _values.ContainsKey( userId ) )
            {
                _values[userId] = Tuple.Create( userName, _values[userId].Item2 + increment );
            }
            else
            {
                _values.Add( userId, Tuple.Create( userName, increment ) );
            }

            return Task.FromResult( _values[userId].Item2 );
        }

        public Task<Dictionary<string, long>> GetScoresByNameAsync()
        {
            return Task.FromResult( _values.ToDictionary( p => p.Value.Item1, p => p.Value.Item2 ) );
        }
    }
}