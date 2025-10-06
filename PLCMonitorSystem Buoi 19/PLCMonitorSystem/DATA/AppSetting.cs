using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCMonitorSystem.DATA
{
    public class AppSetting
    {
        // Field & Property
        private RunSetting _runSetting;
        private McSetting _mcSetting;
        private SockSetting _sockSetting;

        public RunSetting RunSetting { get => _runSetting; set => _runSetting = value; }
        public McSetting McSetting { get => _mcSetting; set => _mcSetting = value; }
        public SockSetting SockSetting { get => _sockSetting; set => _sockSetting = value; }


        // Method
        public AppSetting()
        {
            this.McSetting = new McSetting();
            this.SockSetting = new SockSetting();
            this.RunSetting = new RunSetting();
        }
    }
}
