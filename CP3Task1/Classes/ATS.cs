using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Checkpoint03.Classes;

namespace CP3Task1
{
    [Serializable]
    public class ATS
    {
        private PortSet _ports = new PortSet();

        public ATS()
        {
            LoadData();
        }

        private void LoadData()
        {
            var dir = new DirectoryInfo(Path.GetDirectoryName(Port.GenerateFileName(0)));

            foreach (FileInfo f in dir.GetFiles(searchPattern: String.Format("*.{0}", Program.PortData[1])))
            {
                Port port = Port.LoadFromFile(Int32.Parse(Path.GetFileNameWithoutExtension(f.Name)));
                _ports.Add(port); // сохраняем все конфеты в списке, чтобы потом было удобнее работать
            }
        }

        public void CreateFirstPorts(int count = 50)
        {
            for (int i = 0; i < count; i++)
            {
                Port port = new Port()
                {
                    PhoneNumber = new PhoneNumber()
                    {
                        Name = String.Format("Name{0}{1}", new String('0', 6 - i.ToString().Length), i),
                        Number = i
                    },
                    PortState = PortState.UnPlugget
                };
                Serializer.SaveToXml(Path.Combine(Program.AppPath, Program.PortData[0], Path.ChangeExtension(port.PhoneNumber.Name, Program.PortData[1])), port);
            }
        }
    }
}
