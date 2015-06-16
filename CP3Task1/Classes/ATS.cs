using System;
using System.IO;
using System.Linq;
using Checkpoint03.Classes;

namespace CP3Task1.Classes
{
    [Serializable]
    public class ATS
    {
        private PortSet _ports = new PortSet();
        private TerminalSet _terminals = new TerminalSet();
        
        public PortSet Ports
        {
            get { return _ports; }
            set { _ports = value; }
        }

        public TerminalSet Terminals
        {
            get { return _terminals; }
            set { _terminals = value; }
        }

        public ATS()
        {
            LoadData(); // LoadMainData - Starting ATS
        }

        #region WorkWithPorts

        public void ActivatePortsFromContracts()
        {
            Random r = new Random();
            for (int i = 0; i < Ports.Count/2; i ++)
            {
                var tmp = _ports.Where(x => x.PortState == PortState.UnPlugged).ToArray();
                tmp[r.Next(1, tmp.Length)].PortState = PortState.Plugged;
            }
        }

        public void CreateFirstPorts(int count = 50)
        {
            for (int i = 1; i <= count; i++)
            {
                Port port = new Port()
                {
                    PhoneNumber = new PhoneNumber()
                    {
                        Name = String.Format("{0}{1}", new String('0', 6 - i.ToString().Length), i),
                        Number = i
                    },
                    PortState = PortState.UnPlugged
                };
                Serializer.SaveToXml(Path.Combine(Program.AppPath, Program.PortData[0], Path.ChangeExtension(port.PhoneNumber.Name, Program.PortData[1])), port);
            }
        }

        #endregion

        #region WorkWithTerminals

        public void ActivateTerminalsFromContracts()
        {
            Random r = new Random();
            for (int i = 0; i < Ports.Count / 2; i++)
            {
                var tmp = _ports.Where(x => x.PortState == PortState.UnPlugged).ToArray();
                tmp[r.Next(1, tmp.Length)].PortState = PortState.Plugged;
            }
        }

        public void CreateFirstTerminals(int count = 50)
        {
            for (int i = 1; i <= count; i++)
            {
                Terminal terminal = new Terminal()
                {
                    Ats = this,
                    Number = i,
                    Port = null,
                    TerminalState = TerminalState.Off
                };
                Serializer.SaveToXml(Path.Combine(Program.AppPath, Program.TerminalData[0], Path.ChangeExtension(terminal.Number.ToString(), Program.PortData[1])), terminal);
            }
        }

        public void ConnectTerminals()
        {
            foreach (var t in Terminals)
            {
                t.Port = _ports.FirstOrDefault(x => x.PhoneNumber.Number == t.Number);
                
                if ((t.Port != null) && (t.Port.PortState == PortState.Plugged))
                {
                    t.ConnectToPort(this, null);
                }
            }
        }

        #endregion

        
        private void LoadData()
        {
            var dir = new DirectoryInfo(Path.GetDirectoryName(Port.GenerateFileName(0)));
            if (!Directory.Exists(dir.FullName))
                return;
            // loading ports
            foreach (Port port in dir.GetFiles(searchPattern: String.Format("*.{0}", Program.PortData[1])).Select(f => Port.LoadFromFile(Int32.Parse(Path.GetFileNameWithoutExtension(f.Name)))))
            {
                port.Ats = this;
                _ports.Add(port); 
            }
            dir = new DirectoryInfo(Path.GetDirectoryName(Terminal.GenerateFileName(0)));
            if (!Directory.Exists(dir.FullName))
                return;
            // loading terminals
            foreach (Terminal terminal in dir.GetFiles(searchPattern: String.Format("*.{0}", Program.TerminalData[1])).Select(f => Terminal.LoadFromFile(Int32.Parse(Path.GetFileNameWithoutExtension(f.Name)))))
            {
                terminal.Ats = this;
                _terminals.Add(terminal);
            }
        }
    }
}
