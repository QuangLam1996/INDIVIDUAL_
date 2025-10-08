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
using System.Windows.Shapes;

namespace PLCMonitorSystem.UI
{
    /// <summary>
    /// Interaction logic for WndAlert.xaml
    /// </summary>
    public partial class WndAlert : Window
    {
        private static int seqId = 0;

        private Brush BK_ON_COLOR = Brushes.DarkRed;
        private Brush BK_OFF_COLOR = Brushes.DarkCyan;

        private System.Timers.Timer timer = new System.Timers.Timer(1000);
        private int bgState = 0;

        public WndAlert(String msg = "", String solution = "", int code = 0, string mode = "AUTO")
        {
            InitializeComponent();

            this.Loaded += this.WndAlert_Loaded;
            this.Unloaded += this.WndAlert_Unloaded;
            this.btOk.Click += this.BtOk_Click;

            this.txtMessage.Text = msg;
            this.txtSolution.Text = solution;
            this.txtCode.Text = code.ToString();
            this.txtMode.Text = mode;
            this.txtTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // Increase SeqId:
            seqId++;
            this.txtSeqId.Text = seqId.ToString();
        }

        private void BtOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void WndAlert_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.timer.AutoReset = true;
                this.timer.Elapsed += this.Timer_Elapsed;
                this.timer.Start();
            }
            catch { }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(() => {
                try
                {
                    if (this.bgState == 0)
                    {
                        //this.lblContent.Background = BK_ON_COLOR;
                        this.bgState = 1;
                    }
                    else
                    {
                        //this.ClearValue(Window.BackgroundProperty);
                        //this.lblContent.Background = BK_OFF_COLOR;
                        this.bgState = 0;
                    }
                }
                catch { }
            });
        }

        private void WndAlert_Unloaded(object sender, RoutedEventArgs e)
        {
            this.timer.Stop();
        }
    }
}
