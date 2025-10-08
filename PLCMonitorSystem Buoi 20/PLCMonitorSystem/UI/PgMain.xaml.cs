using PLCMonitorSystem.DAO;
using PLCMonitorSystem.DATA;
using PLCMonitorSystem.LIB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;



namespace PLCMonitorSystem.UI
{
    /// <summary>
    /// Interaction logic for PgMain.xaml
    /// </summary>
    /// 

    public partial class PgMain : Page
    {
        //System.Timers.Timer timer = new System.Timers.Timer(1000); // Tạo Timer
        System.Windows.Threading.DispatcherTimer timer1 = new DispatcherTimer();

        MyLogger logger = new MyLogger("Pg.Main");
        Sock_SLMP sMLP = new Sock_SLMP();

        List<double> lstValue1 = new List<double>();
        List<string> lstLabel1 = new List<string>();
        List<double> lstValue2 = new List<double>();
        List<string> lstLabel2 = new List<string>();
        int stt = 0;

        public PgMain()
        {
            InitializeComponent();

            //timer.AutoReset = true; // Tự động lặp lại
            //timer.Elapsed += Timer_Elapsed; // Tạo Event

            timer1.Interval = TimeSpan.FromMilliseconds(1000);
            timer1.Tick += Timer1_Tick;

            this.Loaded += PgMain_Loaded;
            this.Unloaded += PgMain_Unloaded;

            this.btnStart.Click += BtnStart_Click;
            this.btnStop.Click += BtnStop_Click;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            //B1: Đọc giá trị của thanh ghi trong PLC:
            //List<short> value = new List<short>();
            //value = sMLP.ReadMultiWord(Device.D, 100, 2);
            //bool bitValue;
            //bitValue = sMLP.ReadBit(Device.M, 100);

            Random rnd = new Random();
            short[] value = new short[2];
            value[0] = (short)rnd.Next(0, 99);
            value[1] = (short)rnd.Next(0, 99);

            // B2: Chuẩn bị dữ liệu vẽ biểu đồ:
            //if (bitValue == false)
            //{
            //    return;
            //}

            stt++;
            lstValue1.Add(value[0]);//D100
            lstValue2.Add(value[1]);//D101
            lstLabel1.Add(stt.ToString());
            lstLabel2.Add(stt.ToString());

            //B3: Xoa di phan tu so 0 neu so luong phan tu lon hon 200
            if (lstValue1.Count > 100)
            {
                lstValue1.RemoveAt(0);
                lstValue2.RemoveAt(0);
                lstLabel1.RemoveAt(0);
                lstLabel2.RemoveAt(0);
            }

            //B4: Vẽ biểu đồ:
            LineChartViewModel line1 = new LineChartViewModel("Time", "Value", lstValue1, lstLabel1);
            myChart1.Model = line1.MyModel;

            LineChartViewModel line2 = new LineChartViewModel("Time", "Value", lstValue2, lstLabel2);
            myChart2.Model = line2.MyModel;
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            //timer.Stop();
            timer1.Stop();
            this.btnStart.ClearValue(BackgroundProperty);
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (sMLP.Connect() != 0)
            {
                return;
            }
            this.btnStart.Background = Brushes.LightGreen;
            displayAlarm(AlarmInfo.getMessage(AlarmInfo.NEW_ALARM_ID), 100, AlarmInfo.NEW_ALARM_ID);

            //timer.Start();
            //timer1.Start();
        }


        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {

        }

        private void PgMain_Loaded(object sender, RoutedEventArgs e)
        {
            //int kq = sMLP.Connect();
            //timer1.Start();
            //Thread.Sleep(100); 
        }
        private void PgMain_Unloaded(object sender, RoutedEventArgs e)
        {
            //timer.Stop();
            timer1.Stop();
            int kq = sMLP.Disconnect();
        }

        private void displayAlarm(String msg, int _bit, int code = 0)
        {
            try
            {
                var isAlarming = true;

                // Createlog:
                var mode = AlarmInfo.MODE_AUTO;

                var alarm = new AlarmInfo(mode, code, msg, AlarmInfo.getSolution(code));
                DbWrite.createAlarm(alarm);

                //int bit= default;
                // Set Alarm Bit:
                sMLP.WriteBit(Device.M, _bit, true);

                // Display Alarm:
                this.Dispatcher.Invoke(() =>
                {
                    var solution = AlarmInfo.getSolution(code);
                    var wnd = new WndAlert(msg, solution, code, alarm.getMode());
                    wnd.ShowDialog();
                    isAlarming = false;
                });

                // Wait user click OK:
                while (isAlarming)
                {
                    Thread.Sleep(100);
                }

                // Clear Alarm bit:
                sMLP.WriteBit(Device.M, _bit, false);

            }
            catch (Exception ex)
            {
                logger.CreateLog("displayAlarm error:" + ex.Message);
            }
        }

    }
}
