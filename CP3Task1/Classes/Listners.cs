using System.Collections.Generic;

namespace CP3Task1.Classes
{
    public class Listners
    {
        private Dictionary<Port, DelegateCallToTerminal> callFromAtsToPortListner;
        private Dictionary<Terminal, DelegateCallToTerminal> callFromPortToTerminalListner;
        private Dictionary<Port, DelegateCallToTerminal> callFromTerminalToPortListner;
        private Dictionary<ATS, DelegateCallToTerminal> callFromPortToAtsListner;

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

        public Listners()
        {
            CallFromAtsToPortListner = new Dictionary<Port, DelegateCallToTerminal>();
            CallFromPortToTerminalListner = new Dictionary<Terminal, DelegateCallToTerminal>();
            CallFromTerminalToPortListner = new Dictionary<Port, DelegateCallToTerminal>();
            CallFromPortToAtsListner = new Dictionary<ATS, DelegateCallToTerminal>();
        }

        #region FromAtsToPort
        public void AddCallFromAtsToPortListener(Port port, DelegateCallToTerminal listener)
        {
            if (listener == null) return;
            if (callFromAtsToPortListner.ContainsKey(port)) callFromAtsToPortListner[port] += listener;
            else callFromAtsToPortListner[port] = listener;
        }

        public void DelCallFromAtsToPortListener(Port port, DelegateCallToTerminal listener)
        {
            if (listener == null) return;
            if (callFromAtsToPortListner.ContainsKey(port)) callFromAtsToPortListner[port] -= listener;
        }
        #endregion
        #region FromPortToTerminal
        public void AddCallFromPortToTerminalListener(Terminal terminal, DelegateCallToTerminal listener)
        {
            if (listener == null) return;
            if (callFromPortToTerminalListner.ContainsKey(terminal)) callFromPortToTerminalListner[terminal] += listener;
            else callFromPortToTerminalListner[terminal] = listener;
        }

        public void DelCallFromPortToTerminalListener(Terminal terminal, DelegateCallToTerminal listener)
        {
            if (listener == null) return;
            if (callFromPortToTerminalListner.ContainsKey(terminal)) callFromPortToTerminalListner[terminal] -= listener;
        }
        #endregion
        #region FromTerminalToPort
        public void AddCallFromTerminalToPortListener(Port port, DelegateCallToTerminal listener)
        {
            if (listener == null) return;
            if (callFromTerminalToPortListner.ContainsKey(port)) callFromTerminalToPortListner[port] += listener;
            else callFromTerminalToPortListner[port] = listener;
        }

        public void DelCallFromTerminalToPortListener(Port port, DelegateCallToTerminal listener)
        {
            if (listener == null) return;
            if (callFromTerminalToPortListner.ContainsKey(port)) callFromAtsToPortListner[port] -= listener;
        }
        #endregion
        #region FromPortToAts
        public void AddCallPortToAtsListener(ATS ats, DelegateCallToTerminal listener)
        {
            if (listener == null) return;
            if (callFromPortToAtsListner.ContainsKey(ats)) callFromPortToAtsListner[ats] += listener;
            else callFromPortToAtsListner[ats] = listener;
        }

        public void DelCallPortToAtsListener(ATS ats, DelegateCallToTerminal listener)
        {
            if (listener == null) return;
            if (callFromPortToAtsListner.ContainsKey(ats)) callFromPortToAtsListner[ats] -= listener;
        }
        #endregion

    }
}