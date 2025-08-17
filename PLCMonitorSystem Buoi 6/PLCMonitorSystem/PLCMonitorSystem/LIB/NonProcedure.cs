using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace PLCMonitorSystem.LIB
{
    public class NonProcedure
    {
        #region Field and Property
        // Thành phần số 1: Field or Property (Biến)
        private SerialPort port;
        private string portName; //COM1, COM2,...
        private int dataBit; //7bit, 8bit
        private Parity parityBits; //None, Odd, Even
        private StopBits stopBits; //1 <8bit>, 2 <7bit>
        private int baundRate; //9600, 19200,....

        public string PortName
        {
            get => portName;
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    portName = "COM 1";
                }
            }
        }

        public int DataBit
        {
            get => dataBit;
            set
            {
                if (value != 7 && value != 8)
                {
                    dataBit = 8;
                }
                else
                {
                    dataBit = value;
                }

            }
        }
        public StopBits StopBits1 { get => stopBits; set => stopBits = value; }
        public int BaundRate { get => baundRate; set => baundRate = value; }
        public Parity ParityBits { get => parityBits; set => parityBits = value; }
        #endregion

        #region Method
        // Thành phần thứ 2 class: Method / Hàm
        // Hàm dựng(Hàm tạo): Constructor
        // * Cùng tên với Class
        // * Ko có kiểu trả về
        // * Tự động Run khi được khởi tạo

        public  NonProcedure()
        {
            // Khởi tạo deafult cho các Field:
            this.portName = "COM 1";
            this.dataBit = 8;
            this.stopBits = StopBits.One;
            this.parityBits = Parity.Odd;
            this.baundRate = 9600;
            Console.WriteLine(portName);
        }

        // OverLoad: Quả tải
        // Là một hàm mà cùng tên với hàm khác trong class:
        // Khác kiểu trả về or parameter

        public NonProcedure(string _portName, int _dataBit, StopBits _stopBits, Parity _parity, int _baundRate)
        {
            this.portName = _portName;
            this.dataBit = _dataBit;
            this.stopBits = _stopBits;
            this.parityBits = _parity;
            this.baundRate = _baundRate;
        }
        #endregion
    }
    public class Test
    {
        NonProcedure test = new NonProcedure();
        NonProcedure _test = new NonProcedure("", 8, StopBits.One, Parity.Odd, 9600);

        public Test()
        {

        }

    }

}
