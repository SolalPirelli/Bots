using System;

namespace Bots
{
    public sealed class UserEventArgs : EventArgs
    {
        public IUser User { get; }

        public UserEventArgs( IUser user )
        {
            User = user;
        }
    }
}