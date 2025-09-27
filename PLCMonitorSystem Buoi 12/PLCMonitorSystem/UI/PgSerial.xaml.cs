using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PLCMonitorSystem.LIB;
using System.IO.Ports;

namespace PLCMonitorSystem.UI
{
    /// <summary>
    /// Interaction logic for PgSerial.xaml
    /// </summary>
    /// 
    public partial class PgSerial : Page
    {

        //NonProcedure nonProtocol = new NonProcedure("COM1", 8, StopBits.One, Parity.Odd, 9600);
        NonProcedure nonProtocol = new NonProcedure();
        MC_Format5 mcFormat5 = new MC_Format5();
        Ethernet ethernet = new Ethernet();
        public PgSerial()
        {
            InitializeComponent();
            this.btnOpen.Click += BtnOpen_Click;
            this.btnClose.Click += BtnClose_Click;
            this.btnSend.Click += BtnSend_Click;
            this.btnRecieve.Click += BtnRecieve_Click;

            this.btnEOpen.Click += BtnEOpen_Click;
            this.btnEClose.Click += BtnEClose_Click;
            this.btnESend.Click += BtnESend_Click;
            this.btnERecieve.Click += BtnERecieve_Click;

            this.btnMcOpen.Click += BtnMcOpen_Click;
            this.btnMcClose.Click += BtnMcClose_Click;
        }

        private void BtnERecieve_Click(object sender, RoutedEventArgs e)
        {
            List<byte> data = new List<byte>();
            ethernet.RecieveData(out data);
            txtERecieve.Text = Encoding.UTF8.GetString(data.ToArray());
        }

        private void BtnESend_Click(object sender, RoutedEventArgs e)
        {
            string txtdata = txtESend.Text;
            byte[] data = Encoding.UTF8.GetBytes(txtdata);
            ethernet.SendData(data);
        }

        private void BtnEClose_Click(object sender, RoutedEventArgs e)
        {
            int kq = ethernet.Disconnect();

            if (kq == 0) { this.btnEOpen.ClearValue(BackgroundProperty); }

            ethernet.Disconnect();
        }

        private void BtnEOpen_Click(object sender, RoutedEventArgs e)
        {
            int kq = ethernet.Connect();

            if (kq == 0) { this.btnEOpen.Background = Brushes.Green; }

        }

        private void BtnReadWord_Click(object sender, RoutedEventArgs e)
        {
            short kq;
            kq = mcFormat5.ReadWord(DevCode.D, 100);
            MessageBox.Show(kq.ToString());
        }

        private void BtnMcClose_Click(object sender, RoutedEventArgs e)
        {
            int kq = mcFormat5.Close();
            if (kq == 0)
            {
                this.btnMcOpen.ClearValue(BackgroundProperty);
            }
        }

        private void BtnMcOpen_Click(object sender, RoutedEventArgs e)
        {
            int kq = mcFormat5.Open();
            if (kq == 0)
            {
                this.btnMcOpen.Background = Brushes.Green;
            }
            else
            {
                MessageBox.Show("OPEN ERROR");
            }
        }

        private void BtnRecieve_Click(object sender, RoutedEventArgs e)
        {
            // B1: Nhận Data
            byte[] arrRcv = nonProtocol.Recieve();
            // B2: Chuyển Data từ byte[] ASCII sang String
            string strRcv = ASCIIEncoding.ASCII.GetString(arrRcv);
            // B3: Gán lên TextBox
            this.txtRecieve.Text = strRcv;
        }
        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            // B1: Chuẩn bị Data gửi
            string strSend = txtSend.Text;
            // B2: Chuyển string qua byte[] ASCII
            byte[] arrSend = ASCIIEncoding.ASCII.GetBytes(strSend);
            // B3: Gửi tới PLC
            nonProtocol.Send(arrSend);
        }
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            int kq = nonProtocol.Close();
            if (kq == 0)
            {
                this.btnOpen.ClearValue(BackgroundProperty);
            }
        }
        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            int kq = nonProtocol.Open();
            if (kq == 0)
            {
                this.btnOpen.Background = Brushes.Green;
            }
            else
            {
                MessageBox.Show("OPEN ERROR");
            }
        }
    }
}
