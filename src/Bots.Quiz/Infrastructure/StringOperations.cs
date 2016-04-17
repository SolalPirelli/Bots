using System;
using System.Collections.Generic;
using System.Linq;

namespace Bots.Quiz.Infrastructure
{
    public static class StringOperations
    {
        /// <summary>
        /// Groups lines using empty lines as delimiters.
        /// </summary>
        public static IEnumerable<IReadOnlyList<string>> GroupLines( IEnumerable<string> lines )
        {
            var current = new List<string>();

            foreach( var line in lines.Select( l => l.Trim() ) )
            {
                if( line.Length == 0 )
                {
                    if( current.Count > 0 )
                    {
                        yield return current;
                        current = new List<string>();
                    }
                }
                else
                {
                    current.Add( line );
                }
            }

            if( current.Count > 0 )
            {
                yield return current;
            }
        }

        /// <summary>
        /// Splits the specified text using the specified separator.
        /// </summary>
        public static IReadOnlyList<string> Split( string text, string separator )
        {
            return text.Split( new[] { separator }, StringSplitOptions.None ).Select( s => s.Trim() ).ToArray();
        }
    }
}