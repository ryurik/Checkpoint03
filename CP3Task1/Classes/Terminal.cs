﻿using System;
using System.Collections.Generic;
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
        private String[] _colorNames = ConsoleColor.GetNames(typeof(ConsoleColor));

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
            throw new NotImplementedException();
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
            Console.WriteLine("I'am a terminal #{0} and I'm get Call from ATS. Time:{1}", Number, DateTime.Now);
            _callStartTime = DateTime.Now;
             _timer = new Timer(TimerCallback, null, 0, 1000);
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
                PortState = PortStateForAts.Default, 
                TerminalState = terminalState
            };
            OnConnecting(this, args);
        }

        private void TimerCallback(Object o)
        {
            var duration = (DateTime.Now - _callStartTime).TotalSeconds;

            ConsoleColor originalCC = Console.ForegroundColor;

            ConsoleColor color = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), _colorNames[Number % _colorNames.Count()]);
            Console.ForegroundColor = color;
            Console.WriteLine("In TimerCallback:{0} in terminal {1}. Call duration(s):{2:0}", DateTime.Now, Number, duration);
            Console.ForegroundColor = originalCC;

            if (duration > 30)
            {
                HangUp();
            }
        }

        private void HangUp()
        {
            throw new NotImplementedException();
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
