using CP3Task1.Enums;

namespace CP3Task1.Classes.EventArgs
{
    public class PortEventArgs : System.EventArgs
    {
        public TerminalState TerminalState { get; set; }
        public PortStateForAts PortState { get; set; }
        public ConnectionPortResult ConnectionPortResult { get; set; }
    }
}
