using System;

namespace CP3Task1.Interfaces
{
    public interface ICallingEventArgs
    {
        DateTime CallStart { get; }
        PhoneNumber Tagget { get; }
        ConnectionResult ConnectionResult { get;}
    }
}