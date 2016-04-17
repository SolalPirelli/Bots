using System.Collections.Generic;

namespace Bots.Quiz.Infrastructure
{
    public static class ListOperations
    {
        public static IReadOnlyList<T> Concat<T>( IEnumerable<T> left, IEnumerable<T> right )
        {
            var list = new List<T>( left );
            list.AddRange( right );
            return list;
        }

        public static IReadOnlyList<T> Concat<T>(IEnumerable<IEnumerable<T>> lists)
        {
            var list = new List<T>( );
            foreach( var subList in lists )
            {
                list.AddRange( subList );
            }
            return list;
        }
    }
}