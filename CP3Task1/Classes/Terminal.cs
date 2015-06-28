﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Checkpoint03.Classes;
using CP3Task1.Classes;
using CP3Task1.Enums;

namespace CP3Task1
{
    //public delegate Call
    public class Terminal
    {
        public int Number { get; set; }
        public TerminalState TerminalState { get; set; }
        public Port Port { get; set; }
        public ATS Ats { get; set; }

        private EventHandler<CallingEventArgs> _calling;
        private EventHandler<PortEventArgs> _connecting;

        public event EventHandler<CallingEventArgs> AfterCalling;

        public event EventHandler<CallingEventArgs> Calling
        {
            add { _calling += value; }
            remove { _calling -= value; }
        }

        public event EventHandler<PortEventArgs> Connecting
        {
            add { _connecting += value; }
            remove { _connecting -= value; }
        }

        protected virtual void OnCalling(object sender, CallingEventArgs args)
        {
            if (_calling != null)
            {
                _calling(sender, args);
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
            var temp = _connecting;
            if (temp != null)
            {
                temp(this, args);
            }
        }


        public ConnectionResult StartCall(PhoneNumber targetPhoneNumber)
        {
            if ((Port != null) && (Port.PortState == PortStateForAts.Free))
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
            else
            {
                return ConnectionResult.Default;
            }

        }

        public void SwitchOn()
        {
            // Try to register Termnal. Test Port status
            if (Port != null)
            {
                ConnectToPort();
            }
            if (Port != null)
            {
                ;
            }
        }

        private void ConnectToPort()
        {
            var args = new PortEventArgs() { ConnectionPortResult = ConnectionPortResult.Default, PortState = PortStateForAts.Default};
            OnConnecting(this, args);
            if (args.ConnectionPortResult == ConnectionPortResult.PortListning)
            {
                TerminalState = TerminalState.On;
            }
        }
        public void SwitchOff()
        {

        }
        #region WorkWithFiles
        public static void SaveToFile(PhoneNumber phoneNumber)
        {

            Terminal terminal = new Terminal()
            {
                Ats = null,
                Number = phoneNumber.Number,
                Port = null,
                TerminalState = TerminalState.Off,
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

        public void ConnectToPort(object sender, PortEventArgs args)
        {
            OnConnecting(sender, args);
        }
    }
}
