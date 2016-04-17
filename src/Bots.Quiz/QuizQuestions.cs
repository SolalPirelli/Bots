using System;
using System.Collections.Generic;
using System.Linq;

namespace Bots.Quiz
{
    public static class QuizQuestions
    {
        public static IEnumerable<QuizQuestion> InfiniteShuffle( IReadOnlyList<QuizQuestion> questions )
        {
            var random = new Random();
            while( true )
            {
                yield return questions[random.Next( 0, questions.Count )];
            }
        }

        public static IEnumerable<QuizQuestion> WithHints( int count, double fraction, IEnumerable<QuizQuestion> questions )
        {
            return questions.Select( q =>
                new QuizQuestion( q.Id, q.Category,
                                  Concat( q.Paragraphs, CreateHints( q.Answers[0], count, fraction ) ), q.Answers,
                                  q.AnswersComparer, q.Speed )
            );
        }

        private static IEnumerable<string> CreateHints( string answer, int count, double fraction )
        {
            for( int n = 1; n <= count; n++ )
            {
                yield return answer.Substring( 0, (int) Math.Ceiling( answer.Length * fraction * n ) );
            }
        }

        private static IReadOnlyCollection<T> Concat<T>( IEnumerable<T> left, IEnumerable<T> right )
        {
            var list = new List<T>( left );
            list.AddRange( right );
            return list;
        }
    }
}