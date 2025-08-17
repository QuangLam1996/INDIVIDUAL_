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
        public PgSerial()
        {
            InitializeComponent();
            this.btnOpen.Click += BtnOpen_Click;
            this.btnClose.Click += BtnClose_Click;
            this.btnSend.Click += BtnSend_Click;
            this.btnRecieve.Click += BtnRecieve_Click;

            this.btnMcOpen.Click += BtnMcOpen_Click;
            this.btnMcClose.Click += BtnMcClose_Click;
            this.btnReadWord.Click += BtnReadWord_Click;
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
            MessageBox.Show("OPEN GOOD");
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
