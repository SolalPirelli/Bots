using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Bots.Quiz
{
    public sealed class QuizQuestion
    {
        /// <summary>
        /// Gets an unique ID for the question.
        /// Used for logging purposes.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the question's category, which may be null.
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Gets the list of paragraphs in the question's text.
        /// </summary>
        /// <remarks>
        /// They will be displayed in-order, but not necessarily at the same time.
        /// The end may also be skipped if an user answers the question quickly enough.
        /// </remarks>
        public IReadOnlyList<string> Paragraphs { get; }

        /// <summary>
        /// Gets all acceptable answers for the question.
        /// Whitespace at the beginning or end will always be ignored when evaluating candidate answers.
        /// </remarks>
        public IReadOnlyList<string> Answers { get; }

        /// <summary>
        /// Gets the comparer used when evaluating candidate answers.
        /// If <c>null</c>, an ordinal comparison will be performed, ignoring case.
        /// </summary>
        public StringComparer AnswersComparer { get; }
        

        public QuizQuestion( string id, string category,
                             IReadOnlyCollection<string> paragraphs, IReadOnlyCollection<string> answers,
                             StringComparer answersComparer = null )
        {
            if( id == null )
            {
                throw new ArgumentNullException( nameof( id ) );
            }
            if( paragraphs == null )
            {
                throw new ArgumentNullException( nameof( paragraphs ) );
            }
            if( paragraphs.Count == 0 )
            {
                throw new ArgumentException( "A question must have at least one paragraph.", nameof( paragraphs ) );
            }
            if( answers == null )
            {
                throw new ArgumentNullException( nameof( answers ) );
            }
            if( answers.Count == 0 )
            {
                throw new ArgumentException( "A question must have at least one answer.", nameof( answers ) );
            }

            Id = id;
            Category = category;
            Paragraphs = paragraphs.ToArray();
            Answers = answers.ToArray();
            AnswersComparer = answersComparer ?? CultureInfo.InvariantCulture.CompareInfo.GetStringComparer( CompareOptions.OrdinalIgnoreCase );
        }
    }
}