using PLCMonitorSystem.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PLCMonitorSystem
{
    public enum Device
    {
        SM = 0x91,
        SD = 0xA9,
        X = 0x9C,
        Y = 0x9D,
        M = 0x90,
        L = 0x92,
        D = 0xA8,
        Z = 0xCC,
        ZR = 0xB0
    }

    public class Sock_SLMP
    {
        // Property & Field
        Socket sock;
        private string ipAddress = UIManager.appSetting.SockSetting.IpAddr;
        private int port = UIManager.appSetting.SockSetting.Port;

        private int stationNo;
        private int netWorkNo;
        private int pcNo;

        public string IpAddress { get => ipAddress; set => ipAddress = value; }
        public int Port { get => port; set => port = value; }
        public int StationNo { get => stationNo; set => stationNo = value; }
        public int NetWorkNo { get => netWorkNo; set => netWorkNo = value; }
        public int PcNo { get => pcNo; set => pcNo = value; }

        // Method
        public Sock_SLMP()
        {
            //this.IpAddress = "192.168.3.39";
            //this.Port = 6000;
            this.NetWorkNo = 0x00;
            this.PcNo = 0xFF;
            this.StationNo = 0x00;
        }
        public Sock_SLMP(string ipAdress, int port)
        {
            this.IpAddress = ipAdress;
            this.Port = port;
            this.NetWorkNo = 0x00;
            this.PcNo = 0xFF;
            this.StationNo = 0x00;
        }
        public Sock_SLMP(int _netWorkNo, int _pcNo, int _stationNo)
        {
            this.IpAddress = "127.0.0.1";
            this.Port = 6000;
            this.NetWorkNo = _netWorkNo;
            this.PcNo = _pcNo;
            this.StationNo = _stationNo;
        }


        // Connect
        public int Connect()
        {
            int kq = -1;
            // B1: Kiểm tra Socket đã khởi tạo
            if (sock == null)
            {
                sock = new Socket(SocketType.Stream, ProtocolType.Tcp);
            }
            // B2: Kiểm tra đã kết nối
            if (sock.Connected == true)
            {
                kq = 0;
                return kq;
            }
            // B3: Kết nối
            try
            {
                sock.Connect(this.IpAddress, this.Port);
                if (sock.Connected == true)
                {
                    kq = 0;
                }
            }
            catch (Exception err) { }

            return kq;
        }
        public int Disconnect()
        {
            int kq = -1;
            // B1: Kiểm tra Socket đã khởi tạo
            if (sock == null)
            {
                return kq;
            }
            if (sock.Connected == false)
            {
                return kq;
            }
            try
            {
                sock.Disconnect(false);
                sock = null;
                kq = 0;
            }
            catch (Exception err) { }

            return kq;
        }

        public short ReadWord(Device _devCode, int _devNumber)
        {
            short kq = 0;
            // B1: Kiểm tra đã khởi tạo
            if (sock == null)
            {
                return kq;
            }
            // B2: Kiểm tra đã kết nối
            if (sock.Connected == false)
            {
                return kq;
            }

            // B3: Chuẩn bị data
            List<byte> lstSendData = new List<byte>();
            // 3.1 Header: Đã tự động thêm vào <Skip>
            // 3.2 Subheader [2byte]
            lstSendData.Add(0x50);
            lstSendData.Add(0x00);

            // 3.3 Access Route
            // 3.3.1 Network No
            lstSendData.Add((byte)this.NetWorkNo);
            // 3.3.2 PC No
            lstSendData.Add((byte)this.PcNo);
            // 3.3.3 Request Destination Module IO
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            // 3.3.4 Request Destination Module Station
            lstSendData.Add((byte)this.StationNo);

            // 3.4 Request Data Length [byte7 - byte8]
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);

            // 3.5 Monitoring Time
            lstSendData.Add(0x10);
            lstSendData.Add(0x00);

            // 3.6 Request Data
            // 3.6.1 Command [0401]
            lstSendData.Add(0x01);
            lstSendData.Add(0x04);
            // 3.6.2 Sub command [0000]
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);
            // 3.6.3 Head Device Number
            byte[] headDevice = BitConverter.GetBytes(_devNumber);
            lstSendData.Add(headDevice[0]);
            lstSendData.Add(headDevice[1]);
            lstSendData.Add(headDevice[2]);
            // 3.6.4 Device Code
            lstSendData.Add((byte)_devCode);
            // 3.6.5 Number Of Device Point
            int devPoint = 1;
            byte[] arrDevPoint = BitConverter.GetBytes(devPoint);
            lstSendData.Add(arrDevPoint[0]);
            lstSendData.Add(arrDevPoint[1]);

            // 3.7 Tính lại Request Data Length
            int reDataL = lstSendData.Count - 2 - 5 - 2;
            byte[] arrReqData = BitConverter.GetBytes(reDataL);
            lstSendData[7] = arrReqData[0];
            lstSendData[8] = arrReqData[1];
            // B4: Send Data
            sock.Send(lstSendData.ToArray());

            // B5: Recieve Data
            byte[] arrRcv = new byte[512];
            List<byte> lstRcv = new List<byte>();
            sock.Receive(arrRcv);
            lstRcv.AddRange(arrRcv);

            // B6: Phân tích data nhận về
            // 6.1 Sub Header
            if (lstRcv[0] != 0xD0 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.2 Access Route
            // 6.2.1 Network No
            if (lstRcv[0] != (byte)this.NetWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.2 PC No
            if (lstRcv[0] != (byte)this.PcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.3 Request Destination Module IO
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            // 6.2.4 Request Station No
            if (lstRcv[0] != (byte)this.StationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            // 6.3 Request Data Length
            short reqDataLength = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (reqDataLength < 2)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.4 End Code
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.5 Data
            kq = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            return kq;
        }
        public int ReadDWord(Device _devCode, int _devNumber)
        {
            int kq = 0;
            // B1: Kiểm tra đã khởi tạo
            if (sock == null)
            {
                return kq;
            }
            // B2: Kiểm tra đã kết nối
            if (sock.Connected == false)
            {
                return kq;
            }

            // B3: Chuẩn bị data
            List<byte> lstSendData = new List<byte>();
            // 3.1 Header: Đã tự động thêm vào <Skip>
            // 3.2 Subheader [2byte]
            lstSendData.Add(0x50);
            lstSendData.Add(0x00);

            // 3.3 Access Route
            // 3.3.1 Network No
            lstSendData.Add((byte)this.NetWorkNo);
            // 3.3.2 PC No
            lstSendData.Add((byte)this.PcNo);
            // 3.3.3 Request Destination Module IO
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            // 3.3.4 Request Destination Module Station
            lstSendData.Add((byte)this.StationNo);

            // 3.4 Request Data Length [byte7 - byte8]
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);

            // 3.5 Monitoring Time
            lstSendData.Add(0x10);
            lstSendData.Add(0x00);

            // 3.6 Request Data
            // 3.6.1 Command [0401]
            lstSendData.Add(0x01);
            lstSendData.Add(0x04);
            // 3.6.2 Sub command [0000]
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);
            // 3.6.3 Head Device Number
            byte[] headDevice = BitConverter.GetBytes(_devNumber);
            lstSendData.Add(headDevice[0]);
            lstSendData.Add(headDevice[1]);
            lstSendData.Add(headDevice[2]);
            // 3.6.4 Device Code
            lstSendData.Add((byte)_devCode);
            // 3.6.5 Number Of Device Point
            int devPoint = 2;
            byte[] arrDevPoint = BitConverter.GetBytes(devPoint);
            lstSendData.Add(arrDevPoint[0]);
            lstSendData.Add(arrDevPoint[1]);

            // 3.7 Tính lại Request Data Length
            int reDataL = lstSendData.Count - 2 - 5 - 2;
            byte[] arrReqData = BitConverter.GetBytes(reDataL);
            lstSendData[7] = arrReqData[0];
            lstSendData[8] = arrReqData[1];


            // B4: Send Data
            sock.Send(lstSendData.ToArray());

            // B5: Recieve Data
            byte[] arrRcv = new byte[512];
            List<byte> lstRcv = new List<byte>();
            sock.Receive(arrRcv);
            lstRcv.AddRange(arrRcv);

            // B6: Phân tích data nhận về
            // 6.1 Sub Header
            if (lstRcv[0] != 0xD0 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.2 Access Route
            // 6.2.1 Network No
            if (lstRcv[0] != (byte)this.NetWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.2 PC No
            if (lstRcv[0] != (byte)this.PcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.3 Request Destination Module IO
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            // 6.2.4 Request Station No
            if (lstRcv[0] != (byte)this.StationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            // 6.3 Request Data Length
            short reqDataLength = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (reqDataLength < 2)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.4 End Code
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.5 Data
            kq = BitConverter.ToInt32(new byte[] { lstRcv[0], lstRcv[1], lstRcv[2], lstRcv[3] }, 0);
            return kq;
        }
        public float ReadFLoat(Device _devCode, int _devNumber)
        {
            float kq = 0f;
            // B1: Kiểm tra đã khởi tạo
            if (sock == null)
            {
                return kq;
            }
            // B2: Kiểm tra đã kết nối
            if (sock.Connected == false)
            {
                return kq;
            }

            // B3: Chuẩn bị data
            List<byte> lstSendData = new List<byte>();
            // 3.1 Header: Đã tự động thêm vào <Skip>
            // 3.2 Subheader [2byte]
            lstSendData.Add(0x50);
            lstSendData.Add(0x00);

            // 3.3 Access Route
            // 3.3.1 Network No
            lstSendData.Add((byte)this.NetWorkNo);
            // 3.3.2 PC No
            lstSendData.Add((byte)this.PcNo);
            // 3.3.3 Request Destination Module IO
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            // 3.3.4 Request Destination Module Station
            lstSendData.Add((byte)this.StationNo);

            // 3.4 Request Data Length [byte7 - byte8]
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);

            // 3.5 Monitoring Time
            lstSendData.Add(0x10);
            lstSendData.Add(0x00);

            // 3.6 Request Data
            // 3.6.1 Command [0401]
            lstSendData.Add(0x01);
            lstSendData.Add(0x04);
            // 3.6.2 Sub command [0000]
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);
            // 3.6.3 Head Device Number
            byte[] headDevice = BitConverter.GetBytes(_devNumber);
            lstSendData.Add(headDevice[0]);
            lstSendData.Add(headDevice[1]);
            lstSendData.Add(headDevice[2]);
            // 3.6.4 Device Code
            lstSendData.Add((byte)_devCode);
            // 3.6.5 Number Of Device Point
            int devPoint = 2;
            byte[] arrDevPoint = BitConverter.GetBytes(devPoint);
            lstSendData.Add(arrDevPoint[0]);
            lstSendData.Add(arrDevPoint[1]);

            // 3.7 Tính lại Request Data Length
            int reDataL = lstSendData.Count - 2 - 5 - 2;
            byte[] arrReqData = BitConverter.GetBytes(reDataL);
            lstSendData[7] = arrReqData[0];
            lstSendData[8] = arrReqData[1];


            // B4: Send Data
            sock.Send(lstSendData.ToArray());

            // B5: Recieve Data
            byte[] arrRcv = new byte[512];
            List<byte> lstRcv = new List<byte>();
            sock.Receive(arrRcv);
            lstRcv.AddRange(arrRcv);

            // B6: Phân tích data nhận về
            // 6.1 Sub Header
            if (lstRcv[0] != 0xD0 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.2 Access Route
            // 6.2.1 Network No
            if (lstRcv[0] != (byte)this.NetWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.2 PC No
            if (lstRcv[0] != (byte)this.PcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.3 Request Destination Module IO
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            // 6.2.4 Request Station No
            if (lstRcv[0] != (byte)this.StationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            // 6.3 Request Data Length
            short reqDataLength = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (reqDataLength < 2)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.4 End Code
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.5 Data
            kq = BitConverter.ToSingle(new byte[] { lstRcv[0], lstRcv[1], lstRcv[2], lstRcv[3] }, 0);
            return kq;
        }

        public bool ReadBit(Device _devCode, int _devNumber)
        {
            bool kq = false;
            // B1: Kiểm tra đã khởi tạo
            if (sock == null)
            {
                return kq;
            }
            // B2: Kiểm tra đã kết nối
            if (sock.Connected == false)
            {
                return kq;
            }

            // B3: Chuẩn bị data
            List<byte> lstSendData = new List<byte>();
            // 3.1 Header: Đã tự động thêm vào <Skip>
            // 3.2 Subheader [2byte]
            lstSendData.Add(0x50);
            lstSendData.Add(0x00);

            // 3.3 Access Route
            // 3.3.1 Network No
            lstSendData.Add((byte)this.NetWorkNo);
            // 3.3.2 PC No
            lstSendData.Add((byte)this.PcNo);
            // 3.3.3 Request Destination Module IO
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            // 3.3.4 Request Destination Module Station
            lstSendData.Add((byte)this.StationNo);

            // 3.4 Request Data Length [byte7 - byte8]
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);

            // 3.5 Monitoring Time
            lstSendData.Add(0x10);
            lstSendData.Add(0x00);

            // 3.6 Request Data
            // 3.6.1 Command [0401]
            lstSendData.Add(0x01);
            lstSendData.Add(0x04);
            // 3.6.2 Sub command [0001]
            lstSendData.Add(0x01);
            lstSendData.Add(0x00);
            // 3.6.3 Head Device Number
            byte[] headDevice = BitConverter.GetBytes(_devNumber);
            lstSendData.Add(headDevice[0]);
            lstSendData.Add(headDevice[1]);
            lstSendData.Add(headDevice[2]);
            // 3.6.4 Device Code
            lstSendData.Add((byte)_devCode);
            // 3.6.5 Number Of Device Point
            int devPoint = 1;
            byte[] arrDevPoint = BitConverter.GetBytes(devPoint);
            lstSendData.Add(arrDevPoint[0]);
            lstSendData.Add(arrDevPoint[1]);

            // 3.7 Tính lại Request Data Length
            int reDataL = lstSendData.Count - 2 - 5 - 2;
            byte[] arrReqData = BitConverter.GetBytes(reDataL);
            lstSendData[7] = arrReqData[0];
            lstSendData[8] = arrReqData[1];
            // B4: Send Data
            sock.Send(lstSendData.ToArray());

            // B5: Recieve Data
            byte[] arrRcv = new byte[512];
            List<byte> lstRcv = new List<byte>();
            sock.Receive(arrRcv);
            lstRcv.AddRange(arrRcv);

            // B6: Phân tích data nhận về
            // 6.1 Sub Header
            if (lstRcv[0] != 0xD0 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.2 Access Route
            // 6.2.1 Network No
            if (lstRcv[0] != (byte)this.NetWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.2 PC No
            if (lstRcv[0] != (byte)this.PcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.3 Request Destination Module IO
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            // 6.2.4 Request Station No
            if (lstRcv[0] != (byte)this.StationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            // 6.3 Request Data Length
            short reqDataLength = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (reqDataLength < 2)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.4 End Code
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.5 Data
            if (lstRcv[0] != 0)
            {
                kq = true;
            }
            return kq;
        }
        public List<bool> ReadMultiBit(Device _devCode, int _devNumber, int _count)
        {
            List<bool> kq = new List<bool>();
            // B1: Kiểm tra đã khởi tạo
            if (sock == null)
            {
                return kq;
            }
            // B2: Kiểm tra đã kết nối
            if (sock.Connected == false)
            {
                return kq;
            }

            // B3: Chuẩn bị data
            List<byte> lstSendData = new List<byte>();
            // 3.1 Header: Đã tự động thêm vào <Skip>
            // 3.2 Subheader [2byte]
            lstSendData.Add(0x50);
            lstSendData.Add(0x00);

            // 3.3 Access Route
            // 3.3.1 Network No
            lstSendData.Add((byte)this.NetWorkNo);
            // 3.3.2 PC No
            lstSendData.Add((byte)this.PcNo);
            // 3.3.3 Request Destination Module IO
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            // 3.3.4 Request Destination Module Station
            lstSendData.Add((byte)this.StationNo);

            // 3.4 Request Data Length [byte7 - byte8]
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);

            // 3.5 Monitoring Time
            lstSendData.Add(0x10);
            lstSendData.Add(0x00);

            // 3.6 Request Data
            // 3.6.1 Command [0401]
            lstSendData.Add(0x01);
            lstSendData.Add(0x04);
            // 3.6.2 Sub command [0001]
            lstSendData.Add(0x01);
            lstSendData.Add(0x00);
            // 3.6.3 Head Device Number
            byte[] headDevice = BitConverter.GetBytes(_devNumber);
            lstSendData.Add(headDevice[0]);
            lstSendData.Add(headDevice[1]);
            lstSendData.Add(headDevice[2]);
            // 3.6.4 Device Code
            lstSendData.Add((byte)_devCode);
            // 3.6.5 Number Of Device Point
            if (_count <= 0) { return kq; }
            int devPoint = _count;
            byte[] arrDevPoint = BitConverter.GetBytes(devPoint);
            lstSendData.Add(arrDevPoint[0]);
            lstSendData.Add(arrDevPoint[1]);

            // 3.7 Tính lại Request Data Length
            int reDataL = lstSendData.Count - 2 - 5 - 2;
            byte[] arrReqData = BitConverter.GetBytes(reDataL);
            lstSendData[7] = arrReqData[0];
            lstSendData[8] = arrReqData[1];
            // B4: Send Data
            sock.Send(lstSendData.ToArray());

            // B5: Recieve Data
            byte[] arrRcv = new byte[512];
            List<byte> lstRcv = new List<byte>();
            sock.Receive(arrRcv);
            lstRcv.AddRange(arrRcv);

            // B6: Phân tích data nhận về
            // 6.1 Sub Header
            if (lstRcv[0] != 0xD0 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.2 Access Route
            // 6.2.1 Network No
            if (lstRcv[0] != (byte)this.NetWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.2 PC No
            if (lstRcv[0] != (byte)this.PcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.3 Request Destination Module IO
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            // 6.2.4 Request Station No
            if (lstRcv[0] != (byte)this.StationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            // 6.3 Request Data Length
            short reqDataLength = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (reqDataLength < 2)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.4 End Code
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.5 Data
            int byteCount = (_count / 2) + (_count % 2);
            // Ví dụ đọc 5 bit > count = 5
            // Số byte cần xử lý là 2 + 1 = 3 byte
            for (int i = 0; i < byteCount; i++)
            {
                if (lstRcv[i] == 0x00)
                {
                    kq.Add(false);
                    kq.Add(false);
                }
                else if (lstRcv[i] == 0x10)
                {
                    kq.Add(true);
                    kq.Add(false);
                }
                else if (lstRcv[i] == 0x01)
                {
                    kq.Add(false);
                    kq.Add(true);
                }
                else
                {
                    kq.Add(true);
                    kq.Add(true);
                }
            }
            return kq;
        }
        public List<short> ReadMultiWord(Device _devCode, int _devNumber, int _count)
        {
            List<short> kq = new List<short>();
            // B1: Kiểm tra đã khởi tạo
            if (sock == null)
            {
                return kq;
            }
            // B2: Kiểm tra đã kết nối
            if (sock.Connected == false)
            {
                return kq;
            }

            // B3: Chuẩn bị data
            List<byte> lstSendData = new List<byte>();
            // 3.1 Header: Đã tự động thêm vào <Skip>
            // 3.2 Subheader [2byte]
            lstSendData.Add(0x50);
            lstSendData.Add(0x00);

            // 3.3 Access Route
            // 3.3.1 Network No
            lstSendData.Add((byte)this.NetWorkNo);
            // 3.3.2 PC No
            lstSendData.Add((byte)this.PcNo);
            // 3.3.3 Request Destination Module IO
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            // 3.3.4 Request Destination Module Station
            lstSendData.Add((byte)this.StationNo);

            // 3.4 Request Data Length [byte7 - byte8]
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);

            // 3.5 Monitoring Time
            lstSendData.Add(0x10);
            lstSendData.Add(0x00);

            // 3.6 Request Data
            // 3.6.1 Command [0401]
            lstSendData.Add(0x01);
            lstSendData.Add(0x04);
            // 3.6.2 Sub command [0000]
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);
            // 3.6.3 Head Device Number
            byte[] headDevice = BitConverter.GetBytes(_devNumber);
            lstSendData.Add(headDevice[0]);
            lstSendData.Add(headDevice[1]);
            lstSendData.Add(headDevice[2]);
            // 3.6.4 Device Code
            lstSendData.Add((byte)_devCode);
            // 3.6.5 Number Of Device Point
            if (_count <= 0) { return kq; }
            int devPoint = _count;
            byte[] arrDevPoint = BitConverter.GetBytes(devPoint);
            lstSendData.Add(arrDevPoint[0]);
            lstSendData.Add(arrDevPoint[1]);

            // 3.7 Tính lại Request Data Length
            int reDataL = lstSendData.Count - 2 - 5 - 2;
            byte[] arrReqData = BitConverter.GetBytes(reDataL);
            lstSendData[7] = arrReqData[0];
            lstSendData[8] = arrReqData[1];
            // B4: Send Data
            sock.Send(lstSendData.ToArray());

            // B5: Recieve Data
            byte[] arrRcv = new byte[512];
            List<byte> lstRcv = new List<byte>();
            sock.Receive(arrRcv);
            lstRcv.AddRange(arrRcv);

            // B6: Phân tích data nhận về
            // 6.1 Sub Header
            if (lstRcv[0] != 0xD0 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.2 Access Route
            // 6.2.1 Network No
            if (lstRcv[0] != (byte)this.NetWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.2 PC No
            if (lstRcv[0] != (byte)this.PcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.3 Request Destination Module IO
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            // 6.2.4 Request Station No
            if (lstRcv[0] != (byte)this.StationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            // 6.3 Request Data Length
            short reqDataLength = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (reqDataLength < 2)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.4 End Code
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.5 Data
            int _byteCount = _count;
            for (int i = 0; i < _byteCount; i++)
            {
                kq.Add(BitConverter.ToInt16(new byte[] { lstRcv[2 * i], lstRcv[2 * i + 1] }, 0));

            }
            return kq;
        }
        public List<int> ReadMultiDWord(Device _devCode, int _devNumber, int _count)
        {
            List<int> kq = new List<int>();
            // B1: Kiểm tra đã khởi tạo
            if (sock == null)
            {
                return kq;
            }
            // B2: Kiểm tra đã kết nối
            if (sock.Connected == false)
            {
                return kq;
            }

            // B3: Chuẩn bị data
            List<byte> lstSendData = new List<byte>();
            // 3.1 Header: Đã tự động thêm vào <Skip>
            // 3.2 Subheader [2byte]
            lstSendData.Add(0x50);
            lstSendData.Add(0x00);

            // 3.3 Access Route
            // 3.3.1 Network No
            lstSendData.Add((byte)this.NetWorkNo);
            // 3.3.2 PC No
            lstSendData.Add((byte)this.PcNo);
            // 3.3.3 Request Destination Module IO
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            // 3.3.4 Request Destination Module Station
            lstSendData.Add((byte)this.StationNo);

            // 3.4 Request Data Length [byte7 - byte8]
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);

            // 3.5 Monitoring Time
            lstSendData.Add(0x10);
            lstSendData.Add(0x00);

            // 3.6 Request Data
            // 3.6.1 Command [0401]
            lstSendData.Add(0x01);
            lstSendData.Add(0x04);
            // 3.6.2 Sub command [0000]
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);
            // 3.6.3 Head Device Number
            byte[] headDevice = BitConverter.GetBytes(_devNumber);
            lstSendData.Add(headDevice[0]);
            lstSendData.Add(headDevice[1]);
            lstSendData.Add(headDevice[2]);
            // 3.6.4 Device Code
            lstSendData.Add((byte)_devCode);
            // 3.6.5 Number Of Device Point
            int devPoint = _count*2;
            byte[] arrDevPoint = BitConverter.GetBytes(devPoint);
            lstSendData.Add(arrDevPoint[0]);
            lstSendData.Add(arrDevPoint[1]);

            // 3.7 Tính lại Request Data Length
            int reDataL = lstSendData.Count - 2 - 5 - 2;
            byte[] arrReqData = BitConverter.GetBytes(reDataL);
            lstSendData[7] = arrReqData[0];
            lstSendData[8] = arrReqData[1];

            // B4: Send Data
            sock.Send(lstSendData.ToArray());

            // B5: Recieve Data
            byte[] arrRcv = new byte[1024];
            List<byte> lstRcv = new List<byte>();
            sock.Receive(arrRcv);
            lstRcv.AddRange(arrRcv);

            // B6: Phân tích data nhận về
            // 6.1 Sub Header
            if (lstRcv[0] != 0xD0 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.2 Access Route
            // 6.2.1 Network No
            if (lstRcv[0] != (byte)this.NetWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.2 PC No
            if (lstRcv[0] != (byte)this.PcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.3 Request Destination Module IO
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            // 6.2.4 Request Station No
            if (lstRcv[0] != (byte)this.StationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            // 6.3 Request Data Length
            short reqDataLength = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (reqDataLength < 2)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.4 End Code
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.5 Data
            int _byteCount = _count;
            for (int i = 0; i < _byteCount ; i++)
            {
                kq.Add(BitConverter.ToInt32(new byte[] { lstRcv[4 * i], lstRcv[4 * i + 1], lstRcv[4 * i + 2], lstRcv[4 * i + 3] }, 0));
            }
            return kq;
        }



        public short WriteWord(Device _devCode, int _devNumber, short _value)
        {
            short kq = -1;
            // B1: Kiểm tra đã khởi tạo
            if (sock == null)
            {
                return kq;
            }
            // B2: Kiểm tra đã kết nối
            if (sock.Connected == false)
            {
                return kq;
            }

            // B3: Chuẩn bị data
            List<byte> lstSendData = new List<byte>();
            // 3.1 Header: Đã tự động thêm vào <Skip>
            // 3.2 Subheader [2byte]
            lstSendData.Add(0x50);
            lstSendData.Add(0x00);

            // 3.3 Access Route
            // 3.3.1 Network No
            lstSendData.Add((byte)this.NetWorkNo);
            // 3.3.2 PC No
            lstSendData.Add((byte)this.PcNo);
            // 3.3.3 Request Destination Module IO
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            // 3.3.4 Request Destination Module Station
            lstSendData.Add((byte)this.StationNo);

            // 3.4 Request Data Length [byte7 - byte8]
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);

            // 3.5 Monitoring Time
            lstSendData.Add(0x10);
            lstSendData.Add(0x00);

            // 3.6 Request Data
            // 3.6.1 Command [1401]
            lstSendData.Add(0x01);
            lstSendData.Add(0x14);
            // 3.6.2 Sub command [0000]
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);
            // 3.6.3 Head Device Number
            byte[] headDevice = BitConverter.GetBytes(_devNumber);
            lstSendData.Add(headDevice[0]);
            lstSendData.Add(headDevice[1]);
            lstSendData.Add(headDevice[2]);
            // 3.6.4 Device Code
            lstSendData.Add((byte)_devCode);
            // 3.6.5 Number Of Device Point
            int devPoint = 1;
            byte[] arrDevPoint = BitConverter.GetBytes(devPoint);
            lstSendData.Add(arrDevPoint[0]);
            lstSendData.Add(arrDevPoint[1]);
            // 3.6.6 Data
            byte[] arrData = BitConverter.GetBytes(_value);
            lstSendData.Add(arrData[0]);
            lstSendData.Add(arrData[1]);

            // 3.7 Tính lại Request Data Length
            int reDataL = lstSendData.Count - 2 - 5 - 2;
            byte[] arrReqData = BitConverter.GetBytes(reDataL);
            lstSendData[7] = arrReqData[0];
            lstSendData[8] = arrReqData[1];
            // B4: Send Data
            sock.Send(lstSendData.ToArray());

            // B5: Recieve Data
            byte[] arrRcv = new byte[512];
            List<byte> lstRcv = new List<byte>();
            sock.Receive(arrRcv);
            lstRcv.AddRange(arrRcv);

            // B6: Phân tích data nhận về
            // 6.1 Sub Header
            if (lstRcv[0] != 0xD0 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.2 Access Route
            // 6.2.1 Network No
            if (lstRcv[0] != (byte)this.NetWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.2 PC No
            if (lstRcv[0] != (byte)this.PcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.3 Request Destination Module IO
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            // 6.2.4 Request Station No
            if (lstRcv[0] != (byte)this.StationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            // 6.3 Request Data Length
            short reqDataLength = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (reqDataLength < 2)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.4 End Code
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.5 Data
            kq = 0;
            return kq;
        }
        public int WriteDWord(Device _devCode, int _devNumber, int _value)
        {
            int kq = -1;
            // B1: Kiểm tra đã khởi tạo
            if (sock == null)
            {
                return kq;
            }
            // B2: Kiểm tra đã kết nối
            if (sock.Connected == false)
            {
                return kq;
            }

            // B3: Chuẩn bị data
            List<byte> lstSendData = new List<byte>();
            // 3.1 Header: Đã tự động thêm vào <Skip>
            // 3.2 Subheader [2byte]
            lstSendData.Add(0x50);
            lstSendData.Add(0x00);

            // 3.3 Access Route
            // 3.3.1 Network No
            lstSendData.Add((byte)this.NetWorkNo);
            // 3.3.2 PC No
            lstSendData.Add((byte)this.PcNo);
            // 3.3.3 Request Destination Module IO
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            // 3.3.4 Request Destination Module Station
            lstSendData.Add((byte)this.StationNo);

            // 3.4 Request Data Length [byte7 - byte8]
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);

            // 3.5 Monitoring Time
            lstSendData.Add(0x10);
            lstSendData.Add(0x00);

            // 3.6 Request Data
            // 3.6.1 Command [1401]
            lstSendData.Add(0x01);
            lstSendData.Add(0x14);
            // 3.6.2 Sub command [0000]
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);
            // 3.6.3 Head Device Number
            byte[] headDevice = BitConverter.GetBytes(_devNumber);
            lstSendData.Add(headDevice[0]);
            lstSendData.Add(headDevice[1]);
            lstSendData.Add(headDevice[2]);
            // 3.6.4 Device Code
            lstSendData.Add((byte)_devCode);
            // 3.6.5 Number Of Device Point
            int devPoint = 2;
            byte[] arrDevPoint = BitConverter.GetBytes(devPoint);
            lstSendData.Add(arrDevPoint[0]);
            lstSendData.Add(arrDevPoint[1]);
            // 3.6.6 Data
            byte[] arrData = BitConverter.GetBytes(_value);
            lstSendData.Add(arrData[0]);
            lstSendData.Add(arrData[1]);
            lstSendData.Add(arrData[2]);
            lstSendData.Add(arrData[3]);

            // 3.7 Tính lại Request Data Length
            int reDataL = lstSendData.Count - 2 - 5 - 2;
            byte[] arrReqData = BitConverter.GetBytes(reDataL);
            lstSendData[7] = arrReqData[0];
            lstSendData[8] = arrReqData[1];
            // B4: Send Data
            sock.Send(lstSendData.ToArray());

            // B5: Recieve Data
            byte[] arrRcv = new byte[512];
            List<byte> lstRcv = new List<byte>();
            sock.Receive(arrRcv);
            lstRcv.AddRange(arrRcv);

            // B6: Phân tích data nhận về
            // 6.1 Sub Header
            if (lstRcv[0] != 0xD0 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.2 Access Route
            // 6.2.1 Network No
            if (lstRcv[0] != (byte)this.NetWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.2 PC No
            if (lstRcv[0] != (byte)this.PcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.3 Request Destination Module IO
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            // 6.2.4 Request Station No
            if (lstRcv[0] != (byte)this.StationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            // 6.3 Request Data Length
            short reqDataLength = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (reqDataLength < 2)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.4 End Code
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.5 Data
            kq = 0;
            return kq;
        }
        public int WriteFloat(Device _devCode, int _devNumber, float _value)
        {
            int kq = -1;
            // B1: Kiểm tra đã khởi tạo
            if (sock == null)
            {
                return kq;
            }
            // B2: Kiểm tra đã kết nối
            if (sock.Connected == false)
            {
                return kq;
            }

            // B3: Chuẩn bị data
            List<byte> lstSendData = new List<byte>();
            // 3.1 Header: Đã tự động thêm vào <Skip>
            // 3.2 Subheader [2byte]
            lstSendData.Add(0x50);
            lstSendData.Add(0x00);

            // 3.3 Access Route
            // 3.3.1 Network No
            lstSendData.Add((byte)this.NetWorkNo);
            // 3.3.2 PC No
            lstSendData.Add((byte)this.PcNo);
            // 3.3.3 Request Destination Module IO
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            // 3.3.4 Request Destination Module Station
            lstSendData.Add((byte)this.StationNo);

            // 3.4 Request Data Length [byte7 - byte8]
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);

            // 3.5 Monitoring Time
            lstSendData.Add(0x10);
            lstSendData.Add(0x00);

            // 3.6 Request Data
            // 3.6.1 Command [1401]
            lstSendData.Add(0x01);
            lstSendData.Add(0x14);
            // 3.6.2 Sub command [0000]
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);
            // 3.6.3 Head Device Number
            byte[] headDevice = BitConverter.GetBytes(_devNumber);
            lstSendData.Add(headDevice[0]);
            lstSendData.Add(headDevice[1]);
            lstSendData.Add(headDevice[2]);
            // 3.6.4 Device Code
            lstSendData.Add((byte)_devCode);
            // 3.6.5 Number Of Device Point
            int devPoint = 2;
            byte[] arrDevPoint = BitConverter.GetBytes(devPoint);
            lstSendData.Add(arrDevPoint[0]);
            lstSendData.Add(arrDevPoint[1]);
            // 3.6.6 Data
            byte[] arrData = BitConverter.GetBytes(_value);
            lstSendData.Add(arrData[0]);
            lstSendData.Add(arrData[1]);
            lstSendData.Add(arrData[2]);
            lstSendData.Add(arrData[3]);

            // 3.7 Tính lại Request Data Length
            int reDataL = lstSendData.Count - 2 - 5 - 2;
            byte[] arrReqData = BitConverter.GetBytes(reDataL);
            lstSendData[7] = arrReqData[0];
            lstSendData[8] = arrReqData[1];
            // B4: Send Data
            sock.Send(lstSendData.ToArray());

            // B5: Recieve Data
            byte[] arrRcv = new byte[512];
            List<byte> lstRcv = new List<byte>();
            sock.Receive(arrRcv);
            lstRcv.AddRange(arrRcv);

            // B6: Phân tích data nhận về
            // 6.1 Sub Header
            if (lstRcv[0] != 0xD0 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.2 Access Route
            // 6.2.1 Network No
            if (lstRcv[0] != (byte)this.NetWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.2 PC No
            if (lstRcv[0] != (byte)this.PcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.3 Request Destination Module IO
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            // 6.2.4 Request Station No
            if (lstRcv[0] != (byte)this.StationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            // 6.3 Request Data Length
            short reqDataLength = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (reqDataLength < 2)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.4 End Code
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.5 Data
            kq = 0;
            return kq;
        }

        public int WriteBit(Device _devCode, int _devNumber, bool _value)
        {
            int kq = -1;
            // B1: Kiểm tra đã khởi tạo
            if (sock == null)
            {
                return kq;
            }
            // B2: Kiểm tra đã kết nối
            if (sock.Connected == false)
            {
                return kq;
            }

            // B3: Chuẩn bị data
            List<byte> lstSendData = new List<byte>();
            // 3.1 Header: Đã tự động thêm vào <Skip>
            // 3.2 Subheader [2byte]
            lstSendData.Add(0x50);
            lstSendData.Add(0x00);

            // 3.3 Access Route
            // 3.3.1 Network No
            lstSendData.Add((byte)this.NetWorkNo);
            // 3.3.2 PC No
            lstSendData.Add((byte)this.PcNo);
            // 3.3.3 Request Destination Module IO
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            // 3.3.4 Request Destination Module Station
            lstSendData.Add((byte)this.StationNo);

            // 3.4 Request Data Length [byte7 - byte8]
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);

            // 3.5 Monitoring Time
            lstSendData.Add(0x10);
            lstSendData.Add(0x00);

            // 3.6 Request Data
            // 3.6.1 Command [1401]
            lstSendData.Add(0x01);
            lstSendData.Add(0x14);
            // 3.6.2 Sub command [0001]
            lstSendData.Add(0x01);
            lstSendData.Add(0x00);
            // 3.6.3 Head Device Number
            byte[] headDevice = BitConverter.GetBytes(_devNumber);
            lstSendData.Add(headDevice[0]);
            lstSendData.Add(headDevice[1]);
            lstSendData.Add(headDevice[2]);
            // 3.6.4 Device Code
            lstSendData.Add((byte)_devCode);
            // 3.6.5 Number Of Device Point
            int devPoint = 1;
            byte[] arrDevPoint = BitConverter.GetBytes(devPoint);
            lstSendData.Add(arrDevPoint[0]);
            lstSendData.Add(arrDevPoint[1]);
            // 3.6.6 Data
            if (_value) { lstSendData.Add(0x10); }
            else { lstSendData.Add(0x00); }

            // 3.7 Tính lại Request Data Length
            int reDataL = lstSendData.Count - 2 - 5 - 2;
            byte[] arrReqData = BitConverter.GetBytes(reDataL);
            lstSendData[7] = arrReqData[0];
            lstSendData[8] = arrReqData[1];
            // B4: Send Data
            sock.Send(lstSendData.ToArray());

            // B5: Recieve Data
            byte[] arrRcv = new byte[512];
            List<byte> lstRcv = new List<byte>();
            sock.Receive(arrRcv);
            lstRcv.AddRange(arrRcv);

            // B6: Phân tích data nhận về
            // 6.1 Sub Header
            if (lstRcv[0] != 0xD0 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.2 Access Route
            // 6.2.1 Network No
            if (lstRcv[0] != (byte)this.NetWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.2 PC No
            if (lstRcv[0] != (byte)this.PcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.3 Request Destination Module IO
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            // 6.2.4 Request Station No
            if (lstRcv[0] != (byte)this.StationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            // 6.3 Request Data Length
            short reqDataLength = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (reqDataLength < 2)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.4 End Code
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.5 Data
            kq = 0;
            return kq;
        }
        public int WriteMultiBit(Device _devCode, int _devNumber, List<bool> _value)
        {
            int kq = -1;
            // B1: Kiểm tra đã khởi tạo
            if (sock == null)
            {
                return kq;
            }
            // B2: Kiểm tra đã kết nối
            if (sock.Connected == false)
            {
                return kq;
            }

            // B3: Chuẩn bị data
            List<byte> lstSendData = new List<byte>();
            // 3.1 Header: Đã tự động thêm vào <Skip>
            // 3.2 Subheader [2byte]
            lstSendData.Add(0x50);
            lstSendData.Add(0x00);

            // 3.3 Access Route
            // 3.3.1 Network No
            lstSendData.Add((byte)this.NetWorkNo);
            // 3.3.2 PC No
            lstSendData.Add((byte)this.PcNo);
            // 3.3.3 Request Destination Module IO
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            // 3.3.4 Request Destination Module Station
            lstSendData.Add((byte)this.StationNo);

            // 3.4 Request Data Length [byte7 - byte8]
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);

            // 3.5 Monitoring Time
            lstSendData.Add(0x10);
            lstSendData.Add(0x00);

            // 3.6 Request Data
            // 3.6.1 Command [1401]
            lstSendData.Add(0x01);
            lstSendData.Add(0x14);
            // 3.6.2 Sub command [0001]
            lstSendData.Add(0x01);
            lstSendData.Add(0x00);
            // 3.6.3 Head Device Number
            byte[] headDevice = BitConverter.GetBytes(_devNumber);
            lstSendData.Add(headDevice[0]);
            lstSendData.Add(headDevice[1]);
            lstSendData.Add(headDevice[2]);
            // 3.6.4 Device Code
            lstSendData.Add((byte)_devCode);
            // 3.6.5 Number Of Device Point
            if (_value == null) { return kq; }
            int devPoint = _value.Count;

            byte[] arrDevPoint = BitConverter.GetBytes(devPoint);
            lstSendData.Add(arrDevPoint[0]);
            lstSendData.Add(arrDevPoint[1]);
            // 3.6.6 Data
            // Exp: [M10, M11, M12, M13, M14, M15] <> 3byte
            int byteCount = (_value.Count / 2) + (_value.Count % 2);
            for (int i = 0; i < byteCount; i++)
            {
                int bitTruoc = 0;
                int bitSau = 0;

                if (_value[i * 2] == false)
                {
                    bitTruoc = 0x00;
                }
                else
                    bitTruoc |= 0x01;
                if (_value.Count > (i * 2 + 1))
                {
                    if (_value[i * 2 + 1] == false)
                    {
                        bitSau = 0x00;
                    }
                    else
                        bitSau = 0x01;
                }
                byte meger = (byte)(bitTruoc | bitSau);
                lstSendData.Add(meger);
            }

            // 3.7 Tính lại Request Data Length
            int reDataL = lstSendData.Count - 2 - 5 - 2;
            byte[] arrReqData = BitConverter.GetBytes(reDataL);
            lstSendData[7] = arrReqData[0];
            lstSendData[8] = arrReqData[1];
            // B4: Send Data
            sock.Send(lstSendData.ToArray());

            // B5: Recieve Data
            byte[] arrRcv = new byte[512];
            List<byte> lstRcv = new List<byte>();
            sock.Receive(arrRcv);
            lstRcv.AddRange(arrRcv);

            // B6: Phân tích data nhận về
            // 6.1 Sub Header
            if (lstRcv[0] != 0xD0 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.2 Access Route
            // 6.2.1 Network No
            if (lstRcv[0] != (byte)this.NetWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.2 PC No
            if (lstRcv[0] != (byte)this.PcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.3 Request Destination Module IO
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            // 6.2.4 Request Station No
            if (lstRcv[0] != (byte)this.StationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            // 6.3 Request Data Length
            short reqDataLength = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (reqDataLength < 2)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.4 End Code
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.5 Data
            kq = 0;
            return kq;
        }





    }
}
