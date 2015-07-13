using System;
using CP3Task1.Interfaces;

namespace CP3Task1.Classes.EventArgs
{
    public class CallingEventArgs : System.EventArgs, ICallingEventArgs
    {
        public int Id { get; set; }

        public DateTime CallStart { get; set; }
        public PhoneNumber Tagget { get; set; }
        public ConnectionResult ConnectionResult { get; set; }
    }
}
