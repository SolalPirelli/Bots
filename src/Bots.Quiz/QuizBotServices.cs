using Bots;

namespace Bots.Quiz
{
    public sealed class QuizBotServices : IBotServices
    {
        public INetwork Network { get; }
        public ILogger Logger { get; }
        public IQuestionFactory QuestionFactory { get; }
        public IQuizScoreboard Scoreboard { get; }
        public QuizBotSettings Settings { get; }

        public QuizBotServices( INetwork network, ILogger logger,
                                IQuestionFactory questionFactory, IQuizScoreboard scoreboard, 
                                QuizBotSettings settings )
        {
            Network = network;
            Logger = logger;
            QuestionFactory = questionFactory;
            Scoreboard = scoreboard;
            Settings = settings;
        }
    }
}