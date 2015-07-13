using CP3Task1.Enums;
using CP3Task1.Interfaces;

namespace CP3Task1.Classes.EventArgs
{
    public class PortEventArgs : System.EventArgs, IPortEventArgs
    {
        public TerminalState TerminalState { get; set; }
        public PortStateForAts PortStateForAts { get; set; }
        public ConnectionPortResult ConnectionPortResult { get; set; }
    }
}
