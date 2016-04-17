using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Bots.Quiz.Questions
{
    /// <summary>
    /// Creates questions from lines in the format used by WQuizz,
    /// a well-known French quiz bot for mIRC.
    /// http://wiz0u.free.fr/wquizz/
    /// </summary>
    public sealed class WQuizzQuestions
    {
        private static readonly Random Random = new Random();


        public static IReadOnlyList<QuizQuestion> Parse( IEnumerable<string> lines )
        {
            var questions = new List<QuizQuestion>();
            var questionIndex = -1;

            foreach( var line in lines )
            {
                questionIndex++;
                questions.Add( ParseQuestion( line, questionIndex ) );
            }

            return questions;
        }


        private static QuizQuestion ParseQuestion( string line, int questionIndex )
        {
            var parts = line.Split( '\\' );
            string question;
            string[] answers;

            if( parts.Length == 1 )
            {
                var separatorIndex = line.LastIndexOf( '?' );
                question = line.Substring( 0, separatorIndex + 1 );
                answers = new[] { line.Substring( separatorIndex + 1 ).Trim() };
            }
            else
            {
                question = parts[0].Trim();
                answers = parts.Skip( 1 ).Select( s => s.Trim() ).ToArray();
            }

            if( question == "#S" )
            {
                return CreateQuestion( questionIndex, "Anagrammes", Shuffle( answers[0] ), answers );
            }

            string category = null;
            if( question.StartsWith( "{" ) )
            {
                var categorySeparatorIndex = question.IndexOf( '}' );
                if( categorySeparatorIndex != -1 )
                {
                    category = question.Substring( 1, categorySeparatorIndex - 1 );
                    question = question.Substring( categorySeparatorIndex + 1 ).Trim();
                }
            }

            return CreateQuestion( questionIndex, category, question, answers );
        }

        private static string Shuffle( string text )
        {
            var chars = text.ToCharArray();
            for( int n = chars.Length - 1; n > 0; n-- )
            {
                int m = Random.Next( 0, n + 1 );
                var temp = chars[n];
                chars[n] = chars[m];
                chars[m] = temp;
            }
            return new string( chars );
        }


        private static QuizQuestion CreateQuestion( int id, string category, string text, string[] answers )
        {
            return new QuizQuestion( id.ToString(), category,
                                     new[] { text }, answers,
                                     Comparer.Instance, QuestionSpeed.Medium );
        }

        private sealed class Comparer : StringComparer
        {
            private static readonly StringComparer BaseComparer =
                new CultureInfo( "fr-FR" ).CompareInfo.GetStringComparer( CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase );

            public static readonly Comparer Instance = new Comparer();

            public override int Compare( string x, string y )
            {
                return BaseComparer.Compare( Normalize( x ), Normalize( y ) );
            }

            public override bool Equals( string x, string y )
            {
                return Compare( x, y ) == 0;
            }

            public override int GetHashCode( string obj )
            {
                return BaseComparer.GetHashCode( obj );
            }

            private static string Normalize( string s )
            {
                return s.Replace( "-", " " );
            }
        }
    }
}