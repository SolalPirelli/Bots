using System;
using System.Globalization;

namespace Bots.Quiz
{
    public interface IQuizBotResources : IBotResources
    {
        CultureInfo Culture { get; }

        string Pausing();
        string Resuming();
        string Congratulation( string userName, string answer, long newScore );
        string NoAnswer( string answer );
        string NextQuestionAnnouncement( TimeSpan delay );
    }
}