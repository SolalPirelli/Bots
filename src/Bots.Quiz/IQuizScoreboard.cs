using System.Threading.Tasks;

namespace Bots.Quiz
{
    public interface IQuizScoreboard
    {
        /// <summary>
        /// Increases the score of the specified user, and returns the updated score.
        /// </summary>
        Task<long> IncreaseScoreAsync( string userId, long increment );
    }
}