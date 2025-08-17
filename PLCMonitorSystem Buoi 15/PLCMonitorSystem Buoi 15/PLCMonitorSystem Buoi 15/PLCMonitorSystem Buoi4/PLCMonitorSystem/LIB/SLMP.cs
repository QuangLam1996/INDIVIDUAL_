using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;


namespace MTECH
{
    public enum DevCode
    {
        SM = 0x91,
        SD = 0xA9,
        X = 0x9C,
        Y = 0x9D,
        M = 0x90,
        L = 0x92,
        F = 0x93,
        V = 0x94,
        B = 0xA0,
        D = 0xA8,
        W = 0xB4,
        Z = 0xCC,
        R = 0xAF,
        ZR = 0xB0,

    }
    public class SLMP
    {
        #region Properties và Field:
        Socket sock;
        String ipAdress;//IP Address của PLC
        int portNo;// Của PLC
        int stationNo;
        int netWorkNo;
        int pcNo;
       

        public string IpAdress { get => ipAdress; set => ipAdress = value; }
        public int PortNo { get => portNo; set => portNo = value; }
        public int StationNo { get => stationNo; set => stationNo = value; }
        public int NetWorkNo { get => netWorkNo; set => netWorkNo = value; }
        public int PcNo { get => pcNo; set => pcNo = value; }
       
        #endregion


        #region Method
        public SLMP()
        {
            this.ipAdress = "192.168.3.39";
            this.portNo = 1025;
            this.netWorkNo = 0;
            this.pcNo = 255;
            this.stationNo = 0;
        }

        public SLMP(string _ipAdress, int _portNo)
        {
            this.ipAdress = _ipAdress;
            this.portNo = _portNo;
            this.netWorkNo = 0;
            this.pcNo = 255;
            this.stationNo = 0;
        }

        public SLMP(int _netWorkNo, int _pcNo, int _stationNo)
        {
            this.ipAdress = "192.168.3.39";
            this.portNo = 1025;
            this.netWorkNo = _netWorkNo;
            this.pcNo = _pcNo;
            this.stationNo = _stationNo;
        }
        public int Open()
        {
            int kq = -1;
            // B1: Kiểm tra Sock đã được khởi tạo:
            if (sock == null)
            {
                sock = new Socket(SocketType.Stream, ProtocolType.Tcp);
            }
            //B2: Kiểm tra kết nối:
            if (sock.Connected == true)
            {
                kq = 0;
                return kq;
            }
            //B3: Kết nối:
            try
            {
                sock.Connect(this.ipAdress, this.portNo);
                if (sock.Connected == true)
                {
                    kq = 0;
                }
            }
            catch (Exception err)
            {
            }
            return kq;
        }
        public int Close()
        {
            int kq = -1;
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
            catch (Exception err)
            {
            }
            return kq;
        }
        public short ReadWord(DevCode _devCode, int _devNumber)
        {
            short kq = 0;
            // B1: Kiểm tra đã khởi tạo sock chưa:
            if (sock==null)
            {
                return kq;
            }
            //B2: Kiểm tra đã kết nối chưa:
            if (sock.Connected==false)
            {
                return kq;
            }
            // B3: Chuẩn bị dữ liệu gửi đi:
            List<byte> lstSendData = new List<byte>();
            //3.1. Header: Thông thường được tự động thêm vào.
            //3.2. Subheader: 2 byte
            lstSendData.Add(0x50);
            lstSendData.Add(0x00);
            //3.3.Access route:
            //3.3.1: NetWork No:
            lstSendData.Add((byte)this.netWorkNo);
            //3.3.2. PC No:
            lstSendData.Add((byte)this.pcNo);
            //3.3.3. Request Destination Module IO No:
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            //3.3.4. Request Destination Module Station No:
            lstSendData.Add((byte)this.stationNo);

            //3.4. Request Data Length: byte [7][8]
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);

            //3.5. Monitoring Timer:
            lstSendData.Add(0x10);
            lstSendData.Add(0x00);

            //3.6. Request Data:
            //3.6.1. Command: 0401
            lstSendData.Add(0x01);
            lstSendData.Add(0x04);
            //3.6.2 SubCommand: 0000
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);

            //3.6.3. Head Device number: Đọc thanh ghi nào:
            byte[] headDevice = BitConverter.GetBytes(_devNumber);
            lstSendData.Add(headDevice[0]);
            lstSendData.Add(headDevice[1]);
            lstSendData.Add(headDevice[2]);

            //3.6.4. Device Code: Đọc thanh loại gì: D,W,ZR...
            lstSendData.Add((byte)_devCode);

            //3.6.5. Number of device point: Số lượng liên tiếp muốn đọc
            int devPoint = 1;
            byte[] arrDevPoint = BitConverter.GetBytes(devPoint);
            lstSendData.Add(arrDevPoint[0]);
            lstSendData.Add(arrDevPoint[1]);

            // 3.7. Tính request Data length:
            int reqDataL = lstSendData.Count - 2 - 5 - 2;
            byte[] arrReqDataLength = BitConverter.GetBytes(reqDataL);
            lstSendData[7] = arrReqDataLength[0];
            lstSendData[8] = arrReqDataLength[1];

            //Bước 4: Gửi dữ liệu đi:
            sock.Send(lstSendData.ToArray());

            //Bước 5: Nhận dữ liệu về:
            byte[] arrRcv = new byte[512];
            List<byte> lstRcv = new List<byte>();
            sock.Receive(arrRcv);
            lstRcv.AddRange(arrRcv);

            //Bước 6: Phân tích dữ liệu nhận về:
            //6.1. Sub Header:
            if (lstRcv[0]!=0xD0 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            //6.2. Access Route:
            //6.2.1. NetWork No:
            if (lstRcv[0]!= (byte)this.netWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.2.2. PC No
            if (lstRcv[0] != (byte)this.pcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.3. Request Destination Module IO NO:
            if (lstRcv[0] !=0xFF || lstRcv[1]!=0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.2.4. Request station No:
            if (lstRcv[0]!=(byte)this.stationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            //6.3. Request Data Length:
            short reqDataLength = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (reqDataLength<2)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.4. End Code:
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.5. Data:
            kq = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);

            return kq;
        }
        public int ReadDoubleWord(DevCode _devCode, int _devNumber)
        {
            int kq = 0;
            // B1: Kiểm tra đã khởi tạo sock chưa:
            if (sock == null)
            {
                return kq;
            }
            //B2: Kiểm tra đã kết nối chưa:
            if (sock.Connected == false)
            {
                return kq;
            }
            // B3: Chuẩn bị dữ liệu gửi đi:
            List<byte> lstSendData = new List<byte>();
            //3.1. Header: Thông thường được tự động thêm vào.
            //3.2. Subheader: 2 byte
            lstSendData.Add(0x50);
            lstSendData.Add(0x00);
            //3.3.Access route:
            //3.3.1: NetWork No:
            lstSendData.Add((byte)this.netWorkNo);
            //3.3.2. PC No:
            lstSendData.Add((byte)this.pcNo);
            //3.3.3. Request Destination Module IO No:
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            //3.3.4. Request Destination Module Station No:
            lstSendData.Add((byte)this.stationNo);

            //3.4. Request Data Length: byte [7][8]
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);

            //3.5. Monitoring Timer:
            lstSendData.Add(0x10);
            lstSendData.Add(0x00);

            //3.6. Request Data:
            //3.6.1. Command: 0401
            lstSendData.Add(0x01);
            lstSendData.Add(0x04);
            //3.6.2 SubCommand: 0000
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);

            //3.6.3. Head Device number: Đọc thanh ghi nào:
            byte[] headDevice = BitConverter.GetBytes(_devNumber);
            lstSendData.Add(headDevice[0]);
            lstSendData.Add(headDevice[1]);
            lstSendData.Add(headDevice[2]);

            //3.6.4. Device Code: Đọc thanh loại gì: D,W,ZR...
            lstSendData.Add((byte)_devCode);

            //3.6.5. Number of device point: Số lượng liên tiếp muốn đọc
            int devPoint = 2;
            byte[] arrDevPoint = BitConverter.GetBytes(devPoint);
            lstSendData.Add(arrDevPoint[0]);
            lstSendData.Add(arrDevPoint[1]);
            // 3.7. Tính request Data length:
            int reqDataL = lstSendData.Count - 2 - 5 - 2;
            byte[] arrReqDataLength = BitConverter.GetBytes(reqDataL);
            lstSendData[7] = arrReqDataLength[0];
            lstSendData[8] = arrReqDataLength[1];

            //Bước 4: Gửi dữ liệu đi:
            sock.Send(lstSendData.ToArray());

            //Bước 5: Nhận dữ liệu về:
            byte[] arrRcv = new byte[512];
            List<byte> lstRcv = new List<byte>();
            sock.Receive(arrRcv);
            lstRcv.AddRange(arrRcv);

            //Bước 6: Phân tích dữ liệu nhận về:
            //6.1. Sub Header:
            if (lstRcv[0] != 0xD0 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            //6.2. Access Route:
            //6.2.1. NetWork No:
            if (lstRcv[0] != (byte)this.netWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.2.2. PC No
            if (lstRcv[0] != (byte)this.pcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.3. Request Destination Module IO NO:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.2.4. Request station No:
            if (lstRcv[0] != (byte)this.stationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            //6.3. Request Data Length:
            short reqDataLength = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (reqDataLength < 2)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.4. End Code:
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.5. Data:
            kq = BitConverter.ToInt32(new byte[] { lstRcv[0], lstRcv[1], lstRcv[2], lstRcv[3] }, 0);

            return kq;
        }
        public float ReadFloat(DevCode _devCode, int _devNumber)
        {
            float kq = 0f;
            // B1: Kiểm tra đã khởi tạo sock chưa:
            if (sock == null)
            {
                return kq;
            }
            //B2: Kiểm tra đã kết nối chưa:
            if (sock.Connected == false)
            {
                return kq;
            }
            // B3: Chuẩn bị dữ liệu gửi đi:
            List<byte> lstSendData = new List<byte>();
            //3.1. Header: Thông thường được tự động thêm vào.
            //3.2. Subheader: 2 byte
            lstSendData.Add(0x50);
            lstSendData.Add(0x00);
            //3.3.Access route:
            //3.3.1: NetWork No:
            lstSendData.Add((byte)this.netWorkNo);
            //3.3.2. PC No:
            lstSendData.Add((byte)this.pcNo);
            //3.3.3. Request Destination Module IO No:
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            //3.3.4. Request Destination Module Station No:
            lstSendData.Add((byte)this.stationNo);

            //3.4. Request Data Length: byte [7][8]
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);

            //3.5. Monitoring Timer:
            lstSendData.Add(0x10);
            lstSendData.Add(0x00);

            //3.6. Request Data:
            //3.6.1. Command: 0401
            lstSendData.Add(0x01);
            lstSendData.Add(0x04);
            //3.6.2 SubCommand: 0000
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);

            //3.6.3. Head Device number: Đọc thanh ghi nào:
            byte[] headDevice = BitConverter.GetBytes(_devNumber);
            lstSendData.Add(headDevice[0]);
            lstSendData.Add(headDevice[1]);
            lstSendData.Add(headDevice[2]);

            //3.6.4. Device Code: Đọc thanh loại gì: D,W,ZR...
            lstSendData.Add((byte)_devCode);

            //3.6.5. Number of device point: Số lượng liên tiếp muốn đọc
            int devPoint = 2;
            byte[] arrDevPoint = BitConverter.GetBytes(devPoint);
            lstSendData.Add(arrDevPoint[0]);
            lstSendData.Add(arrDevPoint[1]);
            
            // 3.7. Tính request Data length:
            int reqDataL = lstSendData.Count - 2 - 5 - 2;
            byte[] arrReqDataLength = BitConverter.GetBytes(reqDataL);
            lstSendData[7] = arrReqDataLength[0];
            lstSendData[8] = arrReqDataLength[1];

            //Bước 4: Gửi dữ liệu đi:
            sock.Send(lstSendData.ToArray());

            //Bước 5: Nhận dữ liệu về:
            byte[] arrRcv = new byte[512];
            List<byte> lstRcv = new List<byte>();
            sock.Receive(arrRcv);
            lstRcv.AddRange(arrRcv);

            //Bước 6: Phân tích dữ liệu nhận về:
            //6.1. Sub Header:
            if (lstRcv[0] != 0xD0 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            //6.2. Access Route:
            //6.2.1. NetWork No:
            if (lstRcv[0] != (byte)this.netWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.2.2. PC No
            if (lstRcv[0] != (byte)this.pcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.3. Request Destination Module IO NO:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.2.4. Request station No:
            if (lstRcv[0] != (byte)this.stationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            //6.3. Request Data Length:
            short reqDataLength = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (reqDataLength < 2)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.4. End Code:
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.5. Data:
            kq = BitConverter.ToSingle(new byte[] { lstRcv[0], lstRcv[1], lstRcv[2], lstRcv[3] }, 0);

            return kq;
        }
        public bool ReadBit(DevCode _devCode, int _devNumber)
        {
            bool kq = false;
            // B1: Kiểm tra đã khởi tạo sock chưa:
            if (sock == null)
            {
                return kq;
            }
            //B2: Kiểm tra đã kết nối chưa:
            if (sock.Connected == false)
            {
                return kq;
            }
            // B3: Chuẩn bị dữ liệu gửi đi:
            List<byte> lstSendData = new List<byte>();
            //3.1. Header: Thông thường được tự động thêm vào.
            //3.2. Subheader: 2 byte
            lstSendData.Add(0x50);
            lstSendData.Add(0x00);
            //3.3.Access route:
            //3.3.1: NetWork No:
            lstSendData.Add((byte)this.netWorkNo);
            //3.3.2. PC No:
            lstSendData.Add((byte)this.pcNo);
            //3.3.3. Request Destination Module IO No:
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            //3.3.4. Request Destination Module Station No:
            lstSendData.Add((byte)this.stationNo);

            //3.4. Request Data Length: byte [7][8]
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);

            //3.5. Monitoring Timer:
            lstSendData.Add(0x10);
            lstSendData.Add(0x00);

            //3.6. Request Data:
            //3.6.1. Command: 0401
            lstSendData.Add(0x01);
            lstSendData.Add(0x04);
            //3.6.2 SubCommand: 0001
            lstSendData.Add(0x01);
            lstSendData.Add(0x00);

            //3.6.3. Head Device number: Đọc thanh ghi nào:
            byte[] headDevice = BitConverter.GetBytes(_devNumber);
            lstSendData.Add(headDevice[0]);
            lstSendData.Add(headDevice[1]);
            lstSendData.Add(headDevice[2]);

            //3.6.4. Device Code: Đọc thanh loại gì: D,W,ZR...
            lstSendData.Add((byte)_devCode);

            //3.6.5. Number of device point: Số lượng liên tiếp muốn đọc
            int devPoint = 1;
            byte[] arrDevPoint = BitConverter.GetBytes(devPoint);
            lstSendData.Add(arrDevPoint[0]);
            lstSendData.Add(arrDevPoint[1]);

            // 3.7. Tính request Data length:
            int reqDataL = lstSendData.Count - 2 - 5 - 2;
            byte[] arrReqDataLength = BitConverter.GetBytes(reqDataL);
            lstSendData[7] = arrReqDataLength[0];
            lstSendData[8] = arrReqDataLength[1];

            //Bước 4: Gửi dữ liệu đi:
            sock.Send(lstSendData.ToArray());

            //Bước 5: Nhận dữ liệu về:
            byte[] arrRcv = new byte[512];
            List<byte> lstRcv = new List<byte>();
            sock.Receive(arrRcv);
            lstRcv.AddRange(arrRcv);

            //Bước 6: Phân tích dữ liệu nhận về:
            //6.1. Sub Header:
            if (lstRcv[0] != 0xD0 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            //6.2. Access Route:
            //6.2.1. NetWork No:
            if (lstRcv[0] != (byte)this.netWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.2.2. PC No
            if (lstRcv[0] != (byte)this.pcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.3. Request Destination Module IO NO:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.2.4. Request station No:
            if (lstRcv[0] != (byte)this.stationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            //6.3. Request Data Length:
            short reqDataLength = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (reqDataLength < 2)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.4. End Code:
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.5. Data:
            if (lstRcv[0]!=0)
            {
                kq = true;
            }
            return kq;
        }
        public List<bool> ReadMultiBit(DevCode _devCode, int _devNumber, int _count)
        {
            List<bool> kq = new List<bool>();
            // B1: Kiểm tra đã khởi tạo sock chưa:
            if (sock == null)
            {
                return kq;
            }
            //B2: Kiểm tra đã kết nối chưa:
            if (sock.Connected == false)
            {
                return kq;
            }
            // B3: Chuẩn bị dữ liệu gửi đi:
            List<byte> lstSendData = new List<byte>();
            //3.1. Header: Thông thường được tự động thêm vào.
            //3.2. Subheader: 2 byte
            lstSendData.Add(0x50);
            lstSendData.Add(0x00);
            //3.3.Access route:
            //3.3.1: NetWork No:
            lstSendData.Add((byte)this.netWorkNo);
            //3.3.2. PC No:
            lstSendData.Add((byte)this.pcNo);
            //3.3.3. Request Destination Module IO No:
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            //3.3.4. Request Destination Module Station No:
            lstSendData.Add((byte)this.stationNo);

            //3.4. Request Data Length: byte [7][8]
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);

            //3.5. Monitoring Timer:
            lstSendData.Add(0x10);
            lstSendData.Add(0x00);

            //3.6. Request Data:
            //3.6.1. Command: 0401
            lstSendData.Add(0x01);
            lstSendData.Add(0x04);
            //3.6.2 SubCommand: 0001
            lstSendData.Add(0x01);
            lstSendData.Add(0x00);

            //3.6.3. Head Device number: Đọc thanh ghi nào:
            byte[] headDevice = BitConverter.GetBytes(_devNumber);
            lstSendData.Add(headDevice[0]);
            lstSendData.Add(headDevice[1]);
            lstSendData.Add(headDevice[2]);

            //3.6.4. Device Code: Đọc thanh loại gì: D,W,ZR...
            lstSendData.Add((byte)_devCode);

            //3.6.5. Number of device point: Số lượng liên tiếp muốn đọc
            if (_count<=0)
            {
                return kq;
            }
            int devPoint = _count;
            byte[] arrDevPoint = BitConverter.GetBytes(devPoint);
            lstSendData.Add(arrDevPoint[0]);
            lstSendData.Add(arrDevPoint[1]);

            // 3.7. Tính request Data length:
            int reqDataL = lstSendData.Count - 2 - 5 - 2;
            byte[] arrReqDataLength = BitConverter.GetBytes(reqDataL);
            lstSendData[7] = arrReqDataLength[0];
            lstSendData[8] = arrReqDataLength[1];

            //Bước 4: Gửi dữ liệu đi:
            sock.Send(lstSendData.ToArray());

            //Bước 5: Nhận dữ liệu về:
            byte[] arrRcv = new byte[512];
            List<byte> lstRcv = new List<byte>();
            sock.Receive(arrRcv);
            lstRcv.AddRange(arrRcv);

            //Bước 6: Phân tích dữ liệu nhận về:
            //6.1. Sub Header:
            if (lstRcv[0] != 0xD0 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            //6.2. Access Route:
            //6.2.1. NetWork No:
            if (lstRcv[0] != (byte)this.netWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.2.2. PC No
            if (lstRcv[0] != (byte)this.pcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.3. Request Destination Module IO NO:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.2.4. Request station No:
            if (lstRcv[0] != (byte)this.stationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            //6.3. Request Data Length:
            short reqDataLength = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (reqDataLength < 2)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.4. End Code:
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.5. Data:
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
                else if (lstRcv[0] == 0x10)
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
            if (kq.Count > _count)
            {
                kq.RemoveAt(_count);
            }

            return kq;
        }
        public int WriteWord(DevCode _devCode, int _devNumber, short _value)
        {
            int kq = -1;
            // B1: Kiểm tra đã khởi tạo sock chưa:
            if (sock == null)
            {
                return kq;
            }
            //B2: Kiểm tra đã kết nối chưa:
            if (sock.Connected == false)
            {
                return kq;
            }
            // B3: Chuẩn bị dữ liệu gửi đi:
            List<byte> lstSendData = new List<byte>();
            //3.1. Header: Thông thường được tự động thêm vào.
            //3.2. Subheader: 2 byte
            lstSendData.Add(0x50);
            lstSendData.Add(0x00);
            //3.3.Access route:
            //3.3.1: NetWork No:
            lstSendData.Add((byte)this.netWorkNo);
            //3.3.2. PC No:
            lstSendData.Add((byte)this.pcNo);
            //3.3.3. Request Destination Module IO No:
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            //3.3.4. Request Destination Module Station No:
            lstSendData.Add((byte)this.stationNo);

            //3.4. Request Data Length: byte [7][8]
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);

            //3.5. Monitoring Timer:
            lstSendData.Add(0x10);
            lstSendData.Add(0x00);

            //3.6. Request Data:
            //3.6.1. Command: 1401
            lstSendData.Add(0x01);
            lstSendData.Add(0x14);
            //3.6.2 SubCommand: 0000
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);

            //3.6.3. Head Device number: Đọc thanh ghi nào:
            byte[] headDevice = BitConverter.GetBytes(_devNumber);
            lstSendData.Add(headDevice[0]);
            lstSendData.Add(headDevice[1]);
            lstSendData.Add(headDevice[2]);

            //3.6.4. Device Code: Đọc thanh loại gì: D,W,ZR...
            lstSendData.Add((byte)_devCode);

            //3.6.5. Number of device point: Số lượng liên tiếp muốn đọc
            int devPoint = 1;
            byte[] arrDevPoint = BitConverter.GetBytes(devPoint);
            lstSendData.Add(arrDevPoint[0]);
            lstSendData.Add(arrDevPoint[1]);

            // 3.6.6. Data:
            byte[] arrData = BitConverter.GetBytes(_value);
            lstSendData.Add(arrData[0]);
            lstSendData.Add(arrData[1]);

            // 3.7. Tính request Data length:
            int reqDataL = lstSendData.Count - 2 - 5 - 2;
            byte[] arrReqDataLength = BitConverter.GetBytes(reqDataL);
            lstSendData[7] = arrReqDataLength[0];
            lstSendData[8] = arrReqDataLength[1];

            //Bước 4: Gửi dữ liệu đi:
            sock.Send(lstSendData.ToArray());

            //Bước 5: Nhận dữ liệu về:
            byte[] arrRcv = new byte[512];
            List<byte> lstRcv = new List<byte>();
            sock.Receive(arrRcv);
            lstRcv.AddRange(arrRcv);

            //Bước 6: Phân tích dữ liệu nhận về:
            //6.1. Sub Header:
            if (lstRcv[0] != 0xD0 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            //6.2. Access Route:
            //6.2.1. NetWork No:
            if (lstRcv[0] != (byte)this.netWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.2.2. PC No
            if (lstRcv[0] != (byte)this.pcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.3. Request Destination Module IO NO:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.2.4. Request station No:
            if (lstRcv[0] != (byte)this.stationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            //6.3. Request Data Length:
            short reqDataLength = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (reqDataLength < 2)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.4. End Code:
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            kq = 0;
            return kq;
        }
        public int WriteBit(DevCode _devCode, int _devNumber, bool _value)
        {
            int kq = -1;
            // B1: Kiểm tra đã khởi tạo sock chưa:
            if (sock == null)
            {
                return kq;
            }
            //B2: Kiểm tra đã kết nối chưa:
            if (sock.Connected == false)
            {
                return kq;
            }
            // B3: Chuẩn bị dữ liệu gửi đi:
            List<byte> lstSendData = new List<byte>();
            //3.1. Header: Thông thường được tự động thêm vào.
            //3.2. Subheader: 2 byte
            lstSendData.Add(0x50);
            lstSendData.Add(0x00);
            //3.3.Access route:
            //3.3.1: NetWork No:
            lstSendData.Add((byte)this.netWorkNo);
            //3.3.2. PC No:
            lstSendData.Add((byte)this.pcNo);
            //3.3.3. Request Destination Module IO No:
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            //3.3.4. Request Destination Module Station No:
            lstSendData.Add((byte)this.stationNo);

            //3.4. Request Data Length: byte [7][8]
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);

            //3.5. Monitoring Timer:
            lstSendData.Add(0x10);
            lstSendData.Add(0x00);

            //3.6. Request Data:
            //3.6.1. Command: 1401
            lstSendData.Add(0x01);
            lstSendData.Add(0x14);
            //3.6.2 SubCommand: 0001
            lstSendData.Add(0x01);
            lstSendData.Add(0x00);

            //3.6.3. Head Device number: Đọc thanh ghi nào:
            byte[] headDevice = BitConverter.GetBytes(_devNumber);
            lstSendData.Add(headDevice[0]);
            lstSendData.Add(headDevice[1]);
            lstSendData.Add(headDevice[2]);

            //3.6.4. Device Code: Đọc thanh loại gì: D,W,ZR...
            lstSendData.Add((byte)_devCode);

            //3.6.5. Number of device point: Số lượng liên tiếp muốn đọc
            int devPoint = 1;
            byte[] arrDevPoint = BitConverter.GetBytes(devPoint);
            lstSendData.Add(arrDevPoint[0]);
            lstSendData.Add(arrDevPoint[1]);

            // 3.6.6. Data:
            if (_value)
            {
                lstSendData.Add(0x10);
            }
            else
                lstSendData.Add(0x00);

            // 3.7. Tính request Data length:
            int reqDataL = lstSendData.Count - 2 - 5 - 2;
            byte[] arrReqDataLength = BitConverter.GetBytes(reqDataL);
            lstSendData[7] = arrReqDataLength[0];
            lstSendData[8] = arrReqDataLength[1];

            //Bước 4: Gửi dữ liệu đi:
            sock.Send(lstSendData.ToArray());

            //Bước 5: Nhận dữ liệu về:
            byte[] arrRcv = new byte[512];
            List<byte> lstRcv = new List<byte>();
            sock.Receive(arrRcv);
            lstRcv.AddRange(arrRcv);

            //Bước 6: Phân tích dữ liệu nhận về:
            //6.1. Sub Header:
            if (lstRcv[0] != 0xD0 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            //6.2. Access Route:
            //6.2.1. NetWork No:
            if (lstRcv[0] != (byte)this.netWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.2.2. PC No
            if (lstRcv[0] != (byte)this.pcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.3. Request Destination Module IO NO:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.2.4. Request station No:
            if (lstRcv[0] != (byte)this.stationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            //6.3. Request Data Length:
            short reqDataLength = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (reqDataLength < 2)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.4. End Code:
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            kq = 0;
            return kq;
        }
        public int WriteMultiBit(DevCode _devCode, int _devNumber, List<bool> _lstValue)
        {
            int kq = -1;
            // B1: Kiểm tra đã khởi tạo sock chưa:
            if (sock == null)
            {
                return kq;
            }
            //B2: Kiểm tra đã kết nối chưa:
            if (sock.Connected == false)
            {
                return kq;
            }
            // B3: Chuẩn bị dữ liệu gửi đi:
            List<byte> lstSendData = new List<byte>();
            //3.1. Header: Thông thường được tự động thêm vào.
            //3.2. Subheader: 2 byte
            lstSendData.Add(0x50);
            lstSendData.Add(0x00);
            //3.3.Access route:
            //3.3.1: NetWork No:
            lstSendData.Add((byte)this.netWorkNo);
            //3.3.2. PC No:
            lstSendData.Add((byte)this.pcNo);
            //3.3.3. Request Destination Module IO No:
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            //3.3.4. Request Destination Module Station No:
            lstSendData.Add((byte)this.stationNo);

            //3.4. Request Data Length: byte [7][8]
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);

            //3.5. Monitoring Timer:
            lstSendData.Add(0x10);
            lstSendData.Add(0x00);

            //3.6. Request Data:
            //3.6.1. Command: 1401
            lstSendData.Add(0x01);
            lstSendData.Add(0x14);
            //3.6.2 SubCommand: 0001
            lstSendData.Add(0x01);
            lstSendData.Add(0x00);

            //3.6.3. Head Device number: Đọc thanh ghi nào:
            byte[] headDevice = BitConverter.GetBytes(_devNumber);
            lstSendData.Add(headDevice[0]);
            lstSendData.Add(headDevice[1]);
            lstSendData.Add(headDevice[2]);

            //3.6.4. Device Code: Đọc thanh loại gì: D,W,ZR...
            lstSendData.Add((byte)_devCode);

            //3.6.5. Number of device point: Số lượng liên tiếp muốn đọc
            if (_lstValue==null)
            {
                return kq;
            }
            if (_lstValue.Count<=0)
            {
                return kq;
            }
            int devPoint = _lstValue.Count;
            byte[] arrDevPoint = BitConverter.GetBytes(devPoint);
            lstSendData.Add(arrDevPoint[0]);
            lstSendData.Add(arrDevPoint[1]);

            // 3.6.6. Data:

            int byteCount = _lstValue.Count / 2 + _lstValue.Count % 2;
            for (int i = 0; i < byteCount; i++)
            {
                int bitTruoc = 0;//M10
                int bitSau = 0;//M11
                // Bit truoc
                if (_lstValue[i * 2] == false)
                {
                    bitTruoc = 0x00;
                }
                else
                    bitTruoc = 0x10;
                // Bit sau:
                if (_lstValue.Count > (i * 2 + 1))
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
                byte GiaTri = (byte)(bitTruoc | bitSau);
                lstSendData.Add(GiaTri);

            }

            // 3.7. Tính request Data length:
            int reqDataL = lstSendData.Count - 2 - 5 - 2;
            byte[] arrReqDataLength = BitConverter.GetBytes(reqDataL);
            lstSendData[7] = arrReqDataLength[0];
            lstSendData[8] = arrReqDataLength[1];

            //Bước 4: Gửi dữ liệu đi:
            sock.Send(lstSendData.ToArray());

            //Bước 5: Nhận dữ liệu về:
            byte[] arrRcv = new byte[512];
            List<byte> lstRcv = new List<byte>();
            sock.Receive(arrRcv);
            lstRcv.AddRange(arrRcv);

            //Bước 6: Phân tích dữ liệu nhận về:
            //6.1. Sub Header:
            if (lstRcv[0] != 0xD0 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            //6.2. Access Route:
            //6.2.1. NetWork No:
            if (lstRcv[0] != (byte)this.netWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.2.2. PC No
            if (lstRcv[0] != (byte)this.pcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.3. Request Destination Module IO NO:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.2.4. Request station No:
            if (lstRcv[0] != (byte)this.stationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            //6.3. Request Data Length:
            short reqDataLength = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (reqDataLength < 2)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.4. End Code:
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            kq = 0;
            return kq;
        }
        public int WriteDoubleWord(DevCode _devCode, int _devNumber, int _value)
        {
            int kq = -1;
            // B1: Kiểm tra đã khởi tạo sock chưa:
            if (sock == null)
            {
                return kq;
            }
            //B2: Kiểm tra đã kết nối chưa:
            if (sock.Connected == false)
            {
                return kq;
            }
            // B3: Chuẩn bị dữ liệu gửi đi:
            List<byte> lstSendData = new List<byte>();
            //3.1. Header: Thông thường được tự động thêm vào.
            //3.2. Subheader: 2 byte
            lstSendData.Add(0x50);
            lstSendData.Add(0x00);
            //3.3.Access route:
            //3.3.1: NetWork No:
            lstSendData.Add((byte)this.netWorkNo);
            //3.3.2. PC No:
            lstSendData.Add((byte)this.pcNo);
            //3.3.3. Request Destination Module IO No:
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            //3.3.4. Request Destination Module Station No:
            lstSendData.Add((byte)this.stationNo);

            //3.4. Request Data Length: byte [7][8]
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);

            //3.5. Monitoring Timer:
            lstSendData.Add(0x10);
            lstSendData.Add(0x00);

            //3.6. Request Data:
            //3.6.1. Command: 1401
            lstSendData.Add(0x01);
            lstSendData.Add(0x14);
            //3.6.2 SubCommand: 0000
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);

            //3.6.3. Head Device number: Đọc thanh ghi nào:
            byte[] headDevice = BitConverter.GetBytes(_devNumber);
            lstSendData.Add(headDevice[0]);
            lstSendData.Add(headDevice[1]);
            lstSendData.Add(headDevice[2]);

            //3.6.4. Device Code: Đọc thanh loại gì: D,W,ZR...
            lstSendData.Add((byte)_devCode);

            //3.6.5. Number of device point: Số lượng liên tiếp muốn đọc
            int devPoint = 2;
            byte[] arrDevPoint = BitConverter.GetBytes(devPoint);
            lstSendData.Add(arrDevPoint[0]);
            lstSendData.Add(arrDevPoint[1]);

            // 3.6.6. Data:
            byte[] arrData = BitConverter.GetBytes(_value);
            lstSendData.Add(arrData[0]);
            lstSendData.Add(arrData[1]);
            lstSendData.Add(arrData[2]);
            lstSendData.Add(arrData[3]);

            // 3.7. Tính request Data length:
            int reqDataL = lstSendData.Count - 2 - 5 - 2;
            byte[] arrReqDataLength = BitConverter.GetBytes(reqDataL);
            lstSendData[7] = arrReqDataLength[0];
            lstSendData[8] = arrReqDataLength[1];

            //Bước 4: Gửi dữ liệu đi:
            sock.Send(lstSendData.ToArray());

            //Bước 5: Nhận dữ liệu về:
            byte[] arrRcv = new byte[512];
            List<byte> lstRcv = new List<byte>();
            sock.Receive(arrRcv);
            lstRcv.AddRange(arrRcv);

            //Bước 6: Phân tích dữ liệu nhận về:
            //6.1. Sub Header:
            if (lstRcv[0] != 0xD0 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            //6.2. Access Route:
            //6.2.1. NetWork No:
            if (lstRcv[0] != (byte)this.netWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.2.2. PC No
            if (lstRcv[0] != (byte)this.pcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.3. Request Destination Module IO NO:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.2.4. Request station No:
            if (lstRcv[0] != (byte)this.stationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            //6.3. Request Data Length:
            short reqDataLength = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (reqDataLength < 2)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.4. End Code:
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            kq = 0;
            return kq;
        }
        public int WriteFloat(DevCode _devCode, int _devNumber, float _value)
        {
            int kq = -1;
            // B1: Kiểm tra đã khởi tạo sock chưa:
            if (sock == null)
            {
                return kq;
            }
            //B2: Kiểm tra đã kết nối chưa:
            if (sock.Connected == false)
            {
                return kq;
            }
            // B3: Chuẩn bị dữ liệu gửi đi:
            List<byte> lstSendData = new List<byte>();
            //3.1. Header: Thông thường được tự động thêm vào.
            //3.2. Subheader: 2 byte
            lstSendData.Add(0x50);
            lstSendData.Add(0x00);
            //3.3.Access route:
            //3.3.1: NetWork No:
            lstSendData.Add((byte)this.netWorkNo);
            //3.3.2. PC No:
            lstSendData.Add((byte)this.pcNo);
            //3.3.3. Request Destination Module IO No:
            lstSendData.Add(0xFF);
            lstSendData.Add(0x03);
            //3.3.4. Request Destination Module Station No:
            lstSendData.Add((byte)this.stationNo);

            //3.4. Request Data Length: byte [7][8]
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);

            //3.5. Monitoring Timer:
            lstSendData.Add(0x10);
            lstSendData.Add(0x00);

            //3.6. Request Data:
            //3.6.1. Command: 1401
            lstSendData.Add(0x01);
            lstSendData.Add(0x14);
            //3.6.2 SubCommand: 0000
            lstSendData.Add(0x00);
            lstSendData.Add(0x00);

            //3.6.3. Head Device number: Đọc thanh ghi nào:
            byte[] headDevice = BitConverter.GetBytes(_devNumber);
            lstSendData.Add(headDevice[0]);
            lstSendData.Add(headDevice[1]);
            lstSendData.Add(headDevice[2]);

            //3.6.4. Device Code: Đọc thanh loại gì: D,W,ZR...
            lstSendData.Add((byte)_devCode);

            //3.6.5. Number of device point: Số lượng liên tiếp muốn đọc
            int devPoint = 2;
            byte[] arrDevPoint = BitConverter.GetBytes(devPoint);
            lstSendData.Add(arrDevPoint[0]);
            lstSendData.Add(arrDevPoint[1]);

            // 3.6.6. Data:
            byte[] arrData = BitConverter.GetBytes(_value);
            lstSendData.Add(arrData[0]);
            lstSendData.Add(arrData[1]);
            lstSendData.Add(arrData[2]);
            lstSendData.Add(arrData[3]);

            // 3.7. Tính request Data length:
            int reqDataL = lstSendData.Count - 2 - 5 - 2;
            byte[] arrReqDataLength = BitConverter.GetBytes(reqDataL);
            lstSendData[7] = arrReqDataLength[0];
            lstSendData[8] = arrReqDataLength[1];

            //Bước 4: Gửi dữ liệu đi:
            sock.Send(lstSendData.ToArray());

            //Bước 5: Nhận dữ liệu về:
            byte[] arrRcv = new byte[512];
            List<byte> lstRcv = new List<byte>();
            sock.Receive(arrRcv);
            lstRcv.AddRange(arrRcv);

            //Bước 6: Phân tích dữ liệu nhận về:
            //6.1. Sub Header:
            if (lstRcv[0] != 0xD0 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            //6.2. Access Route:
            //6.2.1. NetWork No:
            if (lstRcv[0] != (byte)this.netWorkNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            //6.2.2. PC No
            if (lstRcv[0] != (byte)this.pcNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);
            // 6.2.3. Request Destination Module IO NO:
            if (lstRcv[0] != 0xFF || lstRcv[1] != 0x03)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);
            //6.2.4. Request station No:
            if (lstRcv[0] != (byte)this.stationNo)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 1);

            //6.3. Request Data Length:
            short reqDataLength = BitConverter.ToInt16(new byte[] { lstRcv[0], lstRcv[1] }, 0);
            if (reqDataLength < 2)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            // 6.4. End Code:
            if (lstRcv[0] != 0x00 || lstRcv[1] != 0x00)
            {
                return kq;
            }
            lstRcv.RemoveRange(0, 2);

            kq = 0;
            return kq;
        }
        #endregion
    }
}
