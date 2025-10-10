using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace PLCMonitorSystem
{
    public class EthernetClient
    {
        //Field & Property
        Socket sock;
        string ipAddress;
        int port;

        public string IpAddress { get => ipAddress; set => ipAddress = value; }
        public int Port { get => port; set => port = value; }

        public EthernetClient()
        {
            this.IpAddress = "127.0.0.1";
            this.Port = 6000;
        }
        public EthernetClient(string ipAddress, int port)
        {
            this.IpAddress = ipAddress;
            this.Port = port;

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
        public int SendData(byte[] data)
        {
            int kq = -1;
            // B1: Kiểm tra đã khởi tạo 
            if (sock == null)
            {
                return kq;
            }
            // B2: Kiểm tra kết nối
            if (sock.Connected == false)
            {
                return kq;
            }
            // B3: Send
            try
            {
                sock.Send(data);
                kq = 0;
            }
            catch (Exception err) { }

            return kq;
        }
        public int RecieveData(out List<byte> data)
        {
            int kq = -1;
            data = new List<byte>();
            // B1: Kiểm tra đã khởi tạo 
            if (sock == null)
            {
                return kq;
            }
            // B2: Kiểm tra kết nối
            if (sock.Connected == false)
            {
                return kq;
            }
            // B3: Send
            try
            {
                byte[] arrbyte = new byte[1024];
                sock.Receive(arrbyte);
                data.AddRange(arrbyte);
                kq = 0;
            }
            catch (Exception err) { }

            return kq;
        }

    }
}
