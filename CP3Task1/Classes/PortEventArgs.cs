using System;
using CP3Task1.Enums;

namespace CP3Task1.Classes
{
    public class PortEventArgs : EventArgs
    {
        public PortStateForAts PortState { get; set; }
        public ConnectionPortResult ConnectionPortResult { get; set; }
    }
}
