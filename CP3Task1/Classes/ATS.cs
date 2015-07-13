using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Checkpoint03.Classes;
using CP3Task1.Classes.EventArgs;

namespace CP3Task1.Classes
{
    public delegate void DelegateCallToTerminal(Object Object, System.EventArgs args);

    class PortEvent<P, E>
    {
        public P _port;
        public E _event;

        public PortEvent(P inPort, E inEvent)
        {
            _port = inPort;
            _event = inEvent;
        }
    }

    [Serializable]
    public class ATS
    {
        private  EventHandler<System.EventArgs> callToTerminalEventHandler;

        public  event EventHandler<System.EventArgs> CallToTerminalEvent
        {
            add
            {
                if (callToTerminalEventHandler != null)
                {
                    if (!callToTerminalEventHandler.GetInvocationList().Contains(value))
                    {
                        callToTerminalEventHandler += value;
                    }
                }
                else
                {
                    callToTerminalEventHandler += value;
                }
            }
            remove { callToTerminalEventHandler -= value; }
        }



        void OnIncomingCall(Object sender, System.EventArgs args)
        {
            // BINGO!!!!
            var temp = callToTerminalEventHandler;
            if (temp != null)
            {
                temp(sender, args);
            }
            else
            {
                Console.WriteLine("A'm lonely s=" + (args as CallingEventArgs).Tagget.Number.ToString());
            }

        }

        public static DelegateCallToTerminal TestEvent(object Object, System.EventArgs args)
        {
            return null;
        }

//        private List<PortEvent<Port, DelegateCallToTerminal>> listCallToTerminals = new List<PortEvent<Port, DelegateCallToTerminal>>();
        private Dictionary<Port, DelegateCallToTerminal> callFromAtsToPortListner = new Dictionary<Port, DelegateCallToTerminal>();
        private Dictionary<Terminal, DelegateCallToTerminal> callFromPortToTerminalListner = new Dictionary<Terminal, DelegateCallToTerminal>();
        private Dictionary<Port, DelegateCallToTerminal> callFromTerminalToPortListner = new Dictionary<Port, DelegateCallToTerminal>();
        private Dictionary<ATS, DelegateCallToTerminal> callFromPortToAtsListner = new Dictionary<ATS, DelegateCallToTerminal>();


        #region FromPortToTerminal
        public void addCallFromPortToTerminalListener(Terminal terminal, DelegateCallToTerminal listener)
        {
            if (listener == null) return;
            if (callFromPortToTerminalListner.ContainsKey(terminal)) callFromPortToTerminalListner[terminal] += listener;
            else callFromPortToTerminalListner[terminal] = listener;
        }

        public void delCallFromPortToTerminalListener(Terminal terminal, DelegateCallToTerminal listener)
        {
            if (listener == null) return;
            if (callFromPortToTerminalListner.ContainsKey(terminal)) callFromPortToTerminalListner[terminal] -= listener;
        }
        #endregion
        #region FromAtsToPort
        public void addCallFromAtsToPortListener(Port port, DelegateCallToTerminal listener)
        {
            if (listener == null) return;
            if (callFromAtsToPortListner.ContainsKey(port)) callFromAtsToPortListner[port] += listener;
            else callFromAtsToPortListner[port] = listener;
        }

        public void delCallFromAtsToPortListener(Port port, DelegateCallToTerminal listener)
        {
            if (listener == null) return;
            if (callFromAtsToPortListner.ContainsKey(port)) callFromAtsToPortListner[port] += listener;
        }
        #endregion
        #region FromTerminalToPort
        public void addCallFromTerminalToPortListener(Port port, DelegateCallToTerminal listener)
        {
            if (listener == null) return;
            if (callFromTerminalToPortListner.ContainsKey(port)) callFromTerminalToPortListner[port] += listener;
            else callFromTerminalToPortListner[port] = listener;
        }

        public void delCallFromTerminalToPortListener(Port port, DelegateCallToTerminal listener)
        {
            if (listener == null) return;
            if (callFromTerminalToPortListner.ContainsKey(port)) callFromAtsToPortListner[port] += listener;
        }
        #endregion
        #region FromPortToAts
        public void addCallPortToAtsListener(ATS ats, DelegateCallToTerminal listener)
        {
            if (listener == null) return;
            if (callFromPortToAtsListner.ContainsKey(ats)) callFromPortToAtsListner[ats] += listener;
            else callFromPortToAtsListner[ats] = listener;
        }

        public void delCallPortToAtsListener(ATS ats, DelegateCallToTerminal listener)
        {
            if (listener == null) return;
            if (callFromPortToAtsListner.ContainsKey(ats)) callFromPortToAtsListner[ats] += listener;
        }
        #endregion

        public void TransferCallFromPortToTerminal(Object sender, System.EventArgs args)
        {
            if ((sender != null) && (sender is Port))
            {
                var t = _terminals.FirstOrDefault(x=>x.Number == (sender as Port).PhoneNumber.Number);
                if ((t != null) && (callFromPortToTerminalListner.ContainsKey(t)))
                {
                    callFromPortToTerminalListner[t](this, args);
                }
                else
                {
                    Console.WriteLine("Terminal with number {0} doesn't exist");
                }
            }
        }

        private PortSet _ports = new PortSet();
        private TerminalSet _terminals = new TerminalSet();
        
        public PortSet Ports
        {
            get { return _ports; }
            set { _ports = value; }
        }

        public TerminalSet Terminals
        {
            get { return _terminals; }
            set { _terminals = value; }
        }

        public ATS()
        {
            LoadData(); // LoadMainData - Starting ATS
        }

        #region WorkWithPorts

        public void ActivatePortsFromContracts()
        {
            Random r = new Random();
            for (int i = 0; i < Ports.Count / 2; i ++)
            {
                var tmp = _ports.Where(x => x.PortStateForAts == PortStateForAts.UnPlugged).ToArray();
                tmp[r.Next(1, tmp.Length)].PortStateForAts = PortStateForAts.Plugged | PortStateForAts.Free;
            }
        }

        public void CreateFirstPorts(int count = 50)
        {
            for (int i = 1; i <= count; i++)
            {
                Port port = new Port()
                {
                    PhoneNumber = new PhoneNumber()
                    {
                        Name = String.Format("{0}{1}", new String('0', 6 - i.ToString().Length), i),
                        Number = i
                    },
                    PortStateForAts = PortStateForAts.UnPlugged
                };
                Serializer.SaveToXml(Path.Combine(Program.AppPath, Program.PortData[0], Path.ChangeExtension(port.PhoneNumber.Name, Program.PortData[1])), port);
            }
        }

        #endregion

        #region WorkWithTerminals

        public void ActivateTerminalsFromContracts()
        {
            Random r = new Random();
            for (int i = 0; i < Ports.Count / 2; i++)
            {
                var tmp = _ports.Where(x => x.PortStateForAts == PortStateForAts.UnPlugged).ToArray();
                tmp[r.Next(1, tmp.Length)].PortStateForAts = PortStateForAts.Plugged;
            }
        }

        public void CreateFirstTerminals(int count = 50)
        {
            for (int i = 1; i <= count; i++)
            {
                Terminal terminal = new Terminal()
                {
                    Ats = null,
                    Number = i,
                    Port = null,
                    TerminalState = TerminalState.Off
                };
                Serializer.SaveToXml(Path.Combine(Program.AppPath, Program.TerminalData[0], Path.ChangeExtension(String.Format("{0}{1}", new String('0', 6 - terminal.Number.ToString().Length), terminal.Number), Program.TerminalData[1])), terminal);
            }
        }

        public void ConnectTerminals()
        {
            foreach (var t in Terminals)
            {
                t.Port = _ports.FirstOrDefault(x => x.PhoneNumber.Number == t.Number);

                if ((t.Port != null) && (t.Port.PortStateForAts == (PortStateForAts.Plugged | PortStateForAts.Free)))
                {
                    t.SwitchOn();
                    Console.WriteLine("Terminal #{0} are plugged", t.Number);
                    //t.ConnectToPort(this, null);
                }
            }
        }

        #endregion

        public void CallToTerminal(PhoneNumber phoneNumber)
        {
            var port = _ports.FirstOrDefault(x => x.PhoneNumber.Number == phoneNumber.Number);
            if ((port != null) && (port.PortStateForAts == (PortStateForAts.Plugged | PortStateForAts.Free)))
            {
                var args = new CallingEventArgs() {ConnectionResult = ConnectionResult.Default, Tagget = phoneNumber};
                if (callFromAtsToPortListner.ContainsKey(port))
                {
                    callFromAtsToPortListner[port](this, args);
                }
                else
                {
                    Console.WriteLine("Port with number {0} doesn't exist");
                    return;
                }
            }
        }
        
        private void LoadData()
        {
            var dir = new DirectoryInfo(Path.GetDirectoryName(Port.GenerateFileName(0)));
            if (!Directory.Exists(dir.FullName))
                return;
            // loading ports
            foreach (Port port in dir.GetFiles(searchPattern: String.Format("*.{0}", Program.PortData[1])).Select(f => Port.LoadFromFile(Int32.Parse(Path.GetFileNameWithoutExtension(f.Name)))))
            {
                port.Ats = this;
                _ports.Add(port); 
            }
            dir = new DirectoryInfo(Path.GetDirectoryName(Terminal.GenerateFileName(0)));
            if (!Directory.Exists(dir.FullName))
                return;
            // loading terminals
            foreach (Terminal terminal in dir.GetFiles(searchPattern: String.Format("*.{0}", Program.TerminalData[1])).Select(f => Terminal.LoadFromFile(Int32.Parse(Path.GetFileNameWithoutExtension(f.Name)))))
            {
                terminal.Ats = this;
                _terminals.Add(terminal);
            }
        }
    }
}
