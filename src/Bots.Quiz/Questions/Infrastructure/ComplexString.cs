using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bots.Quiz.Questions.Infrastructure
{
    public static class ComplexString
    {
        /// <summary>
        /// Gets possible strings from a single one, e.g. 
        /// 
        /// <code>
        /// "(Barack) Obama" -> { "Barack Obama", "Obama" }
        /// "(Franklin (D.)) Roosevelt" -> { "Franklin D. Roosevelt", "Franklin Roosevelt", "Roosevelt" }
        /// </code>
        /// 
        /// The answer with all optional elements set will always be first.
        /// </summary>
        public static IEnumerable<string> GetPossibilities( string answer )
        {
            var left = answer.IndexOf( '(' );
            var leftCount = 1;
            var right = -1;
            for( int n = left + 1; n < answer.Length; n++ )
            {
                if( answer[n] == '(' )
                {
                    leftCount++;
                }
                if( answer[n] == ')' )
                {
                    leftCount--;

                    if( leftCount == 0 )
                    {
                        right = n;
                        break;
                    }
                }
            }

            if( left == -1 )
            {
                if( right != -1 )
                {
                    throw new ArgumentException( $"Imbalanced right paren: {answer}" );
                }

                yield return answer;
                yield break;
            }

            if( right == -1 )
            {
                throw new ArgumentException( $"Imbalanced left paren: {answer}" );
            }

            var start = answer.Substring( 0, left );
            var middle = answer.Substring( left + 1, right - left - 1 );
            var end = answer.Substring( right + 1 );

            // While this code could be rewritten to use nested loops (and yield in both),
            // the order of returned elements is nicer this way.
            var endPossibilities = GetPossibilities( end ).ToArray();

            foreach( var middlePossibility in GetPossibilities( middle ) )
            {
                foreach( var endPossibility in endPossibilities )
                {
                    yield return NormalizeAnswer( start + middlePossibility + endPossibility );
                }
            }

            foreach( var endPossibility in endPossibilities )
            {
                yield return NormalizeAnswer( start + endPossibility );
            }
        }

        /// <summary>
        /// Normalizes an answer, to ensure it does not contain starting, ending or consecutive spaces.
        /// </summary>
        private static string NormalizeAnswer( string answer )
        {
            var builder = new StringBuilder();
            bool wasSpace = true;

            foreach( var character in answer )
            {
                if( char.IsWhiteSpace( character ) )
                {
                    if( !wasSpace )
                    {
                        builder.Append( ' ' );
                    }

                    wasSpace = true;
                }
                else
                {
                    builder.Append( character );
                    wasSpace = false;
                }
            }

            return builder.ToString().TrimEnd();
        }
    }
}
