using System;
using System.Collections.Generic;
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
        public PhoneNumber PhoneNumber { get; set; }
        public PortStateForAts PortStateForAts { get; set; }
        public ATS Ats { get; set; }

        private EventHandler<EventArgs> _portEventHandler;
        private EventHandler<CallingEventArgs> _incomingCallEventHandler;
        private EventHandler<CallingEventArgs> _outgoingCallEventHandler;

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

        public event EventHandler<CallingEventArgs> IncomingCall
        {
            add
            {
                if (_incomingCallEventHandler != null)
                {
                    if (!_incomingCallEventHandler.GetInvocationList().Contains(value))
                    {
                        _incomingCallEventHandler += value;
                    }
                }
                else
                {
                    _incomingCallEventHandler += value;
                }
            }
            remove { _incomingCallEventHandler -= value; }
        }

        public event EventHandler<CallingEventArgs> OutgoingCall
        {
            add
            {
                if (_outgoingCallEventHandler != null)
                {
                    if (!_outgoingCallEventHandler.GetInvocationList().Contains(value))
                    {
                        _outgoingCallEventHandler += value;
                    }
                }
                else
                {
                    _outgoingCallEventHandler += value;
                }
            }
            remove { _outgoingCallEventHandler -= value; }
        }


        protected virtual void OnPortEvent(Object sender, EventArgs args)
        {
            var temp = _portEventHandler;
            if (temp != null)
            {
                temp(sender, args);
            }
        }

        // event "ConnectToPort" from Terminal 
        public void TerminalConnectingToPort(Object sender, PortEventArgs args)
        {
            //if (args.)
            args.PortState = PortStateForAts;
            args.ConnectionPortResult = ConnectionPortResult.PortListning;
        }

        public void OnIncomingCall(Object sender, EventArgs args)
        {
            if (args is PortEventArgs)
            {
                Console.WriteLine("Incoming call from ATS!!!");
                TransferCallToTerminal(sender, args);
            }
        }

        private void TransferCallToTerminal(object sender, EventArgs args)
        {
            throw new NotImplementedException();
        }

        public void OnOutgoingCall(Object sender, EventArgs args)
        {
            if (args is PortEventArgs)
            {
                Console.WriteLine("Incoming call from ATS!!!");
                TransferCallToAts(sender, args);
            }
        }

        private void TransferCallToAts(object sender, EventArgs args)
        {
            throw new NotImplementedException();
        }


        public void GenerateEvent()
        {
            OnPortEvent(this, null);
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
