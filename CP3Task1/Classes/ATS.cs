using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Checkpoint03.Classes;
using CP3Task1.Classes.EventArgs;

namespace CP3Task1.Classes
{
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
        private static object userOpSync = new object();

        private EventHandler<System.EventArgs> callToTerminalEventHandler;
        private Dictionary<TarifficateEvent, CallInfo> tarifficator = new Dictionary<TarifficateEvent, CallInfo>();
        private int callID = 0;
        private List<TarifficateEvent> listTarifficateEvents = new List<TarifficateEvent>();

        private int getNewId()
        {
            return ++callID;
        }

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

        public static DelegateCallToTerminal TestEvent(object Object, System.EventArgs args)
        {
            return null;
        }

//        private List<PortEvent<Port, DelegateCallToTerminal>> listCallToTerminals = new List<PortEvent<Port, DelegateCallToTerminal>>();


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
            //we must to subscribe ATS to HangUp
            Program.Listners.AddHangUpFromPortToAtsListener(this, OnHangUpFromPort);

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
                var callEventStart = DateTime.Now;
                var id = getNewId();
                var args = new CallingEventArgs() {Id=id, CallStart = callEventStart, ConnectionResult = ConnectionResult.Default, Tagget = phoneNumber };
                if (Program.Listners.CallFromAtsToPortListner.ContainsKey(port))
                {
                    lock (userOpSync)
                    {
                        Console.WriteLine("Starting call #{0} at:{1}", id, callEventStart);
                        Program.Listners.CallFromAtsToPortListner[port](this, args); // Start call (from ATS)
                        TarifficateEvent tarifficateEvent = new TarifficateEvent() {ID = id};
                        listTarifficateEvents.Add(tarifficateEvent);
                        if (args.ConnectionResult == ConnectionResult.Ok)
                        {
                            tarifficator.Add(tarifficateEvent,
                                new CallInfo()
                                {
                                    Caller = null,
                                    Receiver = port,
                                    StartCall = callEventStart,
                                    EndCall = callEventStart
                                });
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Port with number {0} doesn't exist");
                }
            }
        }

        public void OnHangUpFromPort(Object Object, HangUpEventArgs args)
        {
            // TransferHangUpToCallerPort(Object, args); // not in this life
            // Okay - we get HangUp event from port. Now we must write all data to store
            TarifficateEvent te = listTarifficateEvents.FirstOrDefault(x => x.ID == args.Id);
            if ((te != null) && (tarifficator.ContainsKey(te)))
            {
                CallInfo ci = tarifficator[te];
                ci.EndCall = DateTime.Now;
                tarifficator[te] = ci;
                Console.WriteLine("Call #{0} was end at:{1}", args.Id, DateTime.Now);
            }
            else
            {
                Console.WriteLine("Cann't to find call #{0} !!!!!!!!!", args.Id);
            }
            args.Id = 0;
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

        internal void ShowStatistic()
        {
            foreach (var t in tarifficator)
            {
                DateTime dt;
                double d;
                if (DateTime.TryParse(t.Value.EndCall.ToString(), out dt))
                {
                    d = (dt - t.Value.StartCall).TotalSeconds;
                }
                else
                {
                    d = 0;
                }
                Console.WriteLine("#{0}"+"\t"+"Destination:{1}"+"\t"+"StartTime{2}"+"\t"+"EndTime{3}"+"\"+Duration in sec:{4:0}", t.Key.ID, t.Value.Receiver.PhoneNumber.Number, t.Value.StartCall, t.Value.EndCall, d);                
            }
        }
    }
}
