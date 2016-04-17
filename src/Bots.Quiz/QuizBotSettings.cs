using System;

namespace Bots.Quiz
{
    public sealed class QuizBotSettings
    {
        /// <summary>
        /// Gets the medium delay between question paragraphs.
        /// </summary>
        public TimeSpan ParagraphDelay { get; }

        /// <summary>
        /// Gets the medium delay before a question is abandoned,
        /// </summary>
        public TimeSpan AnswerDelay { get; }

        /// <summary>
        /// Gets the medium delay between questions.
        /// </summary>
        public TimeSpan QuestionDelay { get; }

        /// <summary>
        /// Gets the number of people to display in the scoreboard.
        /// </summary>
        public int ScoreboardLength { get; }


        public QuizBotSettings( TimeSpan? paragraphDelay = null, TimeSpan? answerDelay = null, TimeSpan? questionDelay = null, int scoreboardLength = 10 )
        {
            ParagraphDelay = paragraphDelay ?? TimeSpan.FromSeconds( 20 );
            AnswerDelay = answerDelay ?? TimeSpan.FromSeconds( 40 );
            QuestionDelay = questionDelay ?? TimeSpan.FromSeconds( 20 );
            ScoreboardLength = scoreboardLength;
        }
    }
}