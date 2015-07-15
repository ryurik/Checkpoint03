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

            ats.CreateFirstPorts(100);
            ats.CreateFirstTerminals(100);
            ats.LoadData();

            //ats.ActivatePortsFromContracts();
            //ats.ActivateTerminalsFromContracts();

            ats.Help();

            const ConsoleKey exitKey = ConsoleKey.Escape; // Esc - exit from Console
            ConsoleKeyInfo cki;
            do
            {
                cki = Console.ReadKey(true);
                switch (cki.Key)
                {
                    case ConsoleKey.F1:
                        ats.Help();
                        break;
                    case ConsoleKey.D1:
                        ats.CallToRandomTerminal();
                        break;
                    case ConsoleKey.D8:
                        ats.LoadData();
                        break;
                    case ConsoleKey.D9:
                        ats.ShowStatistic();
                        break;
                    case ConsoleKey.D0:
                        ats.ShowAtsStates();
                        break;

                } 

            } while (cki.Key != exitKey);
        }
    }

}
