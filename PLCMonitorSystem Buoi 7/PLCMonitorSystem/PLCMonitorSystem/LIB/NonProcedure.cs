using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows.Documents;
using System.Windows;

namespace PLCMonitorSystem.LIB
{
    public class NonProcedure
    {
        #region Field and Property
        // Thành phần số 1: Field or Property (Biến)
        private SerialPort port=new SerialPort();
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
                    portName = "COM1";
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

        public NonProcedure()
        {
            // Khởi tạo deafult cho các Field:
            this.portName = "COM 2";
            this.dataBit = 8;
            this.stopBits = StopBits.One;
            this.parityBits = Parity.Odd;
            this.baundRate = 9600;
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

        #region Open Port Name
        public int Open()
        {
            int kq = -1;
            // Hàm chạy thành công trả về 0
            // Ngược lại
            if (this.port == null)
            {
                this.port = new SerialPort();
            }

            // B1: Truyền các thông số vào Port
            try
            {
                this.port.PortName = this.portName;
                this.port.DataBits = this.dataBit;
                this.port.StopBits = this.stopBits;
                this.port.BaudRate = this.baundRate;
                this.port.Parity = this.parityBits;
            }

            catch (Exception)
            {
                //MessageBox.Show("B1");
            }

            // B2: Kiểm tra mở Port
            if (this.port.IsOpen == true)
            {
                kq = 0;
                return kq; // Thoát khỏi hàm nếu đã mở Port
            }

            // B3: Mở cổng
            try
            {
                this.port.Open();
            }

            catch (Exception)
            {
                //MessageBox.Show("B3");
            }

            if (this.port.IsOpen==true)
            {
                kq = 0;
                //MessageBox.Show("OK");
            }
            return kq;
        }
        #endregion

        #region Close Port Name
        public int Close()
        {
            int kq = -1;
            // B1: Kiểm tra đã khởi tạo Port hay chưa
            if (this.port == null)
            {
                return kq; // Thoát khỏi hàm
            }
            // B2: Kiểm tra đã Close Port
            if (this.port.IsOpen == false)
            {
                kq = 0;
                return kq;
            }
            // B3: Đóng cổng
            try
            {
                this.port.Close();
                this.port = null;
                kq = 0;
            }
            catch (Exception)
            {
            }

            return kq;
        }
        #endregion

        #region Send Data
        public int Send(byte[] arrSend)
        {
            int kq = -1;
            // B1: Kiểm tra khởi tạo Port
            if(this.port == null)
            {
                return kq; // Thoát khỏi hàm
            }

            // B2: Kiểm tra Open Port
            if(this.port.IsOpen == false) 
            {
                return kq; // Thoát khỏi hàm
            }

            // B3: Gửi Data
            List<byte> lstSendData = new List<byte>();
            lstSendData.AddRange(arrSend); // Thêm dữ liệu gửi đi

            // Thêm mã hoàn thành 0D0A <CRLF> <đối với riêng PLC Mitsubishi>
            lstSendData.Add(0x0D); // CR <Mã Hex trong bảng ASCII>
            lstSendData.Add(0x0A); // LF <Mã Hex trong bảng ASCII>

            // B4: Gửi
            this.port.Write(lstSendData.ToArray(), 0, lstSendData.Count);
            kq = 0;

            return kq;
        }

        #endregion

        #region Recieve Data
        public byte[] Recieve()
        {
            byte[] arrRcv = new byte[512];
            // B1: Kiểm tra Port đã khởi tạo
            if (this.port == null) 
            {
                return arrRcv;
            }
            if (this.port.IsOpen==false)
            {
                return arrRcv;
            }

            // B2: Nhận Data
            try
            {
                //this.port.Read(arrRcv, 0, arrRcv.Length);
                this.port.Read(arrRcv, 0, this.port.BytesToRead);
            }
            catch (Exception)
            {
            }
            return arrRcv;
        }
        #endregion
    }
    public class Test
    {
        //NonProcedure test = new NonProcedure();
        //NonProcedure _test = new NonProcedure("", 8, StopBits.One, Parity.Odd, 9600);

        public Test()
        {
            //this.test.PortName = "";
            //this._test.PortName = "";

        }

    }

}
