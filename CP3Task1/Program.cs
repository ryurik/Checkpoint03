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

        public static Listners Listners = new Listners();
        private static ATS ats;

        static void Main(string[] args)
        {
            AppPath = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
            ats = new ATS();

            Console.WriteLine("Loaded {0} port{1}", ats.Ports.Count, (ats.Ports.Count > 1) ? "s": "");
            Console.WriteLine("Of these are {0} : {1} ", PortStateForAts.UnPlugged.ToString(), ats.Ports.Where(x=>x.PortStateForAts == PortStateForAts.UnPlugged).Count());
            //ats.CreateFirstPorts(100);
            //ats.CreateFirstTerminals(100);

            ats.ActivatePortsFromContracts();
            ats.ConnectTerminals();

            ShowInfo();

            const ConsoleKey exitKey = ConsoleKey.Escape; // Esc - exit from Console
            ConsoleKeyInfo cki;
            do
            {
                cki = Console.ReadKey(true);
                switch (cki.Key)
                {
                    case ConsoleKey.D1:
                        Console.WriteLine("Try to call first free terminal:");

                        var p = ats.Ports.FirstOrDefault(x => x.PortStateForAts == (PortStateForAts.Plugged | PortStateForAts.Free));

                        if (p != null)
                        {
                            ats.CallToTerminal(p.PhoneNumber); // try to call first pluged and free number
                        }
                        //p = ats.Ports.Where(x => x.PortStateForAts == (PortStateForAts.Plugged | PortStateForAts.Free)).Skip(1).First();
                        //ats.CallToTerminal(p.PhoneNumber); // try to call to second pluged and free number

                        break;
                    case ConsoleKey.D2:
                        Console.WriteLine("Try to call:");
                        break;
                    case ConsoleKey.D0:
                        Console.WriteLine("Ports plugged:{0}", ats.Ports.Where(x => x.PortStateForAts == (PortStateForAts.Plugged | PortStateForAts.Free)).Count());
                        break;

                } 

            } while (cki.Key != exitKey);
        }

        public static void ShowInfo()
        {
            Console.WriteLine("After activation {0} : {1} ", PortStateForAts.Plugged, ats.Ports.Where(x => x.PortStateForAts == (PortStateForAts.Plugged | PortStateForAts.Free)).Count());
            Console.WriteLine("");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("Press 1 to call to first free terminal.");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("(results see in Output window in debug mode)");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Press 0 to check plugged ports");
            Console.ForegroundColor = ConsoleColor.White;
            
        }
    
    }

}
