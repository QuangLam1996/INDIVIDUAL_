using PLCMonitorSystem.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PLCMonitorSystem
{
    public class Work
    {
        private eDO _output = default;
        public eDO Output { get => _output; set => _output = value; }

        public Work()
        {

        }

        public void ManualSw(eDO _output, bool _control)
        {
            this.Output = _output;

            if (_control)
            {
                UIManager.Device.WriteBit(Device.M, (int)Output, true);
                string log = string.Format("{0} : MANUAL WORK ON", Output);
                UIManager.theLog.CreateLog("", log);
                Thread.Sleep(500);

            }

            else
            {
                UIManager.Device.WriteBit(Device.M, (int)Output, false);
                string log = string.Format("{0} : MANUAL WORK OFF", Output);
                UIManager.theLog.CreateLog("", log);
                Thread.Sleep(500);

            }
        }

        public void Interlock()
        {

        }

        public void Delay()
        {

        }

        public void Initial()
        {

        }

        public void State()
        {

        }
    }
}
