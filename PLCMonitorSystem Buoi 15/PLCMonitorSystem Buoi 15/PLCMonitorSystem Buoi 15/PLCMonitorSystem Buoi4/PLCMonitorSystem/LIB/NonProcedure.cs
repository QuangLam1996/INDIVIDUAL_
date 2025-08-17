using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows;

namespace PLCMonitorSystem.LIB
{
   public class NonProcedure
    {
        #region Field and Property
        // Thành phần số 1: Field hoặc Property (get,set) (Biến): 
        private SerialPort port;
        private string portName;// COM1,COM2,COM3.....
        private int dataBits;   // 7 hoặc 8
        private Parity parityBits;// None (Không dùng), Odd (Lẻ), Even(Chẵn)
        private StopBits stopBit;// One (1), Two (2)
        private int baudRate;// 9600 bps, 11200 bps, ,,,
        public string PortName
        {
            get => portName;
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    portName = "COM1";
                }
                else
                {
                    portName = value;
                }
            }  
        }
        public int DataBits { get => dataBits; set {

                if (value != 7 && value != 8) // Trường hợp khác cả 7 và khác cả 8
                {
                    dataBits = 7;
                }
                else // Trường hợp còn lại nó bằng 7 hoặc bằng 8
                    dataBits = value; 
            } }
        public Parity ParityBits { get => parityBits; set => parityBits = value; }
        public StopBits StopBit { get => stopBit; set => stopBit = value; }
        public int BaudRate { get => baudRate; set => baudRate = value; }

        #endregion
        // Thành phần thứ 2 của 1 class: Method ( Hàm)
        #region Method
        // Hàm dựng (Hàm tạo): Constructor
        // * Cùng tên với Class
        // ** Không có kiểu trả về
        // *** Tự động chạy khi class được khởi tạo
        public NonProcedure()
        {
            // Khởi tạo giá trị mặc định cho các field:
            this.portName = "COM1";
            this.dataBits = 8;
            this.stopBit = StopBits.One;
            this.parityBits = Parity.Odd;
            this.baudRate = 9600;
        }
        // Overload: Quá tải
        // Là một hàm mà cùng tên với hàm khác có trong class:
        // Khác kiểu trả về, hoặc khác parameter
        public NonProcedure(string _portName, int _dataBit, StopBits _stopBits, Parity _parity, int _baudRate)
        {
            this.portName = _portName;
            this.dataBits = _dataBit;
            this.stopBit = _stopBits;
            this.parityBits = _parity;
            this.baudRate = _baudRate;
        }
        public NonProcedure(string _portName, int _dataBit)
        {
            this.portName = _portName;
            this.dataBits = _dataBit;
            this.stopBit = StopBits.One;
            this.parityBits = Parity.Odd;
            this.baudRate = 9600;
        }
        public int Open()
        {
            int kq = -1;
            // Hàm chạy thành công trả về : 0
            // Hàm chạy không thành công : -1

            //B1: Kiểm trả port đã được khởi tạo chưa:
            if (this.port==null)
            {
                this.port = new SerialPort();
            }
            // B2: Truyền các thông số vào port :
            try
            {
                this.port.PortName = this.portName;
                this.port.DataBits = this.dataBits;
                this.port.StopBits = this.stopBit;
                this.port.BaudRate = this.baudRate;
                this.port.Parity = this.parityBits;
            }
            catch (Exception error)
            {
            }
            //B3:Kiểm tra đã mở cổng hay chưa:
            if (this.port.IsOpen==true)
            {
                kq = 0;
                return kq; // Thoát khỏi hàm, không chạy các dòng lệnh bên dưới
            }
            // B4: Mở cổng:
            try
            {
                this.port.Open();
            }
            catch (Exception error)
            {
            }
            //B5: Kiểm tra xem mở cổng hay chưa:
            if (this.port.IsOpen)
            {
                kq = 0;
            }
            return kq;
        }
        public int Close()
        {
            int kq = -1;
            //B1: Kiểm tra port đã khởi tạo hay chưa:
            if (this.port==null)
            {
                kq = 0;
                return kq;//Thoát khỏi hàm.
            }
            //B2: Kiểm tra port đã đóng hay chưa:
            if (this.port.IsOpen==false)
            {
                kq = 0;
                return kq;
            }
            // B3: Đóng cổng:
            try
            {
                this.port.Close();
                this.port = null;
                kq = 0;
            }
            catch (Exception error)
            {
            }
            return kq;
        }
        public int Send(byte[] arrSend)
        {
            int kq = -1;
            //B1: Kiểm tra port đã khởi tạo chưa:
            if (this.port==null)
            {
                return kq;// Thoát khỏi hàm.
            }
            // B2: Kiểm tra đã mở cổng hay chưa:
            if (this.port.IsOpen==false)
            {
                return kq;//Thoát khỏi hàm
            }
            //B3: Chuẩn bị dữ liệu để gửi:
            List<byte> lstSendData = new List<byte>();
            lstSendData.AddRange(arrSend);// thêm dữ liệu gửi
            // Thêm Complete code: Mã hoàn thành
            lstSendData.Add(0x0D);// CR 0x là hệ hexa.
            lstSendData.Add(0x0A);// LF

            // B4: Gửi:
            this.port.Write(lstSendData.ToArray(), 0, lstSendData.Count);
            kq = 0;
            return kq;
        }
        public byte[] Recieve ()
        {
            byte[] arrRcv = new byte[4096];
            //B1: Kiểm tra cổng đã khởi tạo chưa
            if (this.port==null)
            {
                return arrRcv;
            }
            // B2: Kiểm tra đã mở cổng chưa:
            if (this.port.IsOpen==false)
            {
                return arrRcv;
            }
            //B3: Nhận dữ liệu
            this.port.Read(arrRcv, 0, this.port.BytesToRead);

            return arrRcv;
        }
        #endregion

    }
   

}
