using System;

namespace SignalR_Client
{
    internal class UserMessage
    {
        public DateTime Time { get; set; }
        public string User { get; set; }
        public string Message { get; set; }
    }
}