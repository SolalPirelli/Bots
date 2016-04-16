using System;
using System.Threading.Tasks;

namespace Bots.Quiz
{
    public static class SchedulerExtensions
    {
        public static Task Delay( this IScheduler scheduler, string id, TimeSpan time, QuestionSpeed speed )
        {
            var multiplier = speed == QuestionSpeed.Fast ? 0.5
                           : speed == QuestionSpeed.Medium ? 1.0
                           : 1.5;

            time = TimeSpan.FromTicks( (long) ( time.Ticks * multiplier ) );

            return scheduler.Delay( id, time );
        }
    }
}