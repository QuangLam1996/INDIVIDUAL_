using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace PLCMonitorSystem.LIB
{
    public enum DevCode
    {
        SM = 0X91,
        SD = 0XA9,
        X = 0X9C,
        Y = 0X9D,
        M = 0X90,
        L = 0X92,
        D = 0XA8,
        Z = 0XCC,
        ZR = 0XB0
    }
    public class MC_Format5
    {
        #region Field and Property
        // Thành phần số 1: Field or Property (Biến)
        private SerialPort port = new SerialPort();
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
                portName = value;
            }
        }
        // Cách viết khác của Property
        //public string PortName
        //{
        //    get
        //    {
        //        return portName;
        //    }
        //    set
        //    {
        //        portName= value;
        //    }
        //}
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
        public StopBits StopBits { get => stopBits; set => stopBits = value; }
        public int BaundRate { get => baundRate; set => baundRate = value; }
        public Parity ParityBits { get => parityBits; set => parityBits = value; }

        // Các Field and Property của MC Protocol Format5
        private int stationNo;
        private int networkNo;
        private int pcNo;
        private int rqstationNo;
        private int selftstationNo;

        public int StationNo { get => stationNo; set => stationNo = value; }
        public int NetworkNo { get => networkNo; set => networkNo = value; }
        public int PcNo { get => pcNo; set => pcNo = value; }
        public int RqstationNo { get => rqstationNo; set => rqstationNo = value; }
        public int SelftstationNo { get => selftstationNo; set => selftstationNo = value; }
        #endregion

        #region Method
        // Thành phần thứ 2 class: Method / Hàm
        // Hàm dựng(Hàm tạo): Constructor
        // * Cùng tên với Class
        // * Ko có kiểu trả về
        // * Tự động Run khi được khởi tạo

        public MC_Format5()
        {
            // Khởi tạo cho các Field qua property:
            this.PortName = null;
            this.DataBit = 8;
            this.StopBits = StopBits.One;
            this.ParityBits = Parity.Odd;
            this.BaundRate = 9600;
            // Khởi tạo deafult cho MC_Format5
            this.StationNo = 0x00;
            this.NetworkNo = 0x00;
            this.PcNo = 0xFF;
            this.RqstationNo = 0x00;
            this.SelftstationNo = 0x00;
            // Khởi tạo cho SerialPort
            this.port.PortName = PortName;
            this.port.DataBits = DataBit;
            this.port.StopBits = StopBits;
            this.port.Parity = ParityBits;
            this.port.BaudRate = BaundRate;
        }
        // OverLoad: Quả tải
        // Là một hàm mà cùng tên với hàm khác trong class:
        // Khác kiểu trả về or parameter
        public MC_Format5(string _portName, int _dataBit, StopBits _stopBits, Parity _parity, int _baundRate)
        {
            this.PortName = _portName;
            this.DataBit = _dataBit;
            this.StopBits = _stopBits;
            this.ParityBits = _parity;
            this.BaundRate = _baundRate;

            this.port.PortName = PortName;
            this.port.DataBits = DataBit;
            this.port.StopBits = StopBits;
            this.port.Parity = ParityBits;
            this.port.BaudRate = BaundRate;

            this.stationNo = 0x00;
            this.networkNo = 0x00;
            this.pcNo = 0xFF;
            this.rqstationNo = 0x00;
            this.selftstationNo = 0x00;
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
                //this.port = new SerialPort();
            }

            // B1: Truyền các thông số vào Port
            try
            {

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

            if (this.port.IsOpen == true)
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
        public short ReadWord(DevCode _devCode, int _devNumber)
        {
            short kq = -1;
            if (this.port == null)
            {
                return kq;
            }
            if (this.port.IsOpen == false)
            {
                return kq;
            }
            // Chuẩn bị data 
            List<byte> lstSendData = new List<byte>();
            // 1.1 Control code: 0-1 
            lstSendData.Add(0x10); // DLE
            lstSendData.Add(0x02); // STX

            // 1.2 Number of data bytes: 2-3
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);

            // 1.3 Frame ID No: 4
            lstSendData.Add(0xF8);

            // 1.4 Acess Route: 5-11
            // 1.4.1 Station No
            lstSendData.Add((byte)this.StationNo);
            // 1.4.2 Network No
            lstSendData.Add((byte)this.NetworkNo);
            // 1.4.3 PC No
            lstSendData.Add((byte)this.PcNo);
            // 1.4.4 I/O No
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            // 1.4.5 Request Station
            lstSendData.Add((byte)this.RqstationNo);
            // 1.4.6 Seft Station
            lstSendData.Add((byte)this.SelftstationNo);

            // 1.5 Request Data
            // 1.5.1 Comand: 0401
            lstSendData.Add(0x01);
            lstSendData.Add(0x04);
            // 1.5.2 Sub Comand: 0000
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);
            // 1.5.3 Head Device Number
            // Tách số địa chỉ thành các byte
            byte[] __devNumber = BitConverter.GetBytes(_devNumber);
            // Thêm 3 byte theo Frame
            lstSendData.Add(__devNumber[0]);
            lstSendData.Add(__devNumber[1]);
            lstSendData.Add(__devNumber[2]);
            // 1.5.4 Device Code
            // Tách địa chỉ thành byte X,Y,D,...
            lstSendData.Add((byte)_devCode);
            // 1.5.5 Number of device Point
            int devNumPoint = 1;
            byte[] _devNumPoint = BitConverter.GetBytes(devNumPoint);
            // Thêm 2 byte theo Frame
            lstSendData.Add(_devNumPoint[0]);
            lstSendData.Add(_devNumPoint[1]);

            // 1.6 Tính lại Number of Data Byte
            int numByte = lstSendData.Count - 4;
            byte[] arrNumByte = BitConverter.GetBytes(numByte);
            lstSendData[2] = arrNumByte[0];
            lstSendData[3] = arrNumByte[1];

            // 1.7 Tính Sumcheck Code
            int sum = 0;
            for (int i = 2; i<lstSendData.Count; i++)
            {
                sum+= lstSendData[i];
            }
            // Chuyển sum thành kí tự
            string strSum = sum.ToString("x");
            // Thêm kí tự nếu ko đủ 2 byte
            strSum = strSum.PadLeft(2, '0');
            // Cắt 2 byte cuối nếu hơn 2 byte
            strSum = strSum.Substring(strSum.Length - 2, 2);
            // Chuyển về ASCII
            byte[] arrSum = ASCIIEncoding.ASCII.GetBytes(strSum);

            // 1.8 Control Code
            lstSendData.Add(0x10);
            lstSendData.Add(0x03);

            // 1.9 Sumcheck Code
            lstSendData.Add(arrSum[0]);
            lstSendData.Add(arrSum[1]);

            // 2.1 Send Data
            this.port.DiscardInBuffer();
            this.port.Write(lstSendData.ToArray(), 0, lstSendData.Count);

            return kq;
        }
    }
}
