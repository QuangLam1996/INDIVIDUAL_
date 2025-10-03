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
using System.Windows.Threading;
using System.Threading;

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
        EthernetClient ethernetClient = new EthernetClient();
        EthernetServer ethernetServer = new EthernetServer();
        Sock_SLMP sMLP = new Sock_SLMP();
        MyLogger logger = new MyLogger("Pg.Serial");
        MyCsv myCsv = new MyCsv("Pg.Serial");

        DispatcherTimer timer = new DispatcherTimer();
        bool isOpen = false;    
        
        public PgSerial()
        {
            InitializeComponent();

            this.Loaded += PgSerial_Loaded;
            this.Unloaded += PgSerial_Unloaded;

            this.btnOpen.Click += BtnOpen_Click;
            this.btnClose.Click += BtnClose_Click;
            this.btnSend.Click += BtnSend_Click;
            this.btnRecieve.Click += BtnRecieve_Click;

            this.btnEOpen.Click += BtnEOpen_Click;
            this.btnEClose.Click += BtnEClose_Click;
            this.btnESend.Click += BtnESend_Click;
            this.btnERecieve.Click += BtnERecieve_Click;

            this.btnSEOpen.Click += BtnSEOpen_Click;
            this.btnSEClose.Click += BtnSEClose_Click;
            this.btnSESend.Click += BtnSESend_Click;
            this.btnSERecieve.Click += BtnSERecieve_Click;

            this.btnMcOpen.Click += BtnMcOpen_Click;
            this.btnMcClose.Click += BtnMcClose_Click;

            this.btnSMLPOpen.Click += BtnSMLPOpen_Click;
            this.btnSMLPClose.Click += BtnSMLPClose_Click;
            this.btnSMLPReadWord.Click += BtnSMLPReadWord_Click;
            this.btnSMLPReadDWord.Click += BtnSMLPReadDWord_Click;
            this.btnSMLPReadFloat.Click += BtnSMLPReadFloat_Click;

            this.btnSMLPReadBit.Click += BtnSMLPReadBit_Click;
            this.btnSMLPReadMultiBit.Click += BtnSMLPReadMultiBit_Click;

            this.btnSMLPWriteWord.Click += BtnSMLPWriteWord_Click;
            this.btnSMLPWriteDWord.Click += BtnSMLPWriteDWord_Click;
            this.btnSMLPWriteFloat.Click += BtnSMLPWriteFloat_Click;
            this.btnSMLPWriteBit.Click += BtnSMLPWriteBit_Click;
            this.btnSMLPWriteMulBit.Click += BtnSMLPWriteMulBit_Click;

            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            short kq;
            kq = sMLP.ReadWord(Device.D, 100);
            txtSMLPRecieve.Text = kq.ToString();
            logger.CreateLog("ReadWord: " + kq.ToString());
            myCsv.Create(kq, "D100");
        }

        private void PgSerial_Unloaded(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }

        private void PgSerial_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void BtnSMLPWriteFloat_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnSMLPWriteDWord_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnSMLPWriteMulBit_Click(object sender, RoutedEventArgs e)
        {
            sMLP.WriteMultiBit(Device.M, 100, new List<bool>{true, false, true });

        }

        private void BtnSMLPWriteBit_Click(object sender, RoutedEventArgs e)
        {
            sMLP.WriteBit(Device.M, 100, true);
        }

        private void BtnSMLPWriteWord_Click(object sender, RoutedEventArgs e)
        {
            short value = Convert.ToInt16(txtSMLPWrite.Text);
            sMLP.WriteWord(Device.D,100,value);
        }

        private void BtnSMLPReadMultiBit_Click(object sender, RoutedEventArgs e)
        {
            List<bool> lstBit = sMLP.ReadMultiBit(Device.M, 100, 5);
            foreach(var item in lstBit)
            {
                MessageBox.Show(item.ToString());
            }
        }

        private void BtnSMLPReadBit_Click(object sender, RoutedEventArgs e)
        {
            bool kq = sMLP.ReadBit(Device.M, 100);
            MessageBox.Show(kq.ToString());
        }

        private void BtnSMLPReadFloat_Click(object sender, RoutedEventArgs e)
        {
            float kq;
            kq = sMLP.ReadFLoat(Device.D, 100);
            txtSMLPRecieve.Text = kq.ToString();

        }

        private void BtnSMLPReadDWord_Click(object sender, RoutedEventArgs e)
        {
            int kq;
            kq = sMLP.ReadDWord(Device.D, 100);
            txtSMLPRecieve.Text = kq.ToString();

        }

        private void BtnSMLPReadWord_Click(object sender, RoutedEventArgs e)
        {
            if (isOpen)
            {
                timer.Interval = TimeSpan.FromMilliseconds(500);
                timer.Start();
            }
        }

        private void BtnSMLPClose_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            logger.CreateLog("SMLP Close");

            int kq = sMLP.Disconnect();

            if (kq == 0) 
            { this.btnSMLPOpen.ClearValue(BackgroundProperty); }
            isOpen = false; 
        }

        private void BtnSMLPOpen_Click(object sender, RoutedEventArgs e)
        {
            logger.CreateLog("SMLP Open");
            int kq = sMLP.Connect();
            if (kq == 0) { this.btnSMLPOpen.Background = Brushes.Green; }
            isOpen = true;
        }

        #region Socket Client
        private void BtnSERecieve_Click(object sender, RoutedEventArgs e)
        {
            List<byte> data = new List<byte>();
            ethernetServer.RecieveData(out data);
            txtSERecieve.Text = Encoding.UTF8.GetString(data.ToArray());
        }

        private void BtnSESend_Click(object sender, RoutedEventArgs e)
        {
            string txtdata = txtSESend.Text;
            byte[] data = Encoding.UTF8.GetBytes(txtdata);
            ethernetServer.SendData(data);
        }

        private void BtnSEClose_Click(object sender, RoutedEventArgs e)
        {
            int kq = ethernetServer.Disconnect();

            if (kq == 0) { this.btnSEOpen.ClearValue(BackgroundProperty); }

        }

        private void BtnSEOpen_Click(object sender, RoutedEventArgs e)
        {
            int kq = ethernetServer.Listen();
            if (kq == 0) { this.btnSEOpen.Background = Brushes.Green; }

        }
        #endregion

        #region Socket Server
        private void BtnERecieve_Click(object sender, RoutedEventArgs e)
        {
            List<byte> data = new List<byte>();
            ethernetClient.RecieveData(out data);
            txtERecieve.Text = Encoding.UTF8.GetString(data.ToArray());
        }

        private void BtnESend_Click(object sender, RoutedEventArgs e)
        {
            string txtdata = txtESend.Text;
            byte[] data = Encoding.UTF8.GetBytes(txtdata);
            ethernetClient.SendData(data);
        }

        private void BtnEClose_Click(object sender, RoutedEventArgs e)
        {
            int kq = ethernetClient.Disconnect();

            if (kq == 0) { this.btnEOpen.ClearValue(BackgroundProperty); }

        }

        private void BtnEOpen_Click(object sender, RoutedEventArgs e)
        {
            int kq = ethernetClient.Connect();

            if (kq == 0) { this.btnEOpen.Background = Brushes.Green; }

        }
        #endregion

        #region MC Format5 Serial
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
        #endregion

        #region Non-Protocol
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
        #endregion
    }
}
