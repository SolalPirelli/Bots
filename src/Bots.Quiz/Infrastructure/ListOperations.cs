using System.Collections.Generic;

namespace Bots.Quiz.Infrastructure
{
    public static class ListOperations
    {
        public static IReadOnlyCollection<T> Concat<T>( IEnumerable<T> left, IEnumerable<T> right )
        {
            var list = new List<T>( left );
            list.AddRange( right );
            return list;
        }
    }
}