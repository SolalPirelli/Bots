using System;
using System.Threading.Tasks;

namespace Bots
{
    public interface IScheduler
    {
        Task Delay( string id, TimeSpan time );
    }
}