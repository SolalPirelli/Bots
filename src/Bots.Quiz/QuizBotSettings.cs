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


        public QuizBotSettings( TimeSpan paragraphDelay, TimeSpan answerDelay, TimeSpan questionDelay )
        {
            ParagraphDelay = paragraphDelay;
            AnswerDelay = answerDelay;
            QuestionDelay = questionDelay;
        }
    }
}