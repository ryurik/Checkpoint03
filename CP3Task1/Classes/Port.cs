using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using Checkpoint03.Classes;
using CP3Task1.Classes;
using CP3Task1.Classes.EventArgs;
using CP3Task1.Enums;
// dzmitry_nikitsik@epam.com

namespace CP3Task1
{
    [Serializable]
    public class Port
    {
        private PortStateForAts _portStateForAts;
        private int _callId;

        public PhoneNumber PhoneNumber { get; set; }
        public PortStateForAts PortStateForAts {
            get { return _portStateForAts; }
            set
            {
                // try to subscribe
                if (value == (PortStateForAts.Plugged | PortStateForAts.Free))
                {
                    Program.Listners.AddCallFromAtsToPortListener(this, OnIncomingCall);
                    Program.Listners.DelHangUpFromTerminalToPortListener(this, OnHangUpFromTerminal);
                }
                else if (value == (PortStateForAts.Plugged | PortStateForAts.Busy ))
                {
                    Program.Listners.DelCallFromAtsToPortListener(this, OnIncomingCall);
                    Program.Listners.AddHangUpFromTerminalToPortListener(this, OnHangUpFromTerminal);
                }
                _portStateForAts = value;
            }
        }
        public ATS Ats { get; set; }

        private EventHandler<EventArgs> _portEventHandler;

        public event EventHandler<EventArgs> PortEvent
        {
            add
            {
                if (_portEventHandler != null)
                {
                    if (!_portEventHandler.GetInvocationList().Contains(value))
                    {
                        _portEventHandler += value;
                    }
                }
                else
                {
                    _portEventHandler += value;
                }
            }
            remove { _portEventHandler -= value; }
        }


        // event "ConnectToPort" from Terminal 
        public void TerminalConnectingToPort(Object sender, PortEventArgs args)
        {
            args.PortStateForAts = PortStateForAts;
            args.ConnectionPortResult = ConnectionPortResult.PortListning;
        }

        public void OnIncomingCall(Object sender, EventArgs args)
        {
            if ((PortStateForAts & PortStateForAts.Busy) != 0)
            {
                if (args is CallingEventArgs)
                {
                    (args as CallingEventArgs).ConnectionResult = ConnectionResult.TargetBusy;
                }
                return;
            }

            if (args is CallingEventArgs)
            {
                _callId = (args as CallingEventArgs).Id;
                Trace.WriteLine(String.Format("I'm port #{0} and I have incoming call from ATS!!!", PhoneNumber.Number));
                TransferCallToTerminal(this, args);
                if ((args as CallingEventArgs).ConnectionResult == ConnectionResult.Ok)
                {
                    PortStateForAts = (PortStateForAts.Plugged | PortStateForAts.Busy);
                } 
            }
        }

        private void TransferCallToTerminal(object sender, EventArgs args)
        {
            Trace.WriteLine(String.Format("Try to transfer call from port #{0} to terminal #{0}", PhoneNumber.Number));
            TransferCallFromPortToTerminal(this, args);
            if (args is CallingEventArgs)
            {
                (args as CallingEventArgs).ConnectionResult = ConnectionResult.Ok;
            }
            //// we must subscribe the port to HangUpEvent from terminal
            //Program.Listners.AddHangUpFromTerminalToPortListener(this, OnHangUpFromTerminal);
        }

        public void TransferCallFromPortToTerminal(Object sender, System.EventArgs args)
        {
            if ((sender != null) && (sender is Port))
            {
                var t = Ats.Terminals.FirstOrDefault(x => x.Number == (sender as Port).PhoneNumber.Number);
                if ((t != null) && (Program.Listners.CallFromPortToTerminalListner.ContainsKey(t)))
                {
                    Program.Listners.CallFromPortToTerminalListner[t](this, args);
                }
                else
                {
                    Console.WriteLine("Terminal with number {0} doesn't exist");
                }
            }
        }

        public void OnHangUpFromTerminal(Object Object, HangUpEventArgs args)
        {
            TransferHangUpToAts(Object, args);
        }

        private void TransferHangUpToAts(object Object, HangUpEventArgs args)
        {
            if (Program.Listners.HangUpFromPortToAtsListner.ContainsKey(Ats))
            {
                Program.Listners.HangUpFromPortToAtsListner[Ats](this, args); // HangUp from Port->Ats
            }
            if (args.Id == 0)
            {
                Trace.WriteLine(String.Format("Ats say - Call #{0} was end at:{1}", _callId, DateTime.Now));
                _callId = 0;
                PortStateForAts = (PortStateForAts.Plugged | PortStateForAts.Free);
            }
        }


        public void OnOutgoingCall(Object sender, EventArgs args)
        {
            if (args is PortEventArgs)
            {
                Trace.WriteLine("Incoming call from terminal");
                TransferCallToAts(sender, args);
            }
        }

        private void TransferCallToAts(object sender, EventArgs args)
        {
            throw new NotImplementedException();
        }

        #region Work with  files
        public static void SaveToFile(PhoneNumber phoneNumber, PortStateForAts portState)
        {

            Port port = new Port()
            {
                PhoneNumber = phoneNumber,
                PortStateForAts = portState,
                Ats = null,
            };
            string fileName = GenerateFileName(phoneNumber.Number);
            Serializer.SaveToXml(fileName, port);

        }
        public static Port LoadFromFile(int phoneNumber)
        {
            string fileName = GenerateFileName(phoneNumber);
            if (!File.Exists(fileName))
            {
                return null;
            }
            return Serializer.LoadFromXml<Port>(fileName);
        }

        public static string GenerateFileName(int portNumber)
        {
            return Path.Combine(Program.AppPath, Program.PortData[0], Path.ChangeExtension(String.Format("{0}{1}", new String('0', 6 - portNumber.ToString().Length), portNumber), Program.PortData[1]));
        }
        #endregion
    }
}
