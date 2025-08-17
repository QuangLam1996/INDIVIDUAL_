using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Security.Cryptography;
using System.Net;

namespace MTECH
{
   
   public class MCF5
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
        public int DataBits
        {
            get => dataBits; set
            {

                if (value != 7 && value != 8) // Trường hợp khác cả 7 và khác cả 8
                {
                    dataBits = 7;
                }
                else // Trường hợp còn lại nó bằng 7 hoặc bằng 8
                    dataBits = value;
            }
        }
        public Parity ParityBits { get => parityBits; set => parityBits = value; }
        public StopBits StopBit { get => stopBit; set => stopBit = value; }
        public int BaudRate { get => baudRate; set => baudRate = value; }
        public int StationNo { get => stationNo; set => stationNo = value; }
        public int NetWorkNo { get => netWorkNo; set => netWorkNo = value; }
        public int PcNo { get => pcNo; set => pcNo = value; }
        public int ReqStationNo { get => reqStationNo; set => reqStationNo = value; }
        public int SeftStationNo { get => seftStationNo; set => seftStationNo = value; }

        // Các Field and Property của giao thức MC
        private int stationNo;
        private int netWorkNo;
        private int pcNo;
        private int reqStationNo;
        private int seftStationNo;

        #endregion

        #region Method: Các hàm
        //* Trùng tên class:
        //** Không có kiểu trả về
        // *** tự động chạy khi class được khởi tạo
        public MCF5()
        {
            // Cài thông số mặc định cho các property
            this.port.PortName = "COM1";
            this.port.DataBits = 8;
            this.port.StopBits = StopBits.One;
            this.port.BaudRate = 9600;
            this.port.Parity = Parity.Odd;
            // Cài thông số mặc định cho các property của MC:
            this.stationNo = 0;
            this.netWorkNo = 0;
            this.pcNo = 0xFF;
            this.reqStationNo = 0;
            this.seftStationNo = 0;
        }
        public MCF5(string _portName, int _dataBit, StopBits _stopBit, int _baudRate, Parity _parity)
        {
            // Cài thông số mặc định cho các property
            this.PortName = _portName;
            this.DataBits = _dataBit;
            this.StopBit = _stopBit;
            this.BaudRate = _baudRate;
            this.ParityBits = _parity;
            // Cài thông số mặc định cho các property của MC:
            this.stationNo = 0;
            this.netWorkNo = 0;
            this.pcNo = 0xFF;
            this.reqStationNo = 0;
            this.seftStationNo = 0;
        }
        public int Open()
        {
            int kq = -1;
            // Hàm chạy thành công trả về : 0
            // Hàm chạy không thành công : -1

            //B1: Kiểm trả port đã được khởi tạo chưa:
            if (this.port == null)
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
            if (this.port.IsOpen == true)
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
            if (this.port == null)
            {
                kq = 0;
                return kq;//Thoát khỏi hàm.
            }
            //B2: Kiểm tra port đã đóng hay chưa:
            if (this.port.IsOpen == false)
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

        // Hàm đọc giá trị của một thanh ghi 16 bit
        public short ReadWord(DevCode _devCode, int _devNumber)
        {
            short kq = 0;
            //B1: Kiểm tra port đã khởi tạo chưa:
            if (this.port == null)
            {
                return kq;
            }
            //B2: Kiểm tra port đã mở chưa
            if (this.port.IsOpen == false)
            {
                return kq;
            }
            //B3: Chuẩn bị dữ liệu gửi xuống:
            List<byte> lstSendData = new List<byte>();
            //3.1. Control code: 0-1
            lstSendData.Add(0x10);// DLE
            lstSendData.Add(0x02);// STX
            //3.2. Number of databyte : 2-3
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);
            // 3.3. Frame ID No: 4
            lstSendData.Add(0xF8);
            // 3.4. Access Route : 7 byte: 5-11
            // 3.4.1. Station No:
            lstSendData.Add((byte)this.stationNo);
            // 3.4.2. NetWork No:
            lstSendData.Add((byte)this.netWorkNo);
            // 3.4.3. PC No:
            lstSendData.Add((byte)this.pcNo);
            // 3.4.4. IO No:
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            // 3.4.5. Request Station:
            lstSendData.Add((byte)this.reqStationNo);
            //3.4.6. Seft Station :
            lstSendData.Add((byte)this.seftStationNo);

            //3.5. Request Data
            //3.5.1. Command: 0401
            lstSendData.Add(0x01);
            lstSendData.Add(0x04);
            //3.5.2. SubCommand: 0000
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);
            //3.5.3. Thanh ghi số bao nhiêu:
            // Tách thành các byte
            byte[] arrDevNumber = BitConverter.GetBytes(_devNumber);
            // Thêm 3 byte vào danh sách gửi đi
            lstSendData.Add(arrDevNumber[0]);
            lstSendData.Add(arrDevNumber[1]);
            lstSendData.Add(arrDevNumber[2]);
            // 3.5.4. Device Code:
            lstSendData.Add((byte)_devCode);

            //3.5.5. Number of Device Point: Số lượng thanh ghi liên tiếp
            int devNumPoint = 1;
            byte[] arrDevNumPoint = BitConverter.GetBytes(devNumPoint);
            lstSendData.Add(arrDevNumPoint[0]);
            lstSendData.Add(arrDevNumPoint[1]);

            //3.6. Sửa lại number of Data byte:
            int numByte = lstSendData.Count - 4;
            byte[] arrNumByte = BitConverter.GetBytes(numByte);
            lstSendData[2] = arrNumByte[0];
            lstSendData[3] = arrNumByte[1];

            //3.7 Tính sumcheck code:
            // 3.7.1. Tính tổng từ byte 2 đến hết
            int sum = 0;
            for (int i = 2; i < lstSendData.Count; i++)
            {
                sum += lstSendData[i];
            }
            //3.7.2. Chuyển tổng từ số nguyên thành kí tự:
            string strSum = sum.ToString("X");
            //3.7.2. Thêm kí tự 0 vào nếu chuỗi không đủ 2 kí tự:
            strSum = strSum.PadLeft(2, '0');
            // Hàm padleft là hàm điền kí tự yêu cầu sang bên trái
            // cho đủ số lượng kí tự yêu cầu
            // Ví dụ ban đầu strSum = "1", thì sau chạy lệnh strSum = "01";
            // Trong trường hợp strSum nhiều hơn 2 kí tự thì cần cắt ra 2 kí tự bên phải
            strSum = strSum.Substring(strSum.Length - 2, 2);
            //3.7.3. Chuyển chuỗi string về dạng ASCII:
            byte[] arrSum = ASCIIEncoding.ASCII.GetBytes(strSum);

            // 3.8. Control Code: 
            lstSendData.Add(0x10); //DLE
            lstSendData.Add(0x03); // EOT

            //3.9. Thêm sumcheck code ở mục 3.7 vào:
            lstSendData.Add(arrSum[0]);
            lstSendData.Add(arrSum[1]);

            // B4: Gửi dữ liệu đi:
            this.port.DiscardInBuffer();
            this.port.Write(lstSendData.ToArray(), 0, lstSendData.Count);

            // B5: Nhận dữ liệu về từ PLC:
            int timeOut = 1000;//ms = 1s
            // Đợi đến bao giờ đủ số lượng byte mong muốn: ít nhất 20 byte
            int expectBytes = 20;
            while (timeOut > 0)
            {
                if (this.port.BytesToRead >= expectBytes)
                {
                    break;
                }
                timeOut = timeOut - 10;// Mỗi lần trừ đi 10ms
                Thread.Sleep(10);// Dừng trong 10ms
            }
            // Vị trí này là thoát khỏi while ************
            if (timeOut <= 0)
            {
                return kq;
            }
            // Nhận dữ liệu:
            byte[] arrRcv = new byte[1024];
            this.port.Read(arrRcv, 0, this.port.BytesToRead);
            List<byte> lstRcv = new List<byte>();
            lstRcv.AddRange(arrRcv);

            // Bước 6: Phân tích dữ liệu nhận về từ PLC:
            // 6.1. Control Code:
            if (lstRcv[0] != 0x10 || lstRcv[1] != 0x02)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);// Remove từ phần tử số 0 và remove đi 2 phần tử
            //6.2. Kiểm tra xem number of databyte là bao nhiêu:
            short numberOfDataByte = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (numberOfDataByte < 12)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.3. Frame ID No:
            if (lstRcv[0] != 0xF8)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.4. Access Route:
            //6.4.1 Station No
            if (lstRcv[0] != (byte)this.stationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.2. NetWorkNo:
            if (lstRcv[0] != (byte)this.netWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.3. PCNo:
            if (lstRcv[0] != (byte)this.pcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.4. Request Destination Module IO No:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.4.5. Request Station No:
            if (lstRcv[0] != (byte)this.reqStationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.6. Seft Station No:
            if (lstRcv[0] != (byte)this.seftStationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            // 6.5. Response ID Code:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0xFF)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            // 6.6. Normal Completition:
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            //6.7. Response Data:
            kq = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);

            return kq;
        }
        // Hàm đọc giá trị của một thanh ghi 32 bit
        public int ReadDoubleWord(DevCode _devCode, int _devNumber)
        {
            int kq = 0;
            //B1: Kiểm tra port đã khởi tạo chưa:
            if (this.port == null)
            {
                return kq;
            }
            //B2: Kiểm tra port đã mở chưa
            if (this.port.IsOpen == false)
            {
                return kq;
            }
            //B3: Chuẩn bị dữ liệu gửi xuống:
            List<byte> lstSendData = new List<byte>();
            //3.1. Control code: 0-1
            lstSendData.Add(0x10);// DLE
            lstSendData.Add(0x02);// STX
            //3.2. Number of databyte : 2-3
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);
            // 3.3. Frame ID No: 4
            lstSendData.Add(0xF8);
            // 3.4. Access Route : 7 byte: 5-11
            // 3.4.1. Station No:
            lstSendData.Add((byte)this.stationNo);
            // 3.4.2. NetWork No:
            lstSendData.Add((byte)this.netWorkNo);
            // 3.4.3. PC No:
            lstSendData.Add((byte)this.pcNo);
            // 3.4.4. IO No:
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            // 3.4.5. Request Station:
            lstSendData.Add((byte)this.reqStationNo);
            //3.4.6. Seft Station :
            lstSendData.Add((byte)this.seftStationNo);

            //3.5. Request Data
            //3.5.1. Command: 0401
            lstSendData.Add(0x01);
            lstSendData.Add(0x04);
            //3.5.2. SubCommand: 0000
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);
            //3.5.3. Thanh ghi số bao nhiêu:
            // Tách thành các byte
            byte[] arrDevNumber = BitConverter.GetBytes(_devNumber);
            // Thêm 3 byte vào danh sách gửi đi
            lstSendData.Add(arrDevNumber[0]);
            lstSendData.Add(arrDevNumber[1]);
            lstSendData.Add(arrDevNumber[2]);
            // 3.5.4. Device Code:
            lstSendData.Add((byte)_devCode);

            //3.5.5. Number of Device Point: Số lượng thanh ghi liên tiếp
            int devNumPoint = 3;
            byte[] arrDevNumPoint = BitConverter.GetBytes(devNumPoint);
            lstSendData.Add(arrDevNumPoint[0]);
            lstSendData.Add(arrDevNumPoint[1]);

            //3.6. Sửa lại number of Data byte:
            int numByte = lstSendData.Count - 4;
            byte[] arrNumByte = BitConverter.GetBytes(numByte);
            lstSendData[2] = arrNumByte[0];
            lstSendData[3] = arrNumByte[1];

            //3.7 Tính sumcheck code:
            // 3.7.1. Tính tổng từ byte 2 đến hết
            int sum = 0;
            for (int i = 2; i < lstSendData.Count; i++)
            {
                sum += lstSendData[i];
            }
            //3.7.2. Chuyển tổng từ số nguyên thành kí tự:
            string strSum = sum.ToString("X");
            //3.7.2. Thêm kí tự 0 vào nếu chuỗi không đủ 2 kí tự:
            strSum = strSum.PadLeft(2, '0');
            // Hàm padleft là hàm điền kí tự yêu cầu sang bên trái
            // cho đủ số lượng kí tự yêu cầu
            // Ví dụ ban đầu strSum = "1", thì sau chạy lệnh strSum = "01";
            // Trong trường hợp strSum nhiều hơn 2 kí tự thì cần cắt ra 2 kí tự bên phải
            strSum = strSum.Substring(strSum.Length - 2, 2);
            //3.7.3. Chuyển chuỗi string về dạng ASCII:
            byte[] arrSum = ASCIIEncoding.ASCII.GetBytes(strSum);

            // 3.8. Control Code: 
            lstSendData.Add(0x10); //DLE
            lstSendData.Add(0x03); // EOT

            //3.9. Thêm sumcheck code ở mục 3.7 vào:
            lstSendData.Add(arrSum[0]);
            lstSendData.Add(arrSum[1]);

            // B4: Gửi dữ liệu đi:
            this.port.DiscardInBuffer();
            this.port.Write(lstSendData.ToArray(), 0, lstSendData.Count);

            // B5: Nhận dữ liệu về từ PLC:
            int timeOut = 1000;//ms = 1s
            // Đợi đến bao giờ đủ số lượng byte mong muốn: ít nhất 20 byte
            int expectBytes = 20;
            while (timeOut > 0)
            {
                if (this.port.BytesToRead >= expectBytes)
                {
                    break;
                }
                timeOut = timeOut - 10;// Mỗi lần trừ đi 10ms
                Thread.Sleep(10);// Dừng trong 10ms
            }
            // Vị trí này là thoát khỏi while ************
            if (timeOut <= 0)
            {
                return kq;
            }
            // Nhận dữ liệu:
            byte[] arrRcv = new byte[1024];
            this.port.Read(arrRcv, 0, this.port.BytesToRead);
            List<byte> lstRcv = new List<byte>();
            lstRcv.AddRange(arrRcv);

            // Bước 6: Phân tích dữ liệu nhận về từ PLC:
            // 6.1. Control Code:
            if (lstRcv[0] != 0x10 || lstRcv[1] != 0x02)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);// Remove từ phần tử số 0 và remove đi 2 phần tử
            //6.2. Kiểm tra xem number of databyte là bao nhiêu:
            short numberOfDataByte = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (numberOfDataByte < 12)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.3. Frame ID No:
            if (lstRcv[0] != 0xF8)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.4. Access Route:
            //6.4.1 Station No
            if (lstRcv[0] != (byte)this.stationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.2. NetWorkNo:
            if (lstRcv[0] != (byte)this.netWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.3. PCNo:
            if (lstRcv[0] != (byte)this.pcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.4. Request Destination Module IO No:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.4.5. Request Station No:
            if (lstRcv[0] != (byte)this.reqStationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.6. Seft Station No:
            if (lstRcv[0] != (byte)this.seftStationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            // 6.5. Response ID Code:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0xFF)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            // 6.6. Normal Completition:
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            //6.7. Response Data:
            kq = BitConverter.ToInt32(new byte[] { lstRcv[0], lstRcv[1], lstRcv[2], lstRcv[3] }, 0);
            return kq;
        }
        //Hàm đọc giá trị một số thực từ PLC
        public float ReadFloat(DevCode _devCode, int _devNumber)
        {
            float kq = 0f;
            //B1: Kiểm tra port đã khởi tạo chưa:
            if (this.port == null)
            {
                return kq;
            }
            //B2: Kiểm tra port đã mở chưa
            if (this.port.IsOpen == false)
            {
                return kq;
            }
            //B3: Chuẩn bị dữ liệu gửi xuống:
            List<byte> lstSendData = new List<byte>();
            //3.1. Control code: 0-1
            lstSendData.Add(0x10);// DLE
            lstSendData.Add(0x02);// STX
            //3.2. Number of databyte : 2-3
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);
            // 3.3. Frame ID No: 4
            lstSendData.Add(0xF8);
            // 3.4. Access Route : 7 byte: 5-11
            // 3.4.1. Station No:
            lstSendData.Add((byte)this.stationNo);
            // 3.4.2. NetWork No:
            lstSendData.Add((byte)this.netWorkNo);
            // 3.4.3. PC No:
            lstSendData.Add((byte)this.pcNo);
            // 3.4.4. IO No:
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            // 3.4.5. Request Station:
            lstSendData.Add((byte)this.reqStationNo);
            //3.4.6. Seft Station :
            lstSendData.Add((byte)this.seftStationNo);

            //3.5. Request Data
            //3.5.1. Command: 0401
            lstSendData.Add(0x01);
            lstSendData.Add(0x04);
            //3.5.2. SubCommand: 0000
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);
            //3.5.3. Thanh ghi số bao nhiêu:
            // Tách thành các byte
            byte[] arrDevNumber = BitConverter.GetBytes(_devNumber);
            // Thêm 3 byte vào danh sách gửi đi
            lstSendData.Add(arrDevNumber[0]);
            lstSendData.Add(arrDevNumber[1]);
            lstSendData.Add(arrDevNumber[2]);
            // 3.5.4. Device Code:
            lstSendData.Add((byte)_devCode);

            //3.5.5. Number of Device Point: Số lượng thanh ghi liên tiếp
            int devNumPoint = 3;
            byte[] arrDevNumPoint = BitConverter.GetBytes(devNumPoint);
            lstSendData.Add(arrDevNumPoint[0]);
            lstSendData.Add(arrDevNumPoint[1]);

            //3.6. Sửa lại number of Data byte:
            int numByte = lstSendData.Count - 4;
            byte[] arrNumByte = BitConverter.GetBytes(numByte);
            lstSendData[2] = arrNumByte[0];
            lstSendData[3] = arrNumByte[1];

            //3.7 Tính sumcheck code:
            // 3.7.1. Tính tổng từ byte 2 đến hết
            int sum = 0;
            for (int i = 2; i < lstSendData.Count; i++)
            {
                sum += lstSendData[i];
            }
            //3.7.2. Chuyển tổng từ số nguyên thành kí tự:
            string strSum = sum.ToString("X");
            //3.7.2. Thêm kí tự 0 vào nếu chuỗi không đủ 2 kí tự:
            strSum = strSum.PadLeft(2, '0');
            // Hàm padleft là hàm điền kí tự yêu cầu sang bên trái
            // cho đủ số lượng kí tự yêu cầu
            // Ví dụ ban đầu strSum = "1", thì sau chạy lệnh strSum = "01";
            // Trong trường hợp strSum nhiều hơn 2 kí tự thì cần cắt ra 2 kí tự bên phải
            strSum = strSum.Substring(strSum.Length - 2, 2);
            //3.7.3. Chuyển chuỗi string về dạng ASCII:
            byte[] arrSum = ASCIIEncoding.ASCII.GetBytes(strSum);

            // 3.8. Control Code: 
            lstSendData.Add(0x10); //DLE
            lstSendData.Add(0x03); // EOT

            //3.9. Thêm sumcheck code ở mục 3.7 vào:
            lstSendData.Add(arrSum[0]);
            lstSendData.Add(arrSum[1]);

            // B4: Gửi dữ liệu đi:
            this.port.DiscardInBuffer();
            this.port.Write(lstSendData.ToArray(), 0, lstSendData.Count);

            // B5: Nhận dữ liệu về từ PLC:
            int timeOut = 1000;//ms = 1s
            // Đợi đến bao giờ đủ số lượng byte mong muốn: ít nhất 20 byte
            int expectBytes = 20;
            while (timeOut > 0)
            {
                if (this.port.BytesToRead >= expectBytes)
                {
                    break;
                }
                timeOut = timeOut - 10;// Mỗi lần trừ đi 10ms
                Thread.Sleep(10);// Dừng trong 10ms
            }
            // Vị trí này là thoát khỏi while ************
            if (timeOut <= 0)
            {
                return kq;
            }
            // Nhận dữ liệu:
            byte[] arrRcv = new byte[1024];
            this.port.Read(arrRcv, 0, this.port.BytesToRead);
            List<byte> lstRcv = new List<byte>();
            lstRcv.AddRange(arrRcv);

            // Bước 6: Phân tích dữ liệu nhận về từ PLC:
            // 6.1. Control Code:
            if (lstRcv[0] != 0x10 || lstRcv[1] != 0x02)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);// Remove từ phần tử số 0 và remove đi 2 phần tử
            //6.2. Kiểm tra xem number of databyte là bao nhiêu:
            short numberOfDataByte = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (numberOfDataByte < 12)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.3. Frame ID No:
            if (lstRcv[0] != 0xF8)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.4. Access Route:
            //6.4.1 Station No
            if (lstRcv[0] != (byte)this.stationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.2. NetWorkNo:
            if (lstRcv[0] != (byte)this.netWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.3. PCNo:
            if (lstRcv[0] != (byte)this.pcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.4. Request Destination Module IO No:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.4.5. Request Station No:
            if (lstRcv[0] != (byte)this.reqStationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.6. Seft Station No:
            if (lstRcv[0] != (byte)this.seftStationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            // 6.5. Response ID Code:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0xFF)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            // 6.6. Normal Completition:
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            //6.7. Response Data:
            kq = BitConverter.ToSingle(new byte[] { lstRcv[0], lstRcv[1], lstRcv[2], lstRcv[3] }, 0);
            return kq;
        }
        // Hàm đọc giá trị của một bit:
        public bool ReadBit(DevCode _devCode, int _devNumber)
        {
            bool kq = false;
            //B1: Kiểm tra port đã khởi tạo chưa:
            if (this.port == null)
            {
                return kq;
            }
            //B2: Kiểm tra port đã mở chưa
            if (this.port.IsOpen == false)
            {
                return kq;
            }
            //B3: Chuẩn bị dữ liệu gửi xuống:
            List<byte> lstSendData = new List<byte>();
            //3.1. Control code: 0-1
            lstSendData.Add(0x10);// DLE
            lstSendData.Add(0x02);// STX
            //3.2. Number of databyte : 2-3
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);
            // 3.3. Frame ID No: 4
            lstSendData.Add(0xF8);
            // 3.4. Access Route : 7 byte: 5-11
            // 3.4.1. Station No:
            lstSendData.Add((byte)this.stationNo);
            // 3.4.2. NetWork No:
            lstSendData.Add((byte)this.netWorkNo);
            // 3.4.3. PC No:
            lstSendData.Add((byte)this.pcNo);
            // 3.4.4. IO No:
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            // 3.4.5. Request Station:
            lstSendData.Add((byte)this.reqStationNo);
            //3.4.6. Seft Station :
            lstSendData.Add((byte)this.seftStationNo);

            //3.5. Request Data
            //3.5.1. Command: 0401
            lstSendData.Add(0x01);
            lstSendData.Add(0x04);
            //3.5.2. SubCommand: 0001
            lstSendData.Add(0x01);
            lstSendData.Add(0x00);
            //3.5.3. Bit số bao nhiêu:
            // Tách thành các byte
            byte[] arrDevNumber = BitConverter.GetBytes(_devNumber);
            // Thêm 3 byte vào danh sách gửi đi
            lstSendData.Add(arrDevNumber[0]);
            lstSendData.Add(arrDevNumber[1]);
            lstSendData.Add(arrDevNumber[2]);
            // 3.5.4. Device Code:
            lstSendData.Add((byte)_devCode);

            //3.5.5. Number of Device Point:Số lượng bit liên tiếp
            int devNumPoint = 1;
            byte[] arrDevNumPoint = BitConverter.GetBytes(devNumPoint);
            lstSendData.Add(arrDevNumPoint[0]);
            lstSendData.Add(arrDevNumPoint[1]);

            //3.6. Sửa lại number of Data byte:
            int numByte = lstSendData.Count - 4;
            byte[] arrNumByte = BitConverter.GetBytes(numByte);
            lstSendData[2] = arrNumByte[0];
            lstSendData[3] = arrNumByte[1];

            //3.7 Tính sumcheck code:
            // 3.7.1. Tính tổng từ byte 2 đến hết
            int sum = 0;
            for (int i = 2; i < lstSendData.Count; i++)
            {
                sum += lstSendData[i];
            }
            //3.7.2. Chuyển tổng từ số nguyên thành kí tự:
            string strSum = sum.ToString("X");
            //3.7.2. Thêm kí tự 0 vào nếu chuỗi không đủ 2 kí tự:
            strSum = strSum.PadLeft(2, '0');
            // Hàm padleft là hàm điền kí tự yêu cầu sang bên trái
            // cho đủ số lượng kí tự yêu cầu
            // Ví dụ ban đầu strSum = "1", thì sau chạy lệnh strSum = "01";
            // Trong trường hợp strSum nhiều hơn 2 kí tự thì cần cắt ra 2 kí tự bên phải
            strSum = strSum.Substring(strSum.Length - 2, 2);
            //3.7.3. Chuyển chuỗi string về dạng ASCII:
            byte[] arrSum = ASCIIEncoding.ASCII.GetBytes(strSum);

            // 3.8. Control Code: 
            lstSendData.Add(0x10); //DLE
            lstSendData.Add(0x03); // EOT

            //3.9. Thêm sumcheck code ở mục 3.7 vào:
            lstSendData.Add(arrSum[0]);
            lstSendData.Add(arrSum[1]);

            // B4: Gửi dữ liệu đi:
            this.port.DiscardInBuffer();
            this.port.Write(lstSendData.ToArray(), 0, lstSendData.Count);

            // B5: Nhận dữ liệu về từ PLC:
            int timeOut = 1000;//ms = 1s
            // Đợi đến bao giờ đủ số lượng byte mong muốn: ít nhất 20 byte
            int expectBytes = 20;
            while (timeOut > 0)
            {
                if (this.port.BytesToRead >= expectBytes)
                {
                    break;
                }
                timeOut = timeOut - 10;// Mỗi lần trừ đi 10ms
                Thread.Sleep(10);// Dừng trong 10ms
            }
            // Vị trí này là thoát khỏi while ************
            if (timeOut <= 0)
            {
                return kq;
            }
            // Nhận dữ liệu:
            byte[] arrRcv = new byte[1024];
            this.port.Read(arrRcv, 0, this.port.BytesToRead);
            List<byte> lstRcv = new List<byte>();
            lstRcv.AddRange(arrRcv);

            // Bước 6: Phân tích dữ liệu nhận về từ PLC:
            // 6.1. Control Code:
            if (lstRcv[0] != 0x10 || lstRcv[1] != 0x02)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);// Remove từ phần tử số 0 và remove đi 2 phần tử
            //6.2. Kiểm tra xem number of databyte là bao nhiêu:
            short numberOfDataByte = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (numberOfDataByte < 12)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.3. Frame ID No:
            if (lstRcv[0] != 0xF8)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.4. Access Route:
            //6.4.1 Station No
            if (lstRcv[0] != (byte)this.stationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.2. NetWorkNo:
            if (lstRcv[0] != (byte)this.netWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.3. PCNo:
            if (lstRcv[0] != (byte)this.pcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.4. Request Destination Module IO No:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.4.5. Request Station No:
            if (lstRcv[0] != (byte)this.reqStationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.6. Seft Station No:
            if (lstRcv[0] != (byte)this.seftStationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            // 6.5. Response ID Code:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0xFF)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            // 6.6. Normal Completition:
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            //6.7. Response Data:
            
            if (lstRcv[0]!=0)
            {
                kq = true;
            }

            return kq;
        }
        public List<bool> ReadMultiBit(DevCode _devCode, int _devNumber, int _count)
        {
            List<bool> kq = new List<bool>();
            if (_count<=0)
            {
                return kq;
            }
            //B1: Kiểm tra port đã khởi tạo chưa:
            if (this.port == null)
            {
                return kq;
            }
            //B2: Kiểm tra port đã mở chưa
            if (this.port.IsOpen == false)
            {
                return kq;
            }
            //B3: Chuẩn bị dữ liệu gửi xuống:
            List<byte> lstSendData = new List<byte>();
            //3.1. Control code: 0-1
            lstSendData.Add(0x10);// DLE
            lstSendData.Add(0x02);// STX
            //3.2. Number of databyte : 2-3
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);
            // 3.3. Frame ID No: 4
            lstSendData.Add(0xF8);
            // 3.4. Access Route : 7 byte: 5-11
            // 3.4.1. Station No:
            lstSendData.Add((byte)this.stationNo);
            // 3.4.2. NetWork No:
            lstSendData.Add((byte)this.netWorkNo);
            // 3.4.3. PC No:
            lstSendData.Add((byte)this.pcNo);
            // 3.4.4. IO No:
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            // 3.4.5. Request Station:
            lstSendData.Add((byte)this.reqStationNo);
            //3.4.6. Seft Station :
            lstSendData.Add((byte)this.seftStationNo);

            //3.5. Request Data
            //3.5.1. Command: 0401
            lstSendData.Add(0x01);
            lstSendData.Add(0x04);
            //3.5.2. SubCommand: 0001
            lstSendData.Add(0x01);
            lstSendData.Add(0x00);
            //3.5.3. Bit số bao nhiêu:
            // Tách thành các byte
            byte[] arrDevNumber = BitConverter.GetBytes(_devNumber);
            // Thêm 3 byte vào danh sách gửi đi
            lstSendData.Add(arrDevNumber[0]);
            lstSendData.Add(arrDevNumber[1]);
            lstSendData.Add(arrDevNumber[2]);
            // 3.5.4. Device Code:
            lstSendData.Add((byte)_devCode);

            //3.5.5. Number of Device Point:Số lượng bit liên tiếp
            int devNumPoint = _count;
            byte[] arrDevNumPoint = BitConverter.GetBytes(devNumPoint);
            lstSendData.Add(arrDevNumPoint[0]);
            lstSendData.Add(arrDevNumPoint[1]);

            //3.6. Sửa lại number of Data byte:
            int numByte = lstSendData.Count - 4;
            byte[] arrNumByte = BitConverter.GetBytes(numByte);
            lstSendData[2] = arrNumByte[0];
            lstSendData[3] = arrNumByte[1];

            //3.7 Tính sumcheck code:
            // 3.7.1. Tính tổng từ byte 2 đến hết
            int sum = 0;
            for (int i = 2; i < lstSendData.Count; i++)
            {
                sum += lstSendData[i];
            }
            //3.7.2. Chuyển tổng từ số nguyên thành kí tự:
            string strSum = sum.ToString("X");
            //3.7.2. Thêm kí tự 0 vào nếu chuỗi không đủ 2 kí tự:
            strSum = strSum.PadLeft(2, '0');
            // Hàm padleft là hàm điền kí tự yêu cầu sang bên trái
            // cho đủ số lượng kí tự yêu cầu
            // Ví dụ ban đầu strSum = "1", thì sau chạy lệnh strSum = "01";
            // Trong trường hợp strSum nhiều hơn 2 kí tự thì cần cắt ra 2 kí tự bên phải
            strSum = strSum.Substring(strSum.Length - 2, 2);
            //3.7.3. Chuyển chuỗi string về dạng ASCII:
            byte[] arrSum = ASCIIEncoding.ASCII.GetBytes(strSum);

            // 3.8. Control Code: 
            lstSendData.Add(0x10); //DLE
            lstSendData.Add(0x03); // EOT

            //3.9. Thêm sumcheck code ở mục 3.7 vào:
            lstSendData.Add(arrSum[0]);
            lstSendData.Add(arrSum[1]);

            // B4: Gửi dữ liệu đi:
            this.port.DiscardInBuffer();
            this.port.Write(lstSendData.ToArray(), 0, lstSendData.Count);

            // B5: Nhận dữ liệu về từ PLC:
            int timeOut = 1000;//ms = 1s
            // Đợi đến bao giờ đủ số lượng byte mong muốn: ít nhất 20 byte
            int expectBytes = 20;
            while (timeOut > 0)
            {
                if (this.port.BytesToRead >= expectBytes)
                {
                    break;
                }
                timeOut = timeOut - 10;// Mỗi lần trừ đi 10ms
                Thread.Sleep(10);// Dừng trong 10ms
            }
            // Vị trí này là thoát khỏi while ************
            if (timeOut <= 0)
            {
                return kq;
            }
            // Nhận dữ liệu:
            byte[] arrRcv = new byte[1024];
            this.port.Read(arrRcv, 0, this.port.BytesToRead);
            List<byte> lstRcv = new List<byte>();
            lstRcv.AddRange(arrRcv);

            // Bước 6: Phân tích dữ liệu nhận về từ PLC:
            // 6.1. Control Code:
            if (lstRcv[0] != 0x10 || lstRcv[1] != 0x02)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);// Remove từ phần tử số 0 và remove đi 2 phần tử
            //6.2. Kiểm tra xem number of databyte là bao nhiêu:
            short numberOfDataByte = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (numberOfDataByte < 12)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.3. Frame ID No:
            if (lstRcv[0] != 0xF8)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.4. Access Route:
            //6.4.1 Station No
            if (lstRcv[0] != (byte)this.stationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.2. NetWorkNo:
            if (lstRcv[0] != (byte)this.netWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.3. PCNo:
            if (lstRcv[0] != (byte)this.pcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.4. Request Destination Module IO No:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.4.5. Request Station No:
            if (lstRcv[0] != (byte)this.reqStationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.6. Seft Station No:
            if (lstRcv[0] != (byte)this.seftStationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            // 6.5. Response ID Code:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0xFF)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            // 6.6. Normal Completition:
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            //6.7. Response Data:
            int byteCount = _count / 2 + _count % 2;
            // ví dụ: cần đọc 3 bit _count = 3
            // _count/2 = 1
            // _count%2 = 1
            for (int i = 0; i < byteCount; i++)
            {
                if (lstRcv[0] == 0x00)
                {
                    kq.Add(false);
                    kq.Add(false);
                }
                else if (lstRcv[0]==0x10)
                {
                    kq.Add(true);
                    kq.Add(false);
                }
                else if (lstRcv[0] == 0x01)
                {
                    kq.Add(false);
                    kq.Add(true);
                }
                else
                {
                    kq.Add(true);
                    kq.Add(true);
                }
                lstRcv.RemoveRange(0, 1);
            }
            if (kq.Count>_count)
            {
                kq.RemoveAt(_count);
            }

            return kq;
        }
        public bool WriteBit(DevCode _devCode, int _devNumber, bool _value)
        {
            bool kq = false;
            //B1: Kiểm tra port đã khởi tạo chưa:
            if (this.port == null)
            {
                return kq;
            }
            //B2: Kiểm tra port đã mở chưa
            if (this.port.IsOpen == false)
            {
                return kq;
            }
            //B3: Chuẩn bị dữ liệu gửi xuống:
            List<byte> lstSendData = new List<byte>();
            //3.1. Control code: 0-1
            lstSendData.Add(0x10);// DLE
            lstSendData.Add(0x02);// STX
            //3.2. Number of databyte : 2-3
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);
            // 3.3. Frame ID No: 4
            lstSendData.Add(0xF8);
            // 3.4. Access Route : 7 byte: 5-11
            // 3.4.1. Station No:
            lstSendData.Add((byte)this.stationNo);
            // 3.4.2. NetWork No:
            lstSendData.Add((byte)this.netWorkNo);
            // 3.4.3. PC No:
            lstSendData.Add((byte)this.pcNo);
            // 3.4.4. IO No:
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            // 3.4.5. Request Station:
            lstSendData.Add((byte)this.reqStationNo);
            //3.4.6. Seft Station :
            lstSendData.Add((byte)this.seftStationNo);

            //3.5. Request Data
            //3.5.1. Command: 1401
            lstSendData.Add(0x01);
            lstSendData.Add(0x14);
            //3.5.2. SubCommand: 0001
            lstSendData.Add(0x01);
            lstSendData.Add(0x00);
            //3.5.3. Bit số bao nhiêu:
            // Tách thành các byte
            byte[] arrDevNumber = BitConverter.GetBytes(_devNumber);
            // Thêm 3 byte vào danh sách gửi đi
           
            lstSendData.Add(arrDevNumber[0]);
            lstSendData.Add(arrDevNumber[1]);
            lstSendData.Add(arrDevNumber[2]);
            // 3.5.4. Device Code:
            lstSendData.Add((byte)_devCode);

            //3.5.5. Number of Device Point:Số lượng bit liên tiếp
            int devNumPoint = 1;
            byte[] arrDevNumPoint = BitConverter.GetBytes(devNumPoint);
            lstSendData.Add(arrDevNumPoint[0]);
            lstSendData.Add(arrDevNumPoint[1]);

            //3.5.6. Data: ON/OFF:
            //lstSendData.Add(0x10);// DLE
            if (_value == true)
            {
                lstSendData.Add(0x11);
            }
            else
            {
                lstSendData.Add(0x00);
            }
            //3.6. Sửa lại number of Data byte:
            int numByte = lstSendData.Count - 4;
            byte[] arrNumByte = BitConverter.GetBytes(numByte);
           
            lstSendData[2] = arrNumByte[0];
            lstSendData[3] = arrNumByte[1];

            //3.7 Tính sumcheck code:
            // 3.7.1. Tính tổng từ byte 2 đến hết
            int sum = 0;
            for (int i = 2; i < lstSendData.Count; i++)
            {
                sum += lstSendData[i];
            }
            // sumcheck code không tính DLE:
          //  sum = sum - 0x10;
            //3.7.2. Chuyển tổng từ số nguyên thành kí tự:
            string strSum = sum.ToString("X");
            //3.7.2. Thêm kí tự 0 vào nếu chuỗi không đủ 2 kí tự:
            strSum = strSum.PadLeft(2, '0');
            // Hàm padleft là hàm điền kí tự yêu cầu sang bên trái
            // cho đủ số lượng kí tự yêu cầu
            // Ví dụ ban đầu strSum = "1", thì sau chạy lệnh strSum = "01";
            // Trong trường hợp strSum nhiều hơn 2 kí tự thì cần cắt ra 2 kí tự bên phải
            strSum = strSum.Substring(strSum.Length - 2, 2);
            //3.7.3. Chuyển chuỗi string về dạng ASCII:
            byte[] arrSum = ASCIIEncoding.ASCII.GetBytes(strSum);

            // 3.8. Control Code: 
            lstSendData.Add(0x10); //DLE
            lstSendData.Add(0x03); // EOT

            //3.9. Thêm sumcheck code ở mục 3.7 vào:
            lstSendData.Add(arrSum[0]);
            lstSendData.Add(arrSum[1]);

            // B4: Gửi dữ liệu đi:
            this.port.DiscardInBuffer();
            this.port.Write(lstSendData.ToArray(), 0, lstSendData.Count);

            // B5: Nhận dữ liệu về từ PLC:
            int timeOut = 1000;//ms = 1s
            // Đợi đến bao giờ đủ số lượng byte mong muốn: ít nhất 20 byte
            int expectBytes = 20;
            while (timeOut > 0)
            {
                if (this.port.BytesToRead >= expectBytes)
                {
                    break;
                }
                timeOut = timeOut - 10;// Mỗi lần trừ đi 10ms
                Thread.Sleep(10);// Dừng trong 10ms
            }
            // Vị trí này là thoát khỏi while ************
            if (timeOut <= 0)
            {
                return kq;
            }
            // Nhận dữ liệu:
            byte[] arrRcv = new byte[1024];
            this.port.Read(arrRcv, 0, this.port.BytesToRead);
            List<byte> lstRcv = new List<byte>();
            lstRcv.AddRange(arrRcv);

            // Bước 6: Phân tích dữ liệu nhận về từ PLC:
            // 6.1. Control Code:
            if (lstRcv[0] != 0x10 || lstRcv[1] != 0x02)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);// Remove từ phần tử số 0 và remove đi 2 phần tử
            //6.2. Kiểm tra xem number of databyte là bao nhiêu:
            short numberOfDataByte = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (numberOfDataByte < 12)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.3. Frame ID No:
            if (lstRcv[0] != 0xF8)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.4. Access Route:
            //6.4.1 Station No
            if (lstRcv[0] != (byte)this.stationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.2. NetWorkNo:
            if (lstRcv[0] != (byte)this.netWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.3. PCNo:
            if (lstRcv[0] != (byte)this.pcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.4. Request Destination Module IO No:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.4.5. Request Station No:
            if (lstRcv[0] != (byte)this.reqStationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.6. Seft Station No:
            if (lstRcv[0] != (byte)this.seftStationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            // 6.5. Response ID Code:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0xFF)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            // 6.6. Normal Completition:
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            kq = true;
            return kq;
        }
        public bool WriteMultiBit(DevCode _devCode, int _devNumber, List<bool> _lstValue)
        {
            bool kq = false;
            if (_lstValue==null)
            {
                return kq;
            }
            if (_lstValue.Count<=1)
            {
                return kq;
            }
            //B1: Kiểm tra port đã khởi tạo chưa:
            if (this.port == null)
            {
                return kq;
            }
            //B2: Kiểm tra port đã mở chưa
            if (this.port.IsOpen == false)
            {
                return kq;
            }
            //B3: Chuẩn bị dữ liệu gửi xuống:
            List<byte> lstSendData = new List<byte>();
            //3.1. Control code: 0-1
            lstSendData.Add(0x10);// DLE
            lstSendData.Add(0x02);// STX
            //3.2. Number of databyte : 2-3
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);
            // 3.3. Frame ID No: 4
            lstSendData.Add(0xF8);
            // 3.4. Access Route : 7 byte: 5-11
            // 3.4.1. Station No:
            lstSendData.Add((byte)this.stationNo);
            // 3.4.2. NetWork No:
            lstSendData.Add((byte)this.netWorkNo);
            // 3.4.3. PC No:
            lstSendData.Add((byte)this.pcNo);
            // 3.4.4. IO No:
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            // 3.4.5. Request Station:
            lstSendData.Add((byte)this.reqStationNo);
            //3.4.6. Seft Station :
            lstSendData.Add((byte)this.seftStationNo);

            //3.5. Request Data
            //3.5.1. Command: 1401
            lstSendData.Add(0x01);
            lstSendData.Add(0x14);
            //3.5.2. SubCommand: 0001
            lstSendData.Add(0x01);
            lstSendData.Add(0x00);
            //3.5.3. Bit số bao nhiêu:
            // Tách thành các byte
            byte[] arrDevNumber = BitConverter.GetBytes(_devNumber);
            // Thêm 3 byte vào danh sách gửi đi
            
            lstSendData.Add(arrDevNumber[0]);
            lstSendData.Add(arrDevNumber[1]);
            lstSendData.Add(arrDevNumber[2]);
            // 3.5.4. Device Code:
            lstSendData.Add((byte)_devCode);

            //3.5.5. Number of Device Point:Số lượng bit liên tiếp
            int devNumPoint = _lstValue.Count;
            byte[] arrDevNumPoint = BitConverter.GetBytes(devNumPoint);
            lstSendData.Add(arrDevNumPoint[0]);
            lstSendData.Add(arrDevNumPoint[1]);

            //3.5.6. Data: ON/OFF:

            // True - False - True : 2 byte
            // Cần mấy byte:
            // Ví dụ: M10 đến M14: có 5M, 3 byte.
            int byteCount = _lstValue.Count / 2 + _lstValue.Count % 2;
            for (int i = 0; i < byteCount; i++)
            {
                int bitTruoc = 0;//M10
                int bitSau =   0;//M11
                // Bit truoc
                if (_lstValue[i*2] == false)
                {
                    bitTruoc = 0x00;
                }
                else
                    bitTruoc = 0x10;
                // Bit sau:
                if (_lstValue.Count>(i*2+1))
                {
                    if (_lstValue[i * 2 + 1] == false)
                    {
                        bitSau = 0x00;
                    }
                    else
                    {
                        bitSau = 0x01;
                    }
                }
                // Giá trị cần ghi
                byte GiaTri = (byte)(bitTruoc|bitSau);
                // Thêm DLE: 
                lstSendData.Add(0x10);
                lstSendData.Add(GiaTri);
                
            }
            //3.6. Sửa lại number of Data byte:
            int numByte = lstSendData.Count - 4 - byteCount;
            byte[] arrNumByte = BitConverter.GetBytes(numByte);

            lstSendData[2] = arrNumByte[0];
            lstSendData[3] = arrNumByte[1];

            //3.7 Tính sumcheck code:
            // 3.7.1. Tính tổng từ byte 2 đến hết
            int sum = 0;
            for (int i = 2; i < lstSendData.Count; i++)
            {
                sum += lstSendData[i];
            }
            // sumcheck code không tính DLE:
            sum = sum - byteCount * 0x10;
            //3.7.2. Chuyển tổng từ số nguyên thành kí tự:
            string strSum = sum.ToString("X");
            //3.7.2. Thêm kí tự 0 vào nếu chuỗi không đủ 2 kí tự:
            strSum = strSum.PadLeft(2, '0');
            // Hàm padleft là hàm điền kí tự yêu cầu sang bên trái
            // cho đủ số lượng kí tự yêu cầu
            // Ví dụ ban đầu strSum = "1", thì sau chạy lệnh strSum = "01";
            // Trong trường hợp strSum nhiều hơn 2 kí tự thì cần cắt ra 2 kí tự bên phải
            strSum = strSum.Substring(strSum.Length - 2, 2);
            //3.7.3. Chuyển chuỗi string về dạng ASCII:
            byte[] arrSum = ASCIIEncoding.ASCII.GetBytes(strSum);

            // 3.8. Control Code: 
            lstSendData.Add(0x10); //DLE
            lstSendData.Add(0x03); // EOT

            //3.9. Thêm sumcheck code ở mục 3.7 vào:
            lstSendData.Add(arrSum[0]);
            lstSendData.Add(arrSum[1]);

            // B4: Gửi dữ liệu đi:
            this.port.DiscardInBuffer();
            this.port.Write(lstSendData.ToArray(), 0, lstSendData.Count);

            // B5: Nhận dữ liệu về từ PLC:
            int timeOut = 1000;//ms = 1s
            // Đợi đến bao giờ đủ số lượng byte mong muốn: ít nhất 20 byte
            int expectBytes = 20;
            while (timeOut > 0)
            {
                if (this.port.BytesToRead >= expectBytes)
                {
                    break;
                }
                timeOut = timeOut - 10;// Mỗi lần trừ đi 10ms
                Thread.Sleep(10);// Dừng trong 10ms
            }
            // Vị trí này là thoát khỏi while ************
            if (timeOut <= 0)
            {
                return kq;
            }
            // Nhận dữ liệu:
            byte[] arrRcv = new byte[1024];
            this.port.Read(arrRcv, 0, this.port.BytesToRead);
            List<byte> lstRcv = new List<byte>();
            lstRcv.AddRange(arrRcv);

            // Bước 6: Phân tích dữ liệu nhận về từ PLC:
            // 6.1. Control Code:
            if (lstRcv[0] != 0x10 || lstRcv[1] != 0x02)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);// Remove từ phần tử số 0 và remove đi 2 phần tử
            //6.2. Kiểm tra xem number of databyte là bao nhiêu:
            short numberOfDataByte = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (numberOfDataByte < 12)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.3. Frame ID No:
            if (lstRcv[0] != 0xF8)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.4. Access Route:
            //6.4.1 Station No
            if (lstRcv[0] != (byte)this.stationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.2. NetWorkNo:
            if (lstRcv[0] != (byte)this.netWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.3. PCNo:
            if (lstRcv[0] != (byte)this.pcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.4. Request Destination Module IO No:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.4.5. Request Station No:
            if (lstRcv[0] != (byte)this.reqStationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.6. Seft Station No:
            if (lstRcv[0] != (byte)this.seftStationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            // 6.5. Response ID Code:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0xFF)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            // 6.6. Normal Completition:
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            kq = true;
            return kq;
        }
        public bool WriteWord(DevCode _devCode, int _devNumber, short _value)
        {
            bool kq = false;
            //B1: Kiểm tra port đã khởi tạo chưa:
            if (this.port == null)
            {
                return kq;
            }
            //B2: Kiểm tra port đã mở chưa
            if (this.port.IsOpen == false)
            {
                return kq;
            }
            //B3: Chuẩn bị dữ liệu gửi xuống:
            List<byte> lstSendData = new List<byte>();
            //3.1. Control code: 0-1
            lstSendData.Add(0x10);// DLE
            lstSendData.Add(0x02);// STX
            //3.2. Number of databyte : 2-3
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);
            // 3.3. Frame ID No: 4
            lstSendData.Add(0xF8);
            // 3.4. Access Route : 7 byte: 5-11
            // 3.4.1. Station No:
            lstSendData.Add((byte)this.stationNo);
            // 3.4.2. NetWork No:
            lstSendData.Add((byte)this.netWorkNo);
            // 3.4.3. PC No:
            lstSendData.Add((byte)this.pcNo);
            // 3.4.4. IO No:
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            // 3.4.5. Request Station:
            lstSendData.Add((byte)this.reqStationNo);
            //3.4.6. Seft Station :
            lstSendData.Add((byte)this.seftStationNo);

            //3.5. Request Data
            //3.5.1. Command: 1401
            lstSendData.Add(0x01);
            lstSendData.Add(0x14);
            //3.5.2. SubCommand: 0000
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);
            //3.5.3. Bit số bao nhiêu:
            // Tách thành các byte
            byte[] arrDevNumber = BitConverter.GetBytes(_devNumber);
            // Thêm 3 byte vào danh sách gửi đi

            lstSendData.Add(arrDevNumber[0]);
            lstSendData.Add(arrDevNumber[1]);
            lstSendData.Add(arrDevNumber[2]);
            // 3.5.4. Device Code:
            lstSendData.Add((byte)_devCode);

            //3.5.5. Number of Device Point:Số lượng bit liên tiếp
            int devNumPoint = 1;
            byte[] arrDevNumPoint = BitConverter.GetBytes(devNumPoint);
            lstSendData.Add(arrDevNumPoint[0]);
            lstSendData.Add(arrDevNumPoint[1]);

            //3.5.6. Data:
            byte[] arrData = BitConverter.GetBytes(_value);
            lstSendData.Add(arrData[0]);
            lstSendData.Add(arrData[1]);
           
            //3.6. Sửa lại number of Data byte:
            int numByte = lstSendData.Count - 4;
            byte[] arrNumByte = BitConverter.GetBytes(numByte);

            lstSendData[2] = arrNumByte[0];
            lstSendData[3] = arrNumByte[1];

            //3.7 Tính sumcheck code:
            // 3.7.1. Tính tổng từ byte 2 đến hết
            int sum = 0;
            for (int i = 2; i < lstSendData.Count; i++)
            {
                sum += lstSendData[i];
            }
            // sumcheck code không tính DLE:
            //  sum = sum - 0x10;
            //3.7.2. Chuyển tổng từ số nguyên thành kí tự:
            string strSum = sum.ToString("X");
            //3.7.2. Thêm kí tự 0 vào nếu chuỗi không đủ 2 kí tự:
            strSum = strSum.PadLeft(2, '0');
            // Hàm padleft là hàm điền kí tự yêu cầu sang bên trái
            // cho đủ số lượng kí tự yêu cầu
            // Ví dụ ban đầu strSum = "1", thì sau chạy lệnh strSum = "01";
            // Trong trường hợp strSum nhiều hơn 2 kí tự thì cần cắt ra 2 kí tự bên phải
            strSum = strSum.Substring(strSum.Length - 2, 2);
            //3.7.3. Chuyển chuỗi string về dạng ASCII:
            byte[] arrSum = ASCIIEncoding.ASCII.GetBytes(strSum);

            // 3.8. Control Code: 
            lstSendData.Add(0x10); //DLE
            lstSendData.Add(0x03); // EOT

            //3.9. Thêm sumcheck code ở mục 3.7 vào:
            lstSendData.Add(arrSum[0]);
            lstSendData.Add(arrSum[1]);

            // B4: Gửi dữ liệu đi:
            this.port.DiscardInBuffer();
            this.port.Write(lstSendData.ToArray(), 0, lstSendData.Count);

            // B5: Nhận dữ liệu về từ PLC:
            int timeOut = 1000;//ms = 1s
            // Đợi đến bao giờ đủ số lượng byte mong muốn: ít nhất 20 byte
            int expectBytes = 20;
            while (timeOut > 0)
            {
                if (this.port.BytesToRead >= expectBytes)
                {
                    break;
                }
                timeOut = timeOut - 10;// Mỗi lần trừ đi 10ms
                Thread.Sleep(10);// Dừng trong 10ms
            }
            // Vị trí này là thoát khỏi while ************
            if (timeOut <= 0)
            {
                return kq;
            }
            // Nhận dữ liệu:
            byte[] arrRcv = new byte[1024];
            this.port.Read(arrRcv, 0, this.port.BytesToRead);
            List<byte> lstRcv = new List<byte>();
            lstRcv.AddRange(arrRcv);

            // Bước 6: Phân tích dữ liệu nhận về từ PLC:
            // 6.1. Control Code:
            if (lstRcv[0] != 0x10 || lstRcv[1] != 0x02)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);// Remove từ phần tử số 0 và remove đi 2 phần tử
            //6.2. Kiểm tra xem number of databyte là bao nhiêu:
            short numberOfDataByte = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (numberOfDataByte < 12)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.3. Frame ID No:
            if (lstRcv[0] != 0xF8)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.4. Access Route:
            //6.4.1 Station No
            if (lstRcv[0] != (byte)this.stationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.2. NetWorkNo:
            if (lstRcv[0] != (byte)this.netWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.3. PCNo:
            if (lstRcv[0] != (byte)this.pcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.4. Request Destination Module IO No:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.4.5. Request Station No:
            if (lstRcv[0] != (byte)this.reqStationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.6. Seft Station No:
            if (lstRcv[0] != (byte)this.seftStationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            // 6.5. Response ID Code:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0xFF)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            // 6.6. Normal Completition:
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            kq = true;
            return kq;
        }
        public bool WriteMultiWord(DevCode _devCode, int _devNumber,List<short> _lstValue)
        {
            bool kq = false;
            if (_lstValue==null)
            {
                return kq;
            }
            if (_lstValue.Count<=0)
            {
                return kq;
            }
            //B1: Kiểm tra port đã khởi tạo chưa:
            if (this.port == null)
            {
                return kq;
            }
            //B2: Kiểm tra port đã mở chưa
            if (this.port.IsOpen == false)
            {
                return kq;
            }
            //B3: Chuẩn bị dữ liệu gửi xuống:
            List<byte> lstSendData = new List<byte>();
            //3.1. Control code: 0-1
            lstSendData.Add(0x10);// DLE
            lstSendData.Add(0x02);// STX
            //3.2. Number of databyte : 2-3
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);
            // 3.3. Frame ID No: 4
            lstSendData.Add(0xF8);
            // 3.4. Access Route : 7 byte: 5-11
            // 3.4.1. Station No:
            lstSendData.Add((byte)this.stationNo);
            // 3.4.2. NetWork No:
            lstSendData.Add((byte)this.netWorkNo);
            // 3.4.3. PC No:
            lstSendData.Add((byte)this.pcNo);
            // 3.4.4. IO No:
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            // 3.4.5. Request Station:
            lstSendData.Add((byte)this.reqStationNo);
            //3.4.6. Seft Station :
            lstSendData.Add((byte)this.seftStationNo);

            //3.5. Request Data
            //3.5.1. Command: 1401
            lstSendData.Add(0x01);
            lstSendData.Add(0x14);
            //3.5.2. SubCommand: 0000
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);
            //3.5.3. Bit số bao nhiêu:
            // Tách thành các byte
            byte[] arrDevNumber = BitConverter.GetBytes(_devNumber);
            // Thêm 3 byte vào danh sách gửi đi

            lstSendData.Add(arrDevNumber[0]);
            lstSendData.Add(arrDevNumber[1]);
            lstSendData.Add(arrDevNumber[2]);
            // 3.5.4. Device Code:
            lstSendData.Add((byte)_devCode);

            //3.5.5. Number of Device Point:
            int devNumPoint = _lstValue.Count;
            byte[] arrDevNumPoint = BitConverter.GetBytes(devNumPoint);
            lstSendData.Add(arrDevNumPoint[0]);
            lstSendData.Add(arrDevNumPoint[1]);

            //3.5.6. Data:
            for (int i = 0; i < _lstValue.Count; i++)
            {
                byte[] arrData = BitConverter.GetBytes(_lstValue[i]);
                lstSendData.Add(arrData[0]);
                lstSendData.Add(arrData[1]);
            }
            
            //3.6. Sửa lại number of Data byte:
            int numByte = lstSendData.Count - 4;
            byte[] arrNumByte = BitConverter.GetBytes(numByte);

            lstSendData[2] = arrNumByte[0];
            lstSendData[3] = arrNumByte[1];

            //3.7 Tính sumcheck code:
            // 3.7.1. Tính tổng từ byte 2 đến hết
            int sum = 0;
            for (int i = 2; i < lstSendData.Count; i++)
            {
                sum += lstSendData[i];
            }
            // sumcheck code không tính DLE:
            //  sum = sum - 0x10;
            //3.7.2. Chuyển tổng từ số nguyên thành kí tự:
            string strSum = sum.ToString("X");
            //3.7.2. Thêm kí tự 0 vào nếu chuỗi không đủ 2 kí tự:
            strSum = strSum.PadLeft(2, '0');
            // Hàm padleft là hàm điền kí tự yêu cầu sang bên trái
            // cho đủ số lượng kí tự yêu cầu
            // Ví dụ ban đầu strSum = "1", thì sau chạy lệnh strSum = "01";
            // Trong trường hợp strSum nhiều hơn 2 kí tự thì cần cắt ra 2 kí tự bên phải
            strSum = strSum.Substring(strSum.Length - 2, 2);
            //3.7.3. Chuyển chuỗi string về dạng ASCII:
            byte[] arrSum = ASCIIEncoding.ASCII.GetBytes(strSum);

            // 3.8. Control Code: 
            lstSendData.Add(0x10); //DLE
            lstSendData.Add(0x03); // EOT

            //3.9. Thêm sumcheck code ở mục 3.7 vào:
            lstSendData.Add(arrSum[0]);
            lstSendData.Add(arrSum[1]);

            // B4: Gửi dữ liệu đi:
            this.port.DiscardInBuffer();
            this.port.Write(lstSendData.ToArray(), 0, lstSendData.Count);

            // B5: Nhận dữ liệu về từ PLC:
            int timeOut = 1000;//ms = 1s
            // Đợi đến bao giờ đủ số lượng byte mong muốn: ít nhất 20 byte
            int expectBytes = 20;
            while (timeOut > 0)
            {
                if (this.port.BytesToRead >= expectBytes)
                {
                    break;
                }
                timeOut = timeOut - 10;// Mỗi lần trừ đi 10ms
                Thread.Sleep(10);// Dừng trong 10ms
            }
            // Vị trí này là thoát khỏi while ************
            if (timeOut <= 0)
            {
                return kq;
            }
            // Nhận dữ liệu:
            byte[] arrRcv = new byte[1024];
            this.port.Read(arrRcv, 0, this.port.BytesToRead);
            List<byte> lstRcv = new List<byte>();
            lstRcv.AddRange(arrRcv);

            // Bước 6: Phân tích dữ liệu nhận về từ PLC:
            // 6.1. Control Code:
            if (lstRcv[0] != 0x10 || lstRcv[1] != 0x02)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);// Remove từ phần tử số 0 và remove đi 2 phần tử
            //6.2. Kiểm tra xem number of databyte là bao nhiêu:
            short numberOfDataByte = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (numberOfDataByte < 12)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.3. Frame ID No:
            if (lstRcv[0] != 0xF8)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.4. Access Route:
            //6.4.1 Station No
            if (lstRcv[0] != (byte)this.stationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.2. NetWorkNo:
            if (lstRcv[0] != (byte)this.netWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.3. PCNo:
            if (lstRcv[0] != (byte)this.pcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.4. Request Destination Module IO No:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.4.5. Request Station No:
            if (lstRcv[0] != (byte)this.reqStationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.6. Seft Station No:
            if (lstRcv[0] != (byte)this.seftStationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            // 6.5. Response ID Code:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0xFF)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            // 6.6. Normal Completition:
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            kq = true;
            return kq;
        }
        public bool WriteDoubleWord(DevCode _devCode, int _devNumber, int _value)
        {
            bool kq = false;
            
            //B1: Kiểm tra port đã khởi tạo chưa:
            if (this.port == null)
            {
                return kq;
            }
            //B2: Kiểm tra port đã mở chưa
            if (this.port.IsOpen == false)
            {
                return kq;
            }
            //B3: Chuẩn bị dữ liệu gửi xuống:
            List<byte> lstSendData = new List<byte>();
            //3.1. Control code: 0-1
            lstSendData.Add(0x10);// DLE
            lstSendData.Add(0x02);// STX
            //3.2. Number of databyte : 2-3
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);
            // 3.3. Frame ID No: 4
            lstSendData.Add(0xF8);
            // 3.4. Access Route : 7 byte: 5-11
            // 3.4.1. Station No:
            lstSendData.Add((byte)this.stationNo);
            // 3.4.2. NetWork No:
            lstSendData.Add((byte)this.netWorkNo);
            // 3.4.3. PC No:
            lstSendData.Add((byte)this.pcNo);
            // 3.4.4. IO No:
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            // 3.4.5. Request Station:
            lstSendData.Add((byte)this.reqStationNo);
            //3.4.6. Seft Station :
            lstSendData.Add((byte)this.seftStationNo);

            //3.5. Request Data
            //3.5.1. Command: 1401
            lstSendData.Add(0x01);
            lstSendData.Add(0x14);
            //3.5.2. SubCommand: 0000
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);
            //3.5.3. Bit số bao nhiêu:
            // Tách thành các byte
            byte[] arrDevNumber = BitConverter.GetBytes(_devNumber);
            // Thêm 3 byte vào danh sách gửi đi

            lstSendData.Add(arrDevNumber[0]);
            lstSendData.Add(arrDevNumber[1]);
            lstSendData.Add(arrDevNumber[2]);
            // 3.5.4. Device Code:
            lstSendData.Add((byte)_devCode);

            //3.5.5. Number of Device Point:
            int devNumPoint = 2;
            byte[] arrDevNumPoint = BitConverter.GetBytes(devNumPoint);
            lstSendData.Add(arrDevNumPoint[0]);
            lstSendData.Add(arrDevNumPoint[1]);

            //3.5.6. Data:
                byte[] arrData = BitConverter.GetBytes(_value);
                lstSendData.Add(arrData[0]);
                lstSendData.Add(arrData[1]);
                lstSendData.Add(arrData[2]);
                lstSendData.Add(arrData[3]);

            //3.6. Sửa lại number of Data byte:
            int numByte = lstSendData.Count - 4;
            byte[] arrNumByte = BitConverter.GetBytes(numByte);

            lstSendData[2] = arrNumByte[0];
            lstSendData[3] = arrNumByte[1];

            //3.7 Tính sumcheck code:
            // 3.7.1. Tính tổng từ byte 2 đến hết
            int sum = 0;
            for (int i = 2; i < lstSendData.Count; i++)
            {
                sum += lstSendData[i];
            }
            // sumcheck code không tính DLE:
            //  sum = sum - 0x10;
            //3.7.2. Chuyển tổng từ số nguyên thành kí tự:
            string strSum = sum.ToString("X");
            //3.7.2. Thêm kí tự 0 vào nếu chuỗi không đủ 2 kí tự:
            strSum = strSum.PadLeft(2, '0');
            // Hàm padleft là hàm điền kí tự yêu cầu sang bên trái
            // cho đủ số lượng kí tự yêu cầu
            // Ví dụ ban đầu strSum = "1", thì sau chạy lệnh strSum = "01";
            // Trong trường hợp strSum nhiều hơn 2 kí tự thì cần cắt ra 2 kí tự bên phải
            strSum = strSum.Substring(strSum.Length - 2, 2);
            //3.7.3. Chuyển chuỗi string về dạng ASCII:
            byte[] arrSum = ASCIIEncoding.ASCII.GetBytes(strSum);

            // 3.8. Control Code: 
            lstSendData.Add(0x10); //DLE
            lstSendData.Add(0x03); // EOT

            //3.9. Thêm sumcheck code ở mục 3.7 vào:
            lstSendData.Add(arrSum[0]);
            lstSendData.Add(arrSum[1]);

            // B4: Gửi dữ liệu đi:
            this.port.DiscardInBuffer();
            this.port.Write(lstSendData.ToArray(), 0, lstSendData.Count);

            // B5: Nhận dữ liệu về từ PLC:
            int timeOut = 1000;//ms = 1s
            // Đợi đến bao giờ đủ số lượng byte mong muốn: ít nhất 20 byte
            int expectBytes = 20;
            while (timeOut > 0)
            {
                if (this.port.BytesToRead >= expectBytes)
                {
                    break;
                }
                timeOut = timeOut - 10;// Mỗi lần trừ đi 10ms
                Thread.Sleep(10);// Dừng trong 10ms
            }
            // Vị trí này là thoát khỏi while ************
            if (timeOut <= 0)
            {
                return kq;
            }
            // Nhận dữ liệu:
            byte[] arrRcv = new byte[1024];
            this.port.Read(arrRcv, 0, this.port.BytesToRead);
            List<byte> lstRcv = new List<byte>();
            lstRcv.AddRange(arrRcv);

            // Bước 6: Phân tích dữ liệu nhận về từ PLC:
            // 6.1. Control Code:
            if (lstRcv[0] != 0x10 || lstRcv[1] != 0x02)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);// Remove từ phần tử số 0 và remove đi 2 phần tử
            //6.2. Kiểm tra xem number of databyte là bao nhiêu:
            short numberOfDataByte = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (numberOfDataByte < 12)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.3. Frame ID No:
            if (lstRcv[0] != 0xF8)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.4. Access Route:
            //6.4.1 Station No
            if (lstRcv[0] != (byte)this.stationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.2. NetWorkNo:
            if (lstRcv[0] != (byte)this.netWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.3. PCNo:
            if (lstRcv[0] != (byte)this.pcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.4. Request Destination Module IO No:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.4.5. Request Station No:
            if (lstRcv[0] != (byte)this.reqStationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.6. Seft Station No:
            if (lstRcv[0] != (byte)this.seftStationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            // 6.5. Response ID Code:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0xFF)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            // 6.6. Normal Completition:
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            kq = true;
            return kq;
        }
        public bool WriteFloat(DevCode _devCode, int _devNumber, float _value)
        {
            bool kq = false;

            //B1: Kiểm tra port đã khởi tạo chưa:
            if (this.port == null)
            {
                return kq;
            }
            //B2: Kiểm tra port đã mở chưa
            if (this.port.IsOpen == false)
            {
                return kq;
            }
            //B3: Chuẩn bị dữ liệu gửi xuống:
            List<byte> lstSendData = new List<byte>();
            //3.1. Control code: 0-1
            lstSendData.Add(0x10);// DLE
            lstSendData.Add(0x02);// STX
            //3.2. Number of databyte : 2-3
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);
            // 3.3. Frame ID No: 4
            lstSendData.Add(0xF8);
            // 3.4. Access Route : 7 byte: 5-11
            // 3.4.1. Station No:
            lstSendData.Add((byte)this.stationNo);
            // 3.4.2. NetWork No:
            lstSendData.Add((byte)this.netWorkNo);
            // 3.4.3. PC No:
            lstSendData.Add((byte)this.pcNo);
            // 3.4.4. IO No:
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            // 3.4.5. Request Station:
            lstSendData.Add((byte)this.reqStationNo);
            //3.4.6. Seft Station :
            lstSendData.Add((byte)this.seftStationNo);

            //3.5. Request Data
            //3.5.1. Command: 1401
            lstSendData.Add(0x01);
            lstSendData.Add(0x14);
            //3.5.2. SubCommand: 0000
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);
            //3.5.3. Bit số bao nhiêu:
            // Tách thành các byte
            byte[] arrDevNumber = BitConverter.GetBytes(_devNumber);
            // Thêm 3 byte vào danh sách gửi đi

            lstSendData.Add(arrDevNumber[0]);
            lstSendData.Add(arrDevNumber[1]);
            lstSendData.Add(arrDevNumber[2]);
            // 3.5.4. Device Code:
            lstSendData.Add((byte)_devCode);

            //3.5.5. Number of Device Point:
            int devNumPoint = 2;
            byte[] arrDevNumPoint = BitConverter.GetBytes(devNumPoint);
            lstSendData.Add(arrDevNumPoint[0]);
            lstSendData.Add(arrDevNumPoint[1]);

            //3.5.6. Data:
            byte[] arrData = BitConverter.GetBytes(_value);
            lstSendData.Add(arrData[0]);
            lstSendData.Add(arrData[1]);
            lstSendData.Add(arrData[2]);
            lstSendData.Add(arrData[3]);

            //3.6. Sửa lại number of Data byte:
            int numByte = lstSendData.Count - 4;
            byte[] arrNumByte = BitConverter.GetBytes(numByte);

            lstSendData[2] = arrNumByte[0];
            lstSendData[3] = arrNumByte[1];

            //3.7 Tính sumcheck code:
            // 3.7.1. Tính tổng từ byte 2 đến hết
            int sum = 0;
            for (int i = 2; i < lstSendData.Count; i++)
            {
                sum += lstSendData[i];
            }
            // sumcheck code không tính DLE:
            //  sum = sum - 0x10;
            //3.7.2. Chuyển tổng từ số nguyên thành kí tự:
            string strSum = sum.ToString("X");
            //3.7.2. Thêm kí tự 0 vào nếu chuỗi không đủ 2 kí tự:
            strSum = strSum.PadLeft(2, '0');
            // Hàm padleft là hàm điền kí tự yêu cầu sang bên trái
            // cho đủ số lượng kí tự yêu cầu
            // Ví dụ ban đầu strSum = "1", thì sau chạy lệnh strSum = "01";
            // Trong trường hợp strSum nhiều hơn 2 kí tự thì cần cắt ra 2 kí tự bên phải
            strSum = strSum.Substring(strSum.Length - 2, 2);
            //3.7.3. Chuyển chuỗi string về dạng ASCII:
            byte[] arrSum = ASCIIEncoding.ASCII.GetBytes(strSum);

            // 3.8. Control Code: 
            lstSendData.Add(0x10); //DLE
            lstSendData.Add(0x03); // EOT

            //3.9. Thêm sumcheck code ở mục 3.7 vào:
            lstSendData.Add(arrSum[0]);
            lstSendData.Add(arrSum[1]);

            // B4: Gửi dữ liệu đi:
            this.port.DiscardInBuffer();
            this.port.Write(lstSendData.ToArray(), 0, lstSendData.Count);

            // B5: Nhận dữ liệu về từ PLC:
            int timeOut = 1000;//ms = 1s
            // Đợi đến bao giờ đủ số lượng byte mong muốn: ít nhất 20 byte
            int expectBytes = 20;
            while (timeOut > 0)
            {
                if (this.port.BytesToRead >= expectBytes)
                {
                    break;
                }
                timeOut = timeOut - 10;// Mỗi lần trừ đi 10ms
                Thread.Sleep(10);// Dừng trong 10ms
            }
            // Vị trí này là thoát khỏi while ************
            if (timeOut <= 0)
            {
                return kq;
            }
            // Nhận dữ liệu:
            byte[] arrRcv = new byte[1024];
            this.port.Read(arrRcv, 0, this.port.BytesToRead);
            List<byte> lstRcv = new List<byte>();
            lstRcv.AddRange(arrRcv);

            // Bước 6: Phân tích dữ liệu nhận về từ PLC:
            // 6.1. Control Code:
            if (lstRcv[0] != 0x10 || lstRcv[1] != 0x02)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);// Remove từ phần tử số 0 và remove đi 2 phần tử
            //6.2. Kiểm tra xem number of databyte là bao nhiêu:
            short numberOfDataByte = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (numberOfDataByte < 12)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.3. Frame ID No:
            if (lstRcv[0] != 0xF8)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.4. Access Route:
            //6.4.1 Station No
            if (lstRcv[0] != (byte)this.stationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.2. NetWorkNo:
            if (lstRcv[0] != (byte)this.netWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.3. PCNo:
            if (lstRcv[0] != (byte)this.pcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.4. Request Destination Module IO No:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.4.5. Request Station No:
            if (lstRcv[0] != (byte)this.reqStationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.4.6. Seft Station No:
            if (lstRcv[0] != (byte)this.seftStationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            // 6.5. Response ID Code:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0xFF)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            // 6.6. Normal Completition:
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            kq = true;
            return kq;
        }
        #endregion


    }
}
