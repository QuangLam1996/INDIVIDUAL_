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
        private string _work;


        public eDO Output { get => _output; set => _output = value; }
        public string _Work { get => _work; set => _work = value; }

        public Work()
        {

        }

        public void WorkSW(eDO _output, bool _control)
        {
            this.Output = _output;

            if (_control)
            {
                _Work = "ON";
                UIManager.Device.WriteBit(Device.M, (int)Output, true);
                string log = string.Format("{0} : MANUAL WORK ON", Output);
                UIManager.theLog.CreateLog("", log);
                //Thread.Sleep(500);

            }

            else
            {
                _Work = "OFF";
                UIManager.Device.WriteBit(Device.M, (int)Output, false);
                string log = string.Format("{0} : MANUAL WORK OFF", Output);
                UIManager.theLog.CreateLog("", log);
                //Thread.Sleep(500);

            }
        }

        public int iTL_ON()
        {
            int kq = 0;
            return kq;
        }

        public int iTL_OFF()
        {
            int kq = 0;
            return kq;

        }

        public bool sS_ON()
        {
            if (_Work == "ON") 
            { 
                return true; 
            }
            return false;
        }

        public bool sS_OFF()
        {
            if (_Work == "OFF")
            {
                return true;
            }
            return false;
        }


        public void init_ON()
        {

        }

        public void init_OFF()
        {

        }


        public void state_ON() 
        { 
        
        }

        public void state_OFF() 
        { 
        
        }

    }
}
