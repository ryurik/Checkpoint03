using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CP3Task1.Classes;

namespace CP3Task1
{
    class Program
    {
        public static string AppPath;
        public static string[] PortData = { "Port", "port" };     // directory, extension
        public static string[] TerminalData = { "Terminal", "terminal" }; // directory, extension

        static void Main(string[] args)
        {
            AppPath = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);

            ATS ats = new ATS();
            Console.WriteLine("Loaded {0} port{1}", ats.Ports.Count, (ats.Ports.Count > 1) ? "s": "");
            Console.WriteLine("Of these are {0} : {1} ", PortStateForAts.UnPlugged.ToString(), ats.Ports.Where(x=>x.PortStateForAts == PortStateForAts.UnPlugged).Count());
            //ats.CreateFirstPorts(100);
            //ats.CreateFirstTerminals(100);

            ats.ActivatePortsFromContracts();
            ats.ConnectTerminals();

            Console.WriteLine("After activation {0} : {1} ", PortStateForAts.Plugged, ats.Ports.Where(x => x.PortStateForAts == (PortStateForAts.Plugged | PortStateForAts.Free)).Count());

            var p = ats.Ports.FirstOrDefault(x => x.PortStateForAts == (PortStateForAts.Plugged | PortStateForAts.Free));

            if (p != null)
            {
                ats.CallToTerminal(p.PhoneNumber); // try to call first pluged and free number
            }

            Console.ReadKey();
        }
    }
}
