namespace Bots.Quiz
{
    public sealed class QuizBotServices : BotServices
    {
        public IQuestionFactory QuestionFactory { get; }
        public IQuizScoreboard Scoreboard { get; }
        public QuizBotSettings Settings { get; }

        public QuizBotServices( INetwork network, ILogger logger, IScheduler scheduler,
                                IQuestionFactory questionFactory, IQuizScoreboard scoreboard,
                                QuizBotSettings settings )
            : base( network, logger, scheduler )
        {
            QuestionFactory = questionFactory;
            Scoreboard = scoreboard;
            Settings = settings;
        }
    }
}