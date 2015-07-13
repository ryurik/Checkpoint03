using CP3Task1.Interfaces;

namespace CP3Task1.Classes.EventArgs
{
    public class HangUpEventArgs : IHangUpArg
    {
        public int Id{ get; set; }
    }
}