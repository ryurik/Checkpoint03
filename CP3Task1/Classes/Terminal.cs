using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CP3Task1
{
    public class Terminal
    {
        public int Number { get; set; }
        public TerminalState TerminalState { get; set; }


        public ConnectionResult StartCall(PhoneNumber targetPhoneNumber)
        {
            if (this.PortState == PortState.Free)
            {
                var args = new CallingEventArgs()
                {
                    Tagget = targetPhoneNumber,
                    ConnectionResult = ConnectionResult.Default
                };
                OnCalling(this, args);
                OnAfterCalling(args);
                return args.ConnectionResult;

            }
            else
            {
                return ConnectionResult.Default;
            }

        }
    }
}
