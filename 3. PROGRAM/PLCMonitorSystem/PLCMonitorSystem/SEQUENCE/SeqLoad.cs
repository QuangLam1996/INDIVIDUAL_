using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCMonitorSystem
{
    public class SeqLoad:SeqBase
    {
        public enum eInit
        {
            STEP00,
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
            STEP00,
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

        public SeqLoad(eFlag flag):base(flag)
        {
            
        }

        public void eInitStep(eInit step = eInit.STEP00)
        {
            //
            base.NextStep((int)step);
        }

        public void eAutoStep(eAuto step = eAuto.STEP00)
        {
            //
            base.NextStep((int)(step));
        }

        public override void InitStep()
        {

        }

        public override void AutoStep()
        {


        }
    }
}
