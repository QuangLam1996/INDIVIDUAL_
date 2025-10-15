using PLCMonitorSystem.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PLCMonitorSystem
{
    public class SeqThread
    {
        Thread _thread;
        bool _isRunning = true;

        public void Init()
        {
            UIManager.theLog.CreateLog("SeqThread", "Start.");

            _thread = new Thread(Process);
            _thread.IsBackground = true;
            _thread.Start();
        }
        ~SeqThread()
        {
            _isRunning = false;
            UIManager.theLog.CreateLog("SeqThread", "Stop.");

        }

        public void Process()
        {
            SeqLoad seqLoad = new SeqLoad(eFlag.LOAD);
            while (_isRunning)
            {
                Thread.Sleep(10);
                seqLoad.InitStep();
                seqLoad.AutoStep();
            }
        }
    }
}
