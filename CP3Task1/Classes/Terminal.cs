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
        public Port Port { get; set; }
        public ATS Ats { get; set; }

        private EventHandler<CallingEventArgs> _calling;

        public event EventHandler<CallingEventArgs> AfterCalling;

        public event EventHandler<CallingEventArgs> Calling
        {
            add { _calling += value; }
            remove { _calling -= value; }

        }

        protected virtual void OnCalling(object sender, CallingEventArgs args)
        {
            if (_calling != null)
            {
                _calling(sender, args);
            }
        }

        protected void OnAfterCalling(CallingEventArgs args)
        {
            if (AfterCalling != null)
            {
                AfterCalling(this, args);
            }
        }


        public ConnectionResult StartCall(PhoneNumber targetPhoneNumber)
        {
            if ((Port != null) && (Port.PortState == PortState.Free))
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

        public void SwitchOn()
        {
            if (Ats != null)
            {
                Ats.
            }
            Port = 
        }
        public void SwitchOff()
        {

        }
    }
}
