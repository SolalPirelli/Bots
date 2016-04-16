using System;
using System.Globalization;

namespace Bots.Quiz
{
    public abstract class QuizBotResources : BotResources
    {
        public abstract CultureInfo Culture { get; }

        public abstract string Pausing();
        public abstract string Resuming();
        public abstract string Congratulation( string userName, string answer, long newScore );
        public abstract string NoAnswer( string answer );
        public abstract string NextQuestionAnnouncement( TimeSpan delay );
        public abstract string NoMoreQuestions();
    }
}