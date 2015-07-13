using System;

namespace CP3Task1
{
    public struct CallInfo
    {
        public Port Caller;
        public Port Receiver;
        public DateTime StartCall;
        public DateTime? EndCall;
    }
}