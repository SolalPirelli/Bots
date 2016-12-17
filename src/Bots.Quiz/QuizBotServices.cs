using System;

namespace Bots.Quiz
{
    public sealed class QuizBotServices : BotServices
    {
        public Func<QuizQuestion> QuestionSource { get; }
        public IQuizScoreboard Scoreboard { get; }
        public QuizBotSettings Settings { get; }

        public QuizBotServices( INetwork network,
                                Func<QuizQuestion> questionSource, IQuizScoreboard scoreboard, QuizBotSettings settings,
                                ILogger logger = null, IScheduler scheduler = null )
            : base( network, logger, scheduler )
        {
            QuestionSource = questionSource;
            Scoreboard = scoreboard;
            Settings = settings;
        }
    }
}