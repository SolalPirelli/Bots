using System;
using System.Threading;

namespace Bots.Quiz.Infrastructure
{
    public static class RandomOperations
    {
        private static readonly ThreadLocal<Random> Random = new ThreadLocal<Random>( () => new Random() );

        public static T Pick<T>(params T[] options)
        {
            return options[Random.Value.Next( options.Length )];
        }
    }
}