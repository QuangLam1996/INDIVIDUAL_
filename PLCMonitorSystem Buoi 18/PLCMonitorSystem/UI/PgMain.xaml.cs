using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Timers;
using PLCMonitorSystem.LIB;
using System.Windows.Threading;
using System.Reflection.Emit;
using System.Threading;


namespace PLCMonitorSystem.UI
{
    /// <summary>
    /// Interaction logic for PgMain.xaml
    /// </summary>
    /// 

    public partial class PgMain : Page
    {
        System.Timers.Timer timer = new System.Timers.Timer(100); // Tạo Timer

        MyLogger logger = new MyLogger("Pg.Main");
        Sock_SLMP sMLP = new Sock_SLMP();

        List<List<double>> lstValue = new List<List<double>>();
        List<List<string>> lstLabel = new List<List<string>>();
        int stt = 0;

        public PgMain()
        {
            InitializeComponent();

            timer.AutoReset = true; // Tự động lặp lại
            timer.Elapsed += Timer_Elapsed; // Tạo Event


            this.Loaded += PgMain_Loaded;
            this.Unloaded += PgMain_Unloaded;

            this.btnStart.Click += BtnStart_Click;
            this.btnStop.Click += BtnStop_Click;

        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            this.btnStart.ClearValue(BackgroundProperty);
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (sMLP.Connect() != 0)
            {
                return;
            }
            this.btnStart.Background = Brushes.LightGreen;
            timer.Start();
        }


        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            List D100;
            bool M100 = false;
            //D100 = sMLP.Re(Device.D, 100, 1);
            //M100 = sMLP.ReadBit(Device.M, 100);

            // Chuẩn bị data vẽ biểu đồ
            if (M100 == false)
            {
                //return;
            }
            //lstValue.Add();
            //stt++;
            //lstLabel.Add(stt.ToString());

            // Vẽ biểu đồ
            //LineChartViewModel line = new LineChartViewModel("Time", "Value", lstValue, lstLabel);
            //this.myChart.Model = line.MyModel;
        }

        private void PgMain_Loaded(object sender, RoutedEventArgs e)
        {
            int kq = sMLP.Connect();
            Thread.Sleep(100);

        }
        private void PgMain_Unloaded(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            int kq = sMLP.Disconnect();
        }

    }
}
