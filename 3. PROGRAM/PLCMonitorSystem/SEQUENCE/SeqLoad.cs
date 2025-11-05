using PLCMonitorSystem.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCMonitorSystem
{
    public class SeqLoad : SeqBase
    {
        public enum eControl
        {
            START,
            STOP,
            RESET,
            INIT,
        }
        public enum eInit
        {
            STEP00 = -1,
            STEP01,
            STEP02,
            STEP03,
            STEP04,
            STEP05,
        }

        public enum eInitLink
        {
            BIT00,
            BIT01,
            BIT02,
            BIT03,
            BIT04,
        }

        public enum eAuto
        {
            STEP00 = -1,
            STEP01,
            STEP02,
            STEP03,
            STEP04,
            STEP05,

        }

        public enum eAutoLink
        {
            BIT00,
            BIT01,
            BIT02,
            BIT03,
            BIT04,

        }

        public SeqLoad(eFlag flag) : base(flag)
        {
            
        }

        public new void Test()
        {
            base.Test();
        }

        public void eInitStep(eInit step = eInit.STEP00)
        {
            //
            base.InitStep((int)step);
        }

        public void eAutoStep(eAuto step = eAuto.STEP00)
        {
            //
            base.AutoStep((int)(step));
        }

        public override void InitStep()
        {
            switch ((eInit)_stepInit)
            {
                case eInit.STEP00:
                    //UIManager.theLog.CreateLog("SeqLoad.Init", "-1");
                    eInitStep();
                    break;
                case eInit.STEP01:
                    //UIManager.theLog.CreateLog("SeqLoad.Init", "0");
                    eInitStep();

                    break;
                case eInit.STEP02:
                    //UIManager.theLog.CreateLog("SeqLoad.Init", "1");
                    eInitStep();

                    break;
                case eInit.STEP03:
                    //UIManager.theLog.CreateLog("SeqLoad.Init", "2");
                    eInitStep();

                    break;
                case eInit.STEP04:
                    //UIManager.theLog.CreateLog("SeqLoad.Init", "3");
                    eInitStep();

                    break;
                case eInit.STEP05:
                    //UIManager.theLog.CreateLog("SeqLoad.Init", "4");
                    eInitStep();

                    break;
            }
        }

        public override void AutoStep()
        {
            Enum eLog = (eAuto)_stepAuto;
            string log = eLog.ToString();

            switch ((eAuto)_stepAuto)
            {
                case eAuto.STEP00:
                    UIManager.theLog.CreateLog("SeqLoad.Auto", log);
                    eAutoStep();

                    break;
                case eAuto.STEP01:
                    UIManager.theLog.CreateLog("SeqLoad.Auto", log);
                    UIManager.theWork.WorkSW(eDO.Q00, false);
                    eAutoStep();

                    break;
                case eAuto.STEP02:
                    UIManager.theLog.CreateLog("SeqLoad.Auto", log);
                    UIManager.theWork.WorkSW(eDO.Q01, false);
                    eAutoStep();

                    break;
                case eAuto.STEP03:
                    UIManager.theLog.CreateLog("SeqLoad.Auto", log);
                    UIManager.theWork.WorkSW(eDO.Q02, false);
                    eAutoStep();

                    break;
                case eAuto.STEP04:
                    UIManager.theLog.CreateLog("SeqLoad.Auto", log);
                    UIManager.theWork.WorkSW(eDO.Q03, false);
                    eAutoStep();

                    break;
                case eAuto.STEP05:
                    UIManager.theLog.CreateLog("SeqLoad.Auto", log);
                    UIManager.theWork.WorkSW(eDO.Q04, false);
                    eAutoStep(eAuto.STEP01);

                    break;
            }

        }
    }
}
