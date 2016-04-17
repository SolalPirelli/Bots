using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bots.Quiz
{
    public interface IQuizScoreboard
    {
        /// <summary>
        /// Increases the score of the specified user, and returns the updated score.
        /// </summary>
        Task<long> IncreaseScoreAsync( string userId, string userName, long increment );

        /// <summary>
        /// Gets scores by users' names.
        /// </summary>
        Task<Dictionary<string, long>> GetScoresByNameAsync();
    }
}