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
using MTECH;
using PLCMonitorSystem.LIB;

using ActUtlTypeLib;

namespace PLCMonitorSystem.UI
{
    /// <summary>
    /// Interaction logic for PgSerial.xaml
    /// </summary>
    public partial class PgSerial : Page
    {
        NonProcedure plc = new NonProcedure("COM1", 8, System.IO.Ports.StopBits.One, System.IO.Ports.Parity.Odd, 9600);
        
        MCF5 mc = new MCF5("COM9", 8, System.IO.Ports.StopBits.One, 9600, System.IO.Ports.Parity.Odd);

        Ethernet ethernet = new Ethernet();

        EthernetServer server = new EthernetServer();

        SLMP slmp = new SLMP("192.168.3.39", 1025);

        MyLogger logger = new MyLogger("Page Serial");

        ActUtlType mx = new ActUtlType();

        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        bool isOpen = false;

        MyCsv cvs = new MyCsv("Page Serial");

        public PgSerial()
        {
            InitializeComponent();
            this.btnOpen.Click += BtnOpen_Click;
            this.btnClose.Click += BtnClose_Click;
            this.btnSendData.Click += BtnSendData_Click;
            this.btnRcvData.Click += BtnRcvData_Click;
            this.btnMcOpen.Click += BtnMcOpen_Click;
            this.btnMcClose.Click += BtnMcClose_Click;
            this.btnReadWord.Click += BtnReadWord_Click;
            this.btnReadDoubleWord.Click += BtnReadDoubleWord_Click;
            this.btnReadFloat.Click += BtnReadFloat_Click;
            this.btnReadBit.Click += BtnReadBit_Click;
            this.btnReadMultiBit.Click += BtnReadMultiBit_Click;
            this.btnWriteBitON.Click += BtnWriteBitON_Click;
            this.btnWriteBitOFF.Click += BtnWriteBitOFF_Click;
            this.btnWriteMultiBitON.Click += BtnWriteMultiBitON_Click;
            this.btnWriteMultiBitOFF.Click += BtnWriteMultiBitOFF_Click;
            this.btnWriteWord.Click += BtnWriteWord_Click;
            this.btnWriteMultiWord.Click += BtnWriteMultiWord_Click;
            this.btnWriteDoubleWord.Click += BtnWriteDoubleWord_Click;
            this.btnWriteFloat.Click += BtnWriteFloat_Click;


            // EThernet
            this.btnEOpen.Click += BtnEOpen_Click;
            this.btnEClose.Click += BtnEClose_Click;
            this.btnESend.Click += BtnESend_Click;
            this.btnERecieve.Click += BtnERecieve_Click;

            //Server:
            this.btnSEOPEN.Click += BtnSEOPEN_Click;
            this.btnSECLOSE.Click += BtnSECLOSE_Click;
            this.btnSESEND.Click += BtnSESEND_Click;
            this.btnSERECEIVE.Click += BtnSERECEIVE_Click;

            // MC Protocol:
            this.btnMCOpen.Click += BtnMCOpen_Click;
            this.btnMCClose.Click += BtnMCClose_Click;
            this.btnMCReadWord.Click += BtnMCReadWord_Click;
            this.btnMCReadDoubleWord.Click += BtnMCReadDoubleWord_Click;
            this.btnMCReadFloat.Click += BtnMCReadFloat_Click;
            this.btnMCReadBit.Click += BtnMCReadBit_Click;
            this.btnMCReadMultiBit.Click += BtnMCReadMultiBit_Click;
            this.btnMCWriteWord.Click += BtnMCWriteWord_Click;
            this.btnMCWriteBit.Click += BtnMCWriteBit_Click;
            this.chbMCWriteBit.Checked += ChbMCWriteBit_Checked;
            this.chbMCWriteBit.Unchecked += ChbMCWriteBit_Unchecked;
            this.btnMCWriteMultiBit.Click += BtnMCWriteMultiBit_Click;

            // MX:
            this.btnMXOpen.Click += BtnMXOpen_Click;
            this.btnMXClose.Click += BtnMXClose_Click;
            this.btnMXStart.Click += BtnMXStart_Click;
            this.btnMXStop.Click += BtnMXStop_Click;
            this.Unloaded += PgSerial_Unloaded;
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            short giaTri = 0;
            mx.ReadDeviceBlock2("D1000", 1, out giaTri);
            cvs.Create(giaTri, "Vi tri so 1");
        }

        private void PgSerial_Unloaded(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            mx.Close();
        }

        private void BtnMXStop_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }

        private void BtnMXStart_Click(object sender, RoutedEventArgs e)
        {
            if (isOpen)
            {
                timer.Interval = TimeSpan.FromMilliseconds(100);//100ms
                timer.Start();
            }
        }

        private void BtnMXClose_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            mx.Close();
            this.btnMXOpen.ClearValue(BackgroundProperty);
            isOpen = false;
        }

        private void BtnMXOpen_Click(object sender, RoutedEventArgs e)
        {
            mx.ActLogicalStationNumber = 1;
            int kq =  mx.Open();
            if (kq==0)// Mở thành công
            {
                this.btnMXOpen.Background = Brushes.GreenYellow;
                isOpen = true;
            }
        }

        private void BtnMCWriteMultiBit_Click(object sender, RoutedEventArgs e)
        {
            if (chbMCWriteBit.IsChecked == true)
            {
                slmp.WriteMultiBit(DevCode.M, 10, new List<bool> { true,true,true,true,true});
            }
            else
            {
                slmp.WriteMultiBit(DevCode.M, 10, new List<bool> { false,false, false, false, false });
            }
        }

        private void ChbMCWriteBit_Unchecked(object sender, RoutedEventArgs e)
        {
            slmp.WriteBit(DevCode.M, 10, false);
        }

        private void ChbMCWriteBit_Checked(object sender, RoutedEventArgs e)
        {
            slmp.WriteBit(DevCode.M, 10, true);
        }

        private void BtnMCWriteBit_Click(object sender, RoutedEventArgs e)
        {
            if (chbMCWriteBit.IsChecked==true)
            {
                slmp.WriteBit(DevCode.M, 10, true);
            }
            else
            {
                slmp.WriteBit(DevCode.M, 10, false);
            }
        }

        private void BtnMCWriteWord_Click(object sender, RoutedEventArgs e)
        {
            short value = Convert.ToInt16(txtMCWriteWord.Text);
            slmp.WriteWord(DevCode.D, 10, value);
        }

        private void BtnMCReadMultiBit_Click(object sender, RoutedEventArgs e)
        {
            List<bool> lstKq = slmp.ReadMultiBit(DevCode.M, 10, 5);
            foreach (var item in lstKq)
            {
                MessageBox.Show(item.ToString());
            }
        }

        private void BtnMCReadBit_Click(object sender, RoutedEventArgs e)
        {
            bool kq = slmp.ReadBit(DevCode.SM, 412);
            MessageBox.Show(kq.ToString());
        }

        private void BtnMCReadFloat_Click(object sender, RoutedEventArgs e)
        {
            float kq = slmp.ReadFloat(DevCode.D, 13);
            MessageBox.Show(kq.ToString());
        }

        private void BtnMCReadDoubleWord_Click(object sender, RoutedEventArgs e)
        {
            int kq = slmp.ReadDoubleWord(DevCode.D, 11);
            MessageBox.Show(kq.ToString());
        }

        private void BtnMCReadWord_Click(object sender, RoutedEventArgs e)
        {
            short kq = slmp.ReadWord(DevCode.D, 10);
            MessageBox.Show(kq.ToString());
        }

        private void BtnMCClose_Click(object sender, RoutedEventArgs e)
        {
            slmp.Close();
        }

        private void BtnMCOpen_Click(object sender, RoutedEventArgs e)
        {
            slmp.Open();
        }

        private void BtnSERECEIVE_Click(object sender, RoutedEventArgs e)
        {
            List<byte> data = new List<byte>();
            server.RecieveData(out data);
            MessageBox.Show(ASCIIEncoding.ASCII.GetString(data.ToArray()));
        }

        private void BtnSESEND_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = ASCIIEncoding.ASCII.GetBytes("MTECH");
            server.SendData(data);
        }

        private void BtnSECLOSE_Click(object sender, RoutedEventArgs e)
        {
            server.Close();
        }

        private void BtnSEOPEN_Click(object sender, RoutedEventArgs e)
        {
            server.ListenPort();
        }

        private void BtnERecieve_Click(object sender, RoutedEventArgs e)
        {
            List<byte> data = new List<byte>();
            ethernet.RecieveData(out data);
            MessageBox.Show(ASCIIEncoding.ASCII.GetString(data.ToArray()));
        }

        private void BtnESend_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = ASCIIEncoding.ASCII.GetBytes("NAM VUONG");
            ethernet.SendData(data);
        }

        private void BtnEClose_Click(object sender, RoutedEventArgs e)
        {
            ethernet.Close();
        }

        private void BtnEOpen_Click(object sender, RoutedEventArgs e)
        {
            ethernet.Open();
        }

        private void BtnWriteFloat_Click(object sender, RoutedEventArgs e)
        {
            mc.WriteFloat(DevCode.D, 10, 123.45678f);
            mc.WriteFloat(DevCode.D, 10, 4444.5555f);
        }

        private void BtnWriteDoubleWord_Click(object sender, RoutedEventArgs e)
        {
            mc.WriteDoubleWord(DevCode.D, 10, 123456789);
        }

        private void BtnWriteMultiWord_Click(object sender, RoutedEventArgs e)
        {
            List<short> lstValue = new List<short>() { 122, 456, 01, 02, 03 };
            mc.WriteMultiWord(DevCode.D, 10, lstValue);
        }

        private void BtnWriteWord_Click(object sender, RoutedEventArgs e)
        {
            mc.WriteWord(DevCode.D, 10, 123);
        }

        private void BtnWriteMultiBitOFF_Click(object sender, RoutedEventArgs e)
        {
            List<bool> lstValue = new List<bool>() { false, false, false, false, false };
            mc.WriteMultiBit(DevCode.Y, 0x40, lstValue);
        }

        private void BtnWriteMultiBitON_Click(object sender, RoutedEventArgs e)
        {
            List<bool> lstValue = new List<bool>() {true,false,true,false,true };
            mc.WriteMultiBit(DevCode.Y, 0x40, lstValue);
        }

        private void BtnWriteBitOFF_Click(object sender, RoutedEventArgs e)
        {
            mc.WriteBit(DevCode.Y,0x40, false);
        }

        private void BtnWriteBitON_Click(object sender, RoutedEventArgs e)
        {
            mc.WriteBit(DevCode.Y, 0x40,true);
        }
        private void BtnReadMultiBit_Click(object sender, RoutedEventArgs e)
        {
            List<bool> lstKq = mc.ReadMultiBit(DevCode.M, 10, 3);
            foreach (var item in lstKq)
            {
                MessageBox.Show(item.ToString());
            }
        }
        private void BtnReadBit_Click(object sender, RoutedEventArgs e)
        {
            bool bit = mc.ReadBit(DevCode.M, 10);
            MessageBox.Show(bit.ToString());
        }

        private void BtnReadFloat_Click(object sender, RoutedEventArgs e)
        {
            float ketQua = mc.ReadFloat(DevCode.D, 104);
            MessageBox.Show(ketQua.ToString());
        }

        private void BtnReadDoubleWord_Click(object sender, RoutedEventArgs e)
        {
           int ketqua = mc.ReadDoubleWord(DevCode.D, 102);
            MessageBox.Show(ketqua.ToString());
        }

        private void BtnReadWord_Click(object sender, RoutedEventArgs e)
        {
            short ketqua = mc.ReadWord(DevCode.D, 0);
            MessageBox.Show(ketqua.ToString());
        }

        private void BtnMcClose_Click(object sender, RoutedEventArgs e)
        {
            mc.Close();
        }

        private void BtnMcOpen_Click(object sender, RoutedEventArgs e)
        {
            mc.Open();
        }

        private void BtnRcvData_Click(object sender, RoutedEventArgs e)
        {
            //B1: Nhận dữ liệu
            byte[] arrRcv = plc.Recieve();
            //B2: Chuyển dữ liệu từ mảng byte ASCII thành chuỗi kí tự string:
            string strRcv = ASCIIEncoding.ASCII.GetString(arrRcv);
            //B3: Gán lên textbox:
            this.txtRcvData.Text = strRcv;
        }

        private void BtnSendData_Click(object sender, RoutedEventArgs e)
        {
            //B1: Chuẩn bị dữ liệu gửi:
            string strSend = txtSendData.Text;
            //B2: Chuyển chuỗi string về mảng byte ASCII:
            byte[] arrSend = ASCIIEncoding.ASCII.GetBytes(strSend);
            //B3: Gửi dữ liệu xuống PLC
            plc.Send(arrSend);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            int Ketqua = plc.Close();
            if (Ketqua==0)
            {
                this.btnOpen.ClearValue(BackgroundProperty);
            }
            logger.CreateLog("Nguoi dung da bam nut Close");
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            int KetQua = plc.Open();
            if (KetQua==0)
            {
                this.btnOpen.Background = Brushes.GreenYellow;
            }
            logger.CreateLog("Nguoi dung da bam nut Open");
        }
    }
}
