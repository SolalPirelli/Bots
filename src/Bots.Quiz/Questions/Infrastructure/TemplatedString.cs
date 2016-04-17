using System;
using System.Collections.Generic;

namespace Bots.Quiz.Questions.Infrastructure
{
    public static class TemplatedString
    {
        public static bool HasHole( string template )
        {
            return template.Contains( "___" );
        }

        public static IReadOnlyList<string> FillHoles( IReadOnlyList<string> templates, IReadOnlyList<string> values )
        {
            var result = new List<string>();

            if( templates.Count == 0 )
            {
                if( values.Count != 0 )
                {
                    throw new ArgumentException( "Values were specified, but no template was given." );
                }
            }

            var index = 0;
            foreach( var template in templates )
            {
                var current = template;
                while( HasHole( current ) )
                {
                    if( index == values.Count )
                    {
                        throw new ArgumentException( $"Not enough values ({values.Count}) for template: {template}" );
                    }

                    current = ReplaceHole( current, values[index] );
                    index++;
                }

                result.Add( current );
            }

            if( index < values.Count )
            {
                throw new ArgumentException( $"Too many values ({values.Count}) for template: {templates[0]}" );
            }

            return result;
        }

        private static string ReplaceHole( string text, string replacement )
        {
            var pos = text.IndexOf( "___" );
            return text.Substring( 0, pos ) + replacement + text.Substring( pos + 3 );
        }
    }
}