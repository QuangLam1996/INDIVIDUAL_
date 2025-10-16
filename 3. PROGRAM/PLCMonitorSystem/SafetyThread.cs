using PLCMonitorSystem.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PLCMonitorSystem
{
    public class SafetyThread
    {
        Thread _thread;
        bool _isRunning = true;

        public void Init()
        {
            UIManager.theLog.CreateLog("Safety Thread", "Start.");

            _thread = new Thread(Monitor);
            _thread.IsBackground = true;
            _thread.Start();
        }

        ~SafetyThread()
        {
            _isRunning = false;
            UIManager.theLog.CreateLog("Safety Thread", "Stop.");
        }

        public void Monitor()
        {
            while (_isRunning)
            {

            }
        }
    }
}
