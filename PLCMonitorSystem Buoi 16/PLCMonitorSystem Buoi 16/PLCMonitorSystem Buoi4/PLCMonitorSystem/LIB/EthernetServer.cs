using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace MTECH
{
    public class EthernetServer
    {
        // Property:
        Socket sock;
        TcpListener listener;
        string ipAddress;
        int port;

        public string IpAddress { get => ipAddress; set => ipAddress = value; }
        public int Port { get => port; set => port = value; }
        
        // Method:
        public EthernetServer()
        {
            this.ipAddress = "127.0.0.1";
            this.port = 1025;
        }
        public int ListenPort()
        {
            int kq = -1;
            // B1: Kiểm tra đã khởi tạo chưa:
            if (listener ==null)
            {
                listener = new TcpListener(IPAddress.Parse(this.ipAddress), port);
            }
            try
            {
                listener.Start();
                sock = listener.AcceptSocket();
                kq = 0;
            }
            catch (Exception err)
            {
            }
            return kq;
        }
        public int Close()
        {
            int kq = -1;
            //B1:
            if (listener==null)
            {
                return kq;
            }
            //B2:
            try
            {
                sock.Close();
                sock = null;
                listener.Stop();
                listener = null;
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
            if (sock == null)
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
            if (sock == null)
            {
                return kq;
            }
            //B2: Kiểm tra đã kết nối:
            if (sock.Connected == false)
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
