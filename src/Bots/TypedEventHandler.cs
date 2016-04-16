using System;

namespace Bots
{
    public delegate void TypedEventHandler<TSender, TEventArgs>( TSender sender, TEventArgs eventArgs )
        where TEventArgs : EventArgs;
}