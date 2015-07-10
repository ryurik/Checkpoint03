using System;

namespace CP3Task1
{
    [Flags]
    public enum PortStateForAts
    {
        UnPlugged,
        Default = UnPlugged,
        Plugged,
        Free,
        Busy
    }
}
