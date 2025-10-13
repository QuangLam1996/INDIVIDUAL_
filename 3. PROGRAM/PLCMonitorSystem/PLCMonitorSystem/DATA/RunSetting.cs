using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCMonitorSystem
{
    public class RunSetting
    {
        // Field & Property

        private string _wordName;
        private string _bitName;
        private int _cycle;

        public string WordName { get => _wordName; set => _wordName = value; }
        public string BitName { get => _bitName; set => _bitName = value; }
        public int Cycle { get => _cycle; set => _cycle = value; }

        // Method
        public RunSetting()
        {
            this.WordName = "D100";
            this.BitName = "M100";
            this.Cycle = 500;
        }
    }
}
