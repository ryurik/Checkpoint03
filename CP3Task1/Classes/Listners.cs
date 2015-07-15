using System;
using System.Collections.Generic;
using CP3Task1.Classes.EventArgs;

namespace CP3Task1.Classes
{
    public delegate void DelegateCallToTerminal(Object Object, System.EventArgs args);
    public delegate void DelegateHangUp(Object Object, HangUpEventArgs args);

    public class Listners
    {
        #region CallListners
        private Dictionary<Port, DelegateCallToTerminal> callFromAtsToPortListner;
        private Dictionary<Terminal, DelegateCallToTerminal> callFromPortToTerminalListner;
        private Dictionary<Port, DelegateCallToTerminal> callFromTerminalToPortListner;
        private Dictionary<ATS, DelegateCallToTerminal> callFromPortToAtsListner;
        #endregion

        #region HangUpListners
        private Dictionary<Port, DelegateHangUp> hangUpFromTerminalToPortListner;
        private Dictionary<ATS, DelegateHangUp> hangUpFromPortToAtsListner;
        #endregion

        public Dictionary<Port, DelegateCallToTerminal> CallFromAtsToPortListner { 
            get{return callFromAtsToPortListner;}
            set { callFromAtsToPortListner = value;}
        }
        public Dictionary<Terminal, DelegateCallToTerminal> CallFromPortToTerminalListner { 
            get{return callFromPortToTerminalListner;} 
            set { callFromPortToTerminalListner = value;} 
        }
        public Dictionary<Port, DelegateCallToTerminal> CallFromTerminalToPortListner { 
            get {return callFromTerminalToPortListner;}
            set { callFromTerminalToPortListner = value; }
        }
        public Dictionary<ATS, DelegateCallToTerminal> CallFromPortToAtsListner { 
            get{return callFromPortToAtsListner;}
            set { callFromPortToAtsListner = value; }
        }

        public Dictionary<Port, DelegateHangUp> HangUpFromTerminalToPortListner
        {
            get { return hangUpFromTerminalToPortListner; }
            set { hangUpFromTerminalToPortListner = value; }
        }
        public Dictionary<ATS, DelegateHangUp> HangUpFromPortToAtsListner
        {
            get { return hangUpFromPortToAtsListner; }
            set { hangUpFromPortToAtsListner = value; }
        }


        public Listners()
        {
            CallFromAtsToPortListner = new Dictionary<Port, DelegateCallToTerminal>();
            CallFromPortToTerminalListner = new Dictionary<Terminal, DelegateCallToTerminal>();
            CallFromTerminalToPortListner = new Dictionary<Port, DelegateCallToTerminal>();
            CallFromPortToAtsListner = new Dictionary<ATS, DelegateCallToTerminal>();
            
            HangUpFromTerminalToPortListner = new Dictionary<Port, DelegateHangUp>();
            HangUpFromPortToAtsListner = new Dictionary<ATS, DelegateHangUp>();
        }

        #region FromAtsToPort
        public void AddCallFromAtsToPortListener(Port port, DelegateCallToTerminal listener)
        {
            if (listener == null) return;
            if (callFromAtsToPortListner.ContainsKey(port))
            {
                callFromAtsToPortListner[port] += listener;
            }
            else
                callFromAtsToPortListner[port] = listener;
        }

        public void DelCallFromAtsToPortListener(Port port, DelegateCallToTerminal listener)
        {
            if (listener == null) return;
            if (callFromAtsToPortListner.ContainsKey(port))
                callFromAtsToPortListner[port] -= listener;
        }
        #endregion
        #region FromPortToTerminal
        public void AddCallFromPortToTerminalListener(Terminal terminal, DelegateCallToTerminal listener)
        {
            if (listener == null) return;
            if (callFromPortToTerminalListner.ContainsKey(terminal))
            {
                callFromPortToTerminalListner[terminal] += listener;
            }
            else
                callFromPortToTerminalListner[terminal] = listener;
        }

        public void DelCallFromPortToTerminalListener(Terminal terminal, DelegateCallToTerminal listener)
        {
            if (listener == null) return;
            if (callFromPortToTerminalListner.ContainsKey(terminal)) 
                callFromPortToTerminalListner[terminal] -= listener;
        }

        public void ClearCallFromPortToTerminal(Terminal terminal)
        {
            if (callFromPortToTerminalListner.ContainsKey(terminal))
            {
                callFromPortToTerminalListner.Remove(terminal);
            }
        }
        #endregion
        #region FromTerminalToPort
        public void AddCallFromTerminalToPortListener(Port port, DelegateCallToTerminal listener)
        {
            if (listener == null) return;
            if (callFromTerminalToPortListner.ContainsKey(port))
            {
                callFromTerminalToPortListner[port] += listener;
            }
            else
                callFromTerminalToPortListner[port] = listener;
        }

        public void DelCallFromTerminalToPortListener(Port port, DelegateCallToTerminal listener)
        {
            if (listener == null) return;
            if (callFromTerminalToPortListner.ContainsKey(port)) 
                callFromTerminalToPortListner[port] -= listener;
        }
        #endregion
        #region FromPortToAts
        public void AddCallFromPortToAtsListener(ATS ats, DelegateCallToTerminal listener)
        {
            if (listener == null) return;
            if (callFromPortToAtsListner.ContainsKey(ats))
            {
                callFromPortToAtsListner[ats] += listener;
            }
            else
                callFromPortToAtsListner[ats] = listener;
        }

        public void DelCallFromPortToAtsListener(ATS ats, DelegateCallToTerminal listener)
        {
            if (listener == null) return;
            if (callFromPortToAtsListner.ContainsKey(ats))
                callFromPortToAtsListner[ats] -= listener;
        }
        #endregion

        #region HangUpFromTerminalToPort
        public void AddHangUpFromTerminalToPortListener(Port port, DelegateHangUp listener)
        {
            if (listener == null) return;
            if (HangUpFromTerminalToPortListner.ContainsKey(port))
            {
                HangUpFromTerminalToPortListner[port] += listener;
            }
            else
                HangUpFromTerminalToPortListner[port] = listener;
        }

        public void DelHangUpFromTerminalToPortListener(Port port, DelegateHangUp listener)
        {
            if (listener == null) return;
            if (HangUpFromTerminalToPortListner.ContainsKey(port)) 
                HangUpFromTerminalToPortListner[port] -= listener;
        }
        #endregion
        #region HangUpFromPortToAts
        public void AddHangUpFromPortToAtsListener(ATS ats, DelegateHangUp listener)
        {
            if (listener == null) return;
            if (HangUpFromPortToAtsListner.ContainsKey(ats))
            {
                HangUpFromPortToAtsListner[ats] += listener;
            }
            else
                HangUpFromPortToAtsListner[ats] = listener;
        }

        public void DelHangUpFromPortToAtsListener(ATS ats, DelegateHangUp listener)
        {
            if (listener == null) return;
            if (HangUpFromPortToAtsListner.ContainsKey(ats)) 
                HangUpFromPortToAtsListner[ats] -= listener;
        }
        #endregion


        public void ClearAllListners()
        {
            callFromAtsToPortListner.Clear();
            callFromPortToTerminalListner.Clear();
            callFromTerminalToPortListner.Clear();
            callFromPortToAtsListner.Clear();

            hangUpFromTerminalToPortListner.Clear();
            //hangUpFromPortToAtsListner.Clear();
        }
    }
}