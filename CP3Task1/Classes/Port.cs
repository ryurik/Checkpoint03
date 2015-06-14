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
            return Path.Combine(Program.AppPath, Program.PortData[0], Path.ChangeExtension(String.Format("Name{0}{1}", new String('0', 6 - portNumber.ToString().Length), portNumber), Program.PortData[1]));
        }
    }
}
