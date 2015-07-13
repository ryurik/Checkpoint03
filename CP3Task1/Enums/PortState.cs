using System;

namespace CP3Task1
{
    [Flags]
    public enum PortStateForAts
    {
        UnPlugged = 0,
        Default = UnPlugged,
        Plugged = 1,
        Free = 2,
        Busy = 4
    }
}
