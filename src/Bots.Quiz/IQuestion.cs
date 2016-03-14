using System;
using System.Collections.Generic;

namespace Bots.Quiz
{
    public interface IQuestion
    {
        /// <summary>
        /// Gets an unique ID for the question.
        /// </summary>
        /// <remarks>
        /// Used for logging purposes.
        /// </remarks>
        string Id { get; }

        /// <summary>
        /// Gets the question's category, which may be null.
        /// </summary>
        string Category { get; }

        /// <summary>
        /// Gets the list of paragraphs in the question's text.
        /// </summary>
        /// <remarks>
        /// They will be displayed in-order, but not necessarily at the same time.
        /// The end may also be skipped if an user answers the question quickly enough.
        /// </remarks>
        IReadOnlyList<string> Paragraphs { get; }

        /// <summary>
        /// Gets the type of comparison used when evaluating candidate answers.
        /// </summary>
        StringComparison AnswersComparison { get; }

        /// <summary>
        /// Gets all acceptable answers for the question.
        /// </summary>
        /// <remarks>
        /// White space at the beginning or end will always be ignored when evaluating candidate answers.
        /// </remarks>
        IReadOnlyList<string> AcceptableAnswers { get; }

        /// <summary>
        /// Gets the speed of the question.
        /// </summary>
        /// <remarks>
        /// This affects both the maximum delay to answer, and the speed at which paragraphs will be displayed.
        /// </remarks>
        QuestionSpeed Speed { get; }
    }
}