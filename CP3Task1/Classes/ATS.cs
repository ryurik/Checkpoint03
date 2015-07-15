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
            //LoadData(); // LoadMainData - Starting ATS
            //we must to subscribe ATS to HangUp
            Program.Listners.AddHangUpFromPortToAtsListener(this, OnHangUpFromPort);

        }

        #region WorkWithPorts

        public void ActivatePortsFromContracts()
        {
            Random r = new Random();
            foreach (var p in Ports)
            {
                p.PortStateForAts = PortStateForAts.UnPlugged;
            }

            for (int i = 0; i < r.Next(1, Ports.Count); i ++)
            {
                if (_ports.Where(x => x.PortStateForAts == PortStateForAts.UnPlugged).Any())
                {
                    var tmp = _ports.Where(x => x.PortStateForAts == PortStateForAts.UnPlugged).ToArray();
                    var p = tmp[r.Next(0, tmp.Length - 1)];
                    p.PortStateForAts = PortStateForAts.Plugged;
                    p.PortStateForAts = PortStateForAts.Plugged | PortStateForAts.Free;
                }
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
            foreach (var t in Terminals)
            {
                t.SwitchOff();
            }
            var pluggedPorts = _ports.Where(x => x.PortStateForAts == (PortStateForAts.Plugged | PortStateForAts.Free)).ToArray();
            foreach (var tmp in pluggedPorts)
            {
                var t = Terminals.FirstOrDefault(x => x.Number == tmp.PhoneNumber.Number);
                if (t != null)
                {
                    if (r.Next(1, 10) < 6)
                        t.SwitchOn();
                    else
                        t.SwitchOff();
                } 
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
                        Console.WriteLine("#{0} Starting call to Terminal:{1} at:{2}", id, phoneNumber.Number, callEventStart);
                        var tmp = Program.Listners.CallFromAtsToPortListner[port];
                        if (tmp != null)
                        {
                            tmp(this, args); // Start call (from ATS)
                        }
                        else
                        {
                            Console.WriteLine("Port #{0} don't listen ATS", phoneNumber.Number);
                            return;
                        }
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

        
        public void LoadData()
        {
            var dir = new DirectoryInfo(Path.GetDirectoryName(Port.GenerateFileName(0)));
            if (!Directory.Exists(dir.FullName))
                return;
            // loading ports
            foreach (Port port in dir.GetFiles(searchPattern: String.Format("*.{0}", Program.PortData[1])).Select(f => Port.LoadFromFile(Int32.Parse(Path.GetFileNameWithoutExtension(f.Name)))))
            {
                if (!_ports.Where(x => x.PhoneNumber.Number == port.PhoneNumber.Number).Any())
                {
                    port.Ats = this;
                    _ports.Add(port);
                }
            }
            dir = new DirectoryInfo(Path.GetDirectoryName(Terminal.GenerateFileName(0)));
            if (!Directory.Exists(dir.FullName))
                return;
            // loading terminals
            foreach (Terminal terminal in dir.GetFiles(searchPattern: String.Format("*.{0}", Program.TerminalData[1])).Select(f => Terminal.LoadFromFile(Int32.Parse(Path.GetFileNameWithoutExtension(f.Name)))))
            {
                if (!_terminals.Where(x => x.Number == terminal.Number).Any())
                {
                    terminal.Ats = this;
                    terminal.Port = Ports.FirstOrDefault(x => x.PhoneNumber.Number == terminal.Number);
                    _terminals.Add(terminal);
                }
            }
            Program.Listners.ClearAllListners();
            ActivatePortsFromContracts();
            ActivateTerminalsFromContracts();
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
                Console.WriteLine("#{0}"+"\t"+"Destination:{1}"+"\t"+"StartTime:{2}"+"\t"+"EndTime:{3}"+"\t"+"Duration in sec:{4:0}", t.Key.ID, t.Value.Receiver.PhoneNumber.Number, t.Value.StartCall, t.Value.EndCall, d);                
            }
        }

        internal void ShowAtsStates()
        {
            Console.WriteLine("Ports plugged:{0}" + "\n" + "Terminals on:{1}",
                Ports.Where(x => x.PortStateForAts == (PortStateForAts.Plugged | PortStateForAts.Free)).Count(),
                Terminals.Where(x => x.TerminalState == TerminalState.On).Count());
        }

        internal void CallToRandomTerminal()
        {
            Random r = new Random();        
            var t = Terminals.Where(x => (x.TerminalState == TerminalState.On) && (x.Port.PortStateForAts == (PortStateForAts.Plugged | PortStateForAts.Free)));

            if (t.Any())
            {
                CallToTerminal(new PhoneNumber(){Number = t.ToArray()[r.Next(0, t.Count())].Number}); // try to call first pluged and free number
            }
            else
            {
                Console.WriteLine("No free terminals");
            }

        }

        internal void Help()
        {

            var originalColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Press ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("F1");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" to show help." + "\n");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("Press 1 to call to random free terminal.");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("(results see in Output window in debug mode)");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Press 0 to check plugged ports and terminals");
            Console.ForegroundColor = ConsoleColor.White;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Press 9 to show statistic");
            Console.ForegroundColor = ConsoleColor.White;

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Press 8 to reload ATS");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ForegroundColor = originalColor;
        }

        internal void ShowActualStatistic()
        {
            Console.WriteLine("After activation {0} : {1} ", PortStateForAts.Plugged, Ports.Where(x => x.PortStateForAts == (PortStateForAts.Plugged | PortStateForAts.Free)).Count());
            Console.WriteLine("");
        }
    }
}
