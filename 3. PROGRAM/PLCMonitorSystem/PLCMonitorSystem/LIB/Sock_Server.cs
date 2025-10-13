using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace PLCMonitorSystem
{
    public class EthernetServer
    {
        // Property
        Socket sock;
        TcpListener listener;
        string ipAdress;
        int port;

        public string IpAdress { get => ipAdress; set => ipAdress = value; }
        public int Port { get => port; set => port = value; }

        // Method
        public EthernetServer()
        {
            this.IpAdress = "127.0.0.1";
            this.Port = 6000;
        }

        public int Listen()
        {
            int kq = -1;
            // B1: Kiểm tra khởi tạo
            if (listener == null)
            {
                listener = new TcpListener(IPAddress.Parse(this.IpAdress), Port);
            }

            try
            {
                listener.Start();
                sock = listener.AcceptSocket();
                kq = 0;
            }
            catch (Exception err) { }


            return kq;
        }

        public int Disconnect()
        {
            int kq = -1;
            if (listener == null)
            {
                return kq;
            }
            try
            {
                sock.Close();
                sock = null;
                listener.Stop();
                listener = null;
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
