using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCMonitorSystem
{
    public class TimeSetting
    {

        public struct Delay
        {
            private string[] timeOn;        // Time On/Off
            private string[] timeOff;        // Time On/Off

            public string[] TimeOn { get => timeOn; set => timeOn = value; }
            public string[] TimeOff { get => timeOff; set => timeOff = value; }


            //public int[] timeOn;            // Delay On
            //public int[] timeOff;           // Delay Off

            public Delay(int _cylinOn, int _cylinOff)
            {
                timeOn = new string[_cylinOn];
                timeOff = new string[_cylinOff];
            }

        }

        public Delay[] _arrDelay = new Delay[Enum.GetValues(typeof(eDelay)).Length];
        
    }
}
