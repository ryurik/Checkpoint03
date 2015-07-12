using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Checkpoint03.Classes;
using CP3Task1.Classes;
using CP3Task1.Classes.EventArgs;
using CP3Task1.Enums;

namespace CP3Task1
{
    //public delegate Call
    public class Terminal
    {
        private int _number;
        private TerminalState _terminalState;
        private Port _port;
        private ATS _ats;

        public int Number
        {
            get { return _number; }
            set { _number = value; }
        }

        public TerminalState TerminalState
        {
            get { return _terminalState; }
            set
            {
                _terminalState = value; 
                ConnectToPort(value);
            }
        }

        public Port Port
        {
            get { return _port; }
            set
            {
                _port = value;
                if (value != null)
                {
                    Connecting += value.TerminalConnectingToPort;
                }
            }
        }

        public ATS Ats
        {
            get { return _ats; }
            set { _ats = value; }
        }


        private EventHandler<CallingEventArgs> _callInHandler;
        private EventHandler<CallingEventArgs> _callOutHandler;
        private EventHandler<PortEventArgs> _connectingHandler;

        public event EventHandler<CallingEventArgs> AfterCalling;

        public event EventHandler<CallingEventArgs> CallIn
        {
            add { _callInHandler += value; }
            remove { _callInHandler -= value; }
        }

        public event EventHandler<CallingEventArgs> CallOut
        {
            add { _callOutHandler += value; }
            remove { _callOutHandler -= value; }
        }

        public event EventHandler<PortEventArgs> Connecting
        {
            add { _connectingHandler += value; }
            remove { _connectingHandler -= value; }
        }

        public virtual void OnCalling(object sender, CallingEventArgs args)
        {
            var temp = _callInHandler;
            if (temp != null)
            {
                temp(sender, args); //Generate Event Call (port mut be subscribe to this event)
            }
        }

        protected void OnAfterCalling(CallingEventArgs args)
        {
            if (AfterCalling != null)
            {
                AfterCalling(this, args);
            }
        }

        protected virtual void OnConnecting(object sender, PortEventArgs args)
        {
            // try to connect to port
            var temp = _connectingHandler;
            if (temp != null)
            {
                temp(this, args);
            }
        }


        public ConnectionResult StartCall(PhoneNumber targetPhoneNumber)
        {
            if ((_port != null) && (_port.PortStateForAts == PortStateForAts.Free))
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
            return ConnectionResult.Default;
        }

        public void SwitchOn()
        {
            TerminalState = TerminalState.On;
            if ((Port != null) && (Port.PortStateForAts == PortStateForAts.Plugged))
            {
                this.CallIn += Port.OnIncomingCall;
                Port.OutgoingCall += this.OnCalling;
            }
        }

        public void SwitchOff()
        {
            TerminalState = TerminalState.Off;
        }

        public void ConnectToPort(TerminalState terminalState)
        {
            var args = new PortEventArgs()
            {
                ConnectionPortResult = ConnectionPortResult.Default, 
                PortState = PortStateForAts.Default, 
                TerminalState = terminalState
            };
            OnConnecting(this, args);
        }

        #region WorkWithFiles
        public static void SaveToFile(PhoneNumber phoneNumber)
        {

            Terminal terminal = new Terminal()
            {
                Ats = null,
                _number = phoneNumber.Number,
                _port = null,
                _terminalState = TerminalState.Off,
            };
            string fileName = GenerateFileName(phoneNumber.Number);
            Serializer.SaveToXml(fileName, terminal);

        }
        public static Terminal LoadFromFile(int phoneNumber)
        {
            string fileName = GenerateFileName(phoneNumber);
            if (!File.Exists(fileName))
            {
                return null;
            }
            return Serializer.LoadFromXml<Terminal>(fileName);
        }


        public static string GenerateFileName(int portNumber)
        {
            return Path.Combine(Program.AppPath, Program.TerminalData[0], Path.ChangeExtension(String.Format("{0}{1}", new String('0', 6 - portNumber.ToString().Length), portNumber), Program.TerminalData[1]));
        }
        #endregion

        public void ConnectFromPort(object sender, TerminalState args)
        {
            // if we have a reaction from terminal its mean what terminal is On
            args = TerminalState.On;
        }
    }
}
