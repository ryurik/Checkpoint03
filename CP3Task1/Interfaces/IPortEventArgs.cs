using CP3Task1.Enums;

namespace CP3Task1.Interfaces
{
    public interface IPortEventArgs
    {
         TerminalState TerminalState { get; }
         PortStateForAts PortStateForAts { get; }
         ConnectionPortResult ConnectionPortResult { get; }
    }
}