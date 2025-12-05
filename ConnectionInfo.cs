using System;

namespace ZaldrionNetMonitorGUI
{
    public class ConnectionInfo
    {
        public string State { get; set; } = string.Empty;
        public string ProcessName { get; set; } = string.Empty;
        public int Pid { get; set; }
        public string LocalAddress { get; set; } = string.Empty;
        public int LocalPort { get; set; }
        public string RemoteAddress { get; set; } = string.Empty;
        public int RemotePort { get; set; }
        public string Protocol { get; set; } = string.Empty;
        public string LastSeen { get; set; } = string.Empty;
    }
}
