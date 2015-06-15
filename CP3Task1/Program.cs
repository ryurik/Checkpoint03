using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP3Task1
{
    class Program
    {
        public static string AppPath;
        public static string[] PortData = { "Port", "port" };     // directory, extension
        public static string[] TerminalData = { "Port", "port" }; // directory, extension

        static void Main(string[] args)
        {
            AppPath = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);

            ATS ats = new ATS();
            Console.WriteLine("Loaded {0} port{1}", ats.Ports.Count, (ats.Ports.Count > 1) ? "s": "");
            Console.WriteLine("Of these are {0} : {1} ", PortState.UnPlugget, ats.Ports.Where(x=>x.PortState == PortState.UnPlugget).Count());
            //ats.CreateFirstPorts(100);

            ats.ActivatePortsFromContracts();
            Console.WriteLine("After activation {0} : {1} ", PortState.Plugget, ats.Ports.Where(x => x.PortState == PortState.Plugget).Count());


            Console.ReadKey();
        }
    }
}
