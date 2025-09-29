using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace PLCMonitorSystem.LIB
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
        string ipAddress;
        int port;

        int stationNo;
        int netWorkNo;
        int pcNo;

        public string IpAddress { get => ipAddress; set => ipAddress = value; }
        public int Port { get => port; set => port = value; }
        public int StationNo { get => stationNo; set => stationNo = value; }
        public int NetWorkNo { get => netWorkNo; set => netWorkNo = value; }
        public int PcNo { get => pcNo; set => pcNo = value; }

        // Method
        public Sock_SLMP()
        {
            this.IpAddress = "127.0.0.1";
            this.Port = 6000;
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
            List<byte> lstDensData = new List<byte>();
            // 3.1 Header: Đã tự động thêm vào <Skip>
            // 3.2 Subheader [2byte]
            lstDensData.Add(0x50);
            lstDensData.Add(0x00);

            // 3.3 Access Route
            // 3.3.1 Network No
            lstDensData.Add((byte)this.NetWorkNo);
            // 3.3.2 PC No
            lstDensData.Add((byte)this.PcNo);
            // 3.3.3 Request Destination Module IO
            lstDensData.Add(0xFF);
            lstDensData.Add(0x03);
            // 3.3.4 Request Destination Module Station
            lstDensData.Add((byte)this.StationNo);

            // 3.4 Request Data Length [byte7 - byte8]
            lstDensData.Add(0x00);
            lstDensData.Add(0x00);

            // 3.5 Monitoring Time
            lstDensData.Add(0x10);
            lstDensData.Add(0x00);

            // 3.6 Request Data
            // 3.6.1 Command [0401]
            lstDensData.Add(0x01);
            lstDensData.Add(0x04);
            // 3.6.2 Sub command [0000]
            lstDensData.Add(0x00);
            lstDensData.Add(0x00);
            // 3.6.3 Head Device Number
            byte[] headDevice = BitConverter.GetBytes(_devNumber);
            lstDensData.Add(headDevice[0]);
            lstDensData.Add(headDevice[1]);
            lstDensData.Add(headDevice[2]);
            // 3.6.4 Device Code
            lstDensData.Add((byte)_devCode);
            // 3.6.5 Number Of Device Point
            int devPoint = 1;
            byte[] arrDevPoint = BitConverter.GetBytes(devPoint);
            lstDensData.Add(arrDevPoint[0]);
            lstDensData.Add(arrDevPoint[1]);

            // 3.7 Send Data
            sock.Send










            return kq;
        }

    }
}
