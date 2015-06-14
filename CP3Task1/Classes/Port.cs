using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Checkpoint03.Classes;


namespace CP3Task1
{
    [Serializable]
    public class Port
    {
        public PhoneNumber PhoneNumber { get; set; }
        public PortState PortState { get; set; }

        public static void SaveToFile(PhoneNumber phoneNumber, PortState portState)
        {

            Port port = new Port()
            {
                PhoneNumber = phoneNumber,
                PortState = portState,
            };
            string fileName = Path.Combine(Program.AppPath, Program.PortData[0], Path.ChangeExtension(phoneNumber.Number.ToString(), Program.PortData[1]));
            Serializer.SaveToXml(fileName, port);

        }
        public static Port LoadFromFile(PhoneNumber phoneNumber)
        {
            string fileName = Path.Combine(Program.AppPath, Program.PortData[0], Path.ChangeExtension(phoneNumber.Number.ToString(), Program.PortData[1]));
            if (!File.Exists(fileName))
            {
                return null;
            }
            return Serializer.LoadFromXml<Port>(fileName);
        }
    }
}
