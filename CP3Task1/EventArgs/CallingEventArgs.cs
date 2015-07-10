namespace CP3Task1.Classes.EventArgs
{
    public class CallingEventArgs : System.EventArgs
    {
        public PhoneNumber Tagget { get; set; }
        public ConnectionResult ConnectionResult { get; set; }
    }
}
