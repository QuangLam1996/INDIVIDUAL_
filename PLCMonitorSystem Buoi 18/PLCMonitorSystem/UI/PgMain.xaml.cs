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


namespace PLCMonitorSystem.UI
{
    /// <summary>
    /// Interaction logic for PgMain.xaml
    /// </summary>
    /// 

    public partial class PgMain : Page
    {
        Timer timer = new Timer(100); // Tạo Timer
        DispatcherTimer timer2 = new DispatcherTimer(); // Tạo Timer

        public PgMain()
        {
            InitializeComponent();

            timer.AutoReset = true; // Tự động lặp lại
            timer.Elapsed += Timer_Elapsed; // Tạo Event

            timer2.Interval = TimeSpan.FromMilliseconds(100); // Thêm giá trị 
            timer2.Tick += Timer2_Tick; // Tạo Event

            this.Loaded += PgMain_Loaded;
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            this.timer2.Stop();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.timer.Stop();
        }

        private void PgMain_Loaded(object sender, RoutedEventArgs e)
        {
            timer.Start(); // Bắt đầu chạy
            timer2.Start(); // Bắt đầu chạy
        }

    }
}
