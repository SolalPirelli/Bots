using System.Collections.Generic;

namespace Bots.Quiz
{
    public sealed class QuizBotServices : BotServices
    {
        public IEnumerable<QuizQuestion> Questions { get; }
        public IQuizScoreboard Scoreboard { get; }
        public QuizBotSettings Settings { get; }

        public QuizBotServices( INetwork network,
                                IEnumerable<QuizQuestion> questions, IQuizScoreboard scoreboard, QuizBotSettings settings,
                                ILogger logger = null, IScheduler scheduler = null )
            : base( network, logger, scheduler )
        {
            Questions = questions;
            Scoreboard = scoreboard;
            Settings = settings;
        }
    }
}