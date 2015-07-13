using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Checkpoint03.Classes;
using CP3Task1.Classes;
using CP3Task1.Classes.EventArgs;
using CP3Task1.Enums;
using System.Threading;

namespace CP3Task1
{
    //public delegate Call
    public class Terminal
    {
        private static object userOpSync = new object();

        private String[] _colorNames = ConsoleColor.GetNames(typeof(ConsoleColor));

        private int _callId;
        private int _number;
        private TerminalState _terminalState;
        private Port _port;
        private ATS _ats;
        private DateTime _callStartTime;
        private Timer _timer;


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
                if (_terminalState == value)
                    return;
                switch (value)
                {
                    case TerminalState.On:
                        ConnectToPort(value);
                        if (Ats != null)
                        {
                            Program.Listners.AddCallFromPortToTerminalListener(this, OnIncomingCall);
                        }
                        break;
                    case TerminalState.Off:
                        DisconnectFromPort(value);
                        if (Ats != null)
                        {
                            Program.Listners.DelCallFromPortToTerminalListener(this, OnIncomingCall);
                        }
                        break;
                }
                _terminalState = value; 
            }
        }

        private void DisconnectFromPort(CP3Task1.TerminalState value)
        {
            var tmp = _connectingHandler;
            if (tmp != null)
            {
                var arg = new PortEventArgs()
                {
                    ConnectionPortResult = ConnectionPortResult.Default,
                    PortStateForAts = PortStateForAts.Default,
                    TerminalState = TerminalState.Off
                };
                tmp(this, arg);
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


        private EventHandler<PortEventArgs> _connectingHandler;

        public event EventHandler<CallingEventArgs> AfterCalling;


        public event EventHandler<PortEventArgs> Connecting
        {
            add { _connectingHandler += value; }
            remove { _connectingHandler -= value; }
        }

        public void OnIncomingCall(Object sender, EventArgs args)
        {
            Trace.WriteLine(String.Format("I'am a terminal #{0} and I'm get Call from ATS(Port). Time:{1}", Number, DateTime.Now));
            _callStartTime = DateTime.Now;
            _timer = new Timer(TimerCallback, null, 0, 1000);
            if (args is CallingEventArgs)
                _callId = (args as CallingEventArgs).Id;
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
                OnAfterCalling(args);
                return args.ConnectionResult;

            }
            return ConnectionResult.Default;
        }

        public void SwitchOn()
        {
            TerminalState = TerminalState.On;
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
                PortStateForAts = PortStateForAts.Default, 
                TerminalState = terminalState
            };
            OnConnecting(this, args);
        }

        private void TimerCallback(Object o)
        {
            var duration = (DateTime.Now - _callStartTime).TotalSeconds;

            ConsoleColor originalCC = Console.ForegroundColor;

            ConsoleColor color = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), _colorNames[Number % _colorNames.Count()]);
            //Console.ForegroundColor = color;
            Trace.WriteLine(String.Format("In TimerCallback:{0} in terminal {1}. Call duration(s):{2:0}", DateTime.Now, Number, duration));
            //Console.ForegroundColor = originalCC;

            Random r = new Random();

            if (duration > r.Next(0, 60)) // random Call duration 
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                _timer.Dispose(); // Disable timer
                _timer = null;
                HangUp();
            }
        }

        private void HangUp()
        {
            lock (userOpSync)
            {
                var tmp = _callId;
                if ((Port != null) && (tmp != 0))
                {
                    var args = new HangUpEventArgs() { Id = tmp };
                    if (Program.Listners.HangUpFromTerminalToPortListner.ContainsKey(Port))
                    {
                        Program.Listners.HangUpFromTerminalToPortListner[Port](this, args);
                            // HangUp from Terminal->Port
                        if (args.Id == 0)
                        {
                            Trace.WriteLine(String.Format("Call #{0} was end at:{1}", _callId, DateTime.Now));
                        }
                    }
                }
                _callId = 0;
            }
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

    }
}
