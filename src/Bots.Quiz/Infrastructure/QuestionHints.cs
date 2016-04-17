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
                yield return answer.Substring( 0, (int) Math.Ceiling( answer.Length * fraction * n ) );
            }
        }
    }
}