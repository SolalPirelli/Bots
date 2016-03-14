using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bots
{
    public delegate void TypedEventHandler<TSender, TEventArgs>( TSender sender, TEventArgs eventArgs )
        where TEventArgs : EventArgs;
}