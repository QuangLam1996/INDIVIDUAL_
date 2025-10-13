using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCMonitorSystem
{
    public class SockSetting
    {
        private string _ipAddr;
        private int _port;

        public string IpAddr { get => _ipAddr; set => _ipAddr = value; }
        public int Port { get => _port; set => _port = value; }

        public SockSetting() 
        {
            this.IpAddr = "127.0.0.1";
            this.Port = 6000;
        }

    }
}
