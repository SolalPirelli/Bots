using System;
using System.Collections.Generic;

namespace Bots.Quiz.Infrastructure
{
    public static class QuestionHints
    {
        public static IEnumerable<string> CreateProportional( string answer, int count, double fraction )
        {
            for( int n = 1; n <= count; n++ )
            {
                var hintLength = (int) Math.Ceiling( answer.Length * fraction * n );
                if( hintLength < answer.Length )
                {
                    yield return answer.Substring( 0, hintLength ) + "...";
                }
            }
        }
    }
}