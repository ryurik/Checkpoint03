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

        public PhoneNumber PhoneNumber { get; set; }
        public PortStateForAts PortStateForAts {
            get { return _portStateForAts; }
            set
            {
                // try to subscribe
                if (value == (PortStateForAts.Plugged | PortStateForAts.Free))
                {
                    Program.Listners.AddCallFromAtsToPortListener(this, OnIncomingCall);
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
            Ats.TransferCallFromPortToTerminal(this, args);
            if (args is CallingEventArgs)
            {
                (args as CallingEventArgs).ConnectionResult = ConnectionResult.Ok;
            }
        }

        public void OnOutgoingCall(Object sender, EventArgs args)
        {
            if (args is PortEventArgs)
            {
                Trace.WriteLine("Incoming call from ATS!!!");
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
