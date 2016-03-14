using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bots.Quiz.Tests.Infrastructure
{
    public sealed class FakeQuizScoreboard : IQuizScoreboard
    {
        private readonly Dictionary<string, long> _scores = new Dictionary<string, long>();


        public Func<string, long, Task<long>> IncreaseScoreProcessor { get; set; }

        public void SetScore( string userId, long score )
        {
            _scores[userId] = score;
        }

        #region IScoreboard explicit implementation
        Task<long> IQuizScoreboard.IncreaseScoreAsync( string userId, long increment )
        {
            if( !_scores.ContainsKey( userId ) )
            {
                _scores[userId] = 0;
            }
            _scores[userId] += increment;

            return IncreaseScoreProcessor?.Invoke( userId, increment ) ?? Task.FromResult( _scores[userId] );
        }
        #endregion
    }
}