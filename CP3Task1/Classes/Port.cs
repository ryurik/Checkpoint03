using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using Checkpoint03.Classes;
using CP3Task1.Classes;


namespace CP3Task1
{
    public delegate bool ConectToPort(int portNumber);

    [Serializable]
    public class Port
    {
        public PhoneNumber PhoneNumber { get; set; }
        public PortState PortState { get; set; }
        public ATS Ats { get; set; }

        private EventHandler<EventArgs> _portEvent;

        public event EventHandler<EventArgs> PortEvent
        {
            add
            {
                if (_portEvent != null)
                {
                    if (!_portEvent.GetInvocationList().Contains(value))
                    {
                        _portEvent += value;
                    }
                }
                else
                {
                    _portEvent += value;
                }
            }
            remove { _portEvent -= value; }
        }

        protected virtual void OnPortEvent(Object sender, EventArgs args)
        {
            var temp = _portEvent;
            if (temp != null)
            {
                temp(sender, args);
            }
        }

        public void GenerateEvent()
        {
            OnPortEvent(this, null);
        }


        public static void SaveToFile(PhoneNumber phoneNumber, PortState portState)
        {

            Port port = new Port()
            {
                PhoneNumber = phoneNumber,
                PortState = portState,
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
    }
}
