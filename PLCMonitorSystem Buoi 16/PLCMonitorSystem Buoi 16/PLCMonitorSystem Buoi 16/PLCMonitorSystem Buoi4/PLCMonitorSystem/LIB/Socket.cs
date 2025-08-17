using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;

namespace MTECH
{
    public class Ethernet
    {
        //Field Property:
        Socket sock;
        String ipAddress;
        int port;

        public string IpAddress { get => ipAddress; set => ipAddress = value; }
        public int Port { get => port; set => port = value; }

        // Method:
        public Ethernet()
        {
            this.ipAddress = "192.168.3.39";//Địa chỉ ip của plc
            this.port = 1025;
        }
        public Ethernet(string _ipAddress, int _port)
        {
            this.ipAddress= _ipAddress;
            this.port = _port;
        }
       
        // Kết nối:
        public int Open()
        {
            int kq = -1;
            // B1: Kiểm tra Sock đã được khởi tạo:
            if (sock==null)
            {
                sock = new Socket(SocketType.Stream, ProtocolType.Tcp);
            }
            //B2: Kiểm tra kết nối:
            if (sock.Connected==true)
            {
                kq = 0;
                return kq;
            }
            //B3: Kết nối:
            try
            {
                sock.Connect(this.ipAddress, this.port);
                if (sock.Connected==true)
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
            if (sock==null)
            {
                return kq;
            }
            if (sock.Connected==false)
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
        public int SendData(byte[] data)
        {
            int kq = -1;
            // B1: Kiểm tra đã khởi tạo sock chưa
            if (sock==null)
            {
                return kq;
            }
            // B2: Kiểm tra đã kết nối hay chưa:
            if (sock.Connected == false)
            {
                return kq;
            }
            // B3: Gửi:
            try
            {
                sock.Send(data);
                kq = 0;
            }
            catch (Exception err)
            {
            }
            return kq;
        }
        public int RecieveData(out List<byte> data)
        {
            int kq = -1;
            data = new List<byte>();
            //B1: Kiểm tra đã khởi tạo chưa:
            if (sock==null)
            {
                return kq;
            }
            //B2: Kiểm tra đã kết nối:
            if (sock.Connected==false)
            {
                return kq;
            }
            //B3: Nhận
            try
            {
                byte[] rcvData = new byte[512];
                sock.Receive(rcvData);
                data.AddRange(rcvData);
                kq = 0;
            }
            catch (Exception err)
            {
            }
            return kq;
        }
    }
}
