using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CP3Task1
{
    public enum ConnectionResult
    {
        StationFail,
        Default = StationFail,
        TargetBusy,
        NoAnswer,
        WrongNumber,
        Ok,
    }
}
