using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
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
    /// Interaction logic for PgAlarm.xaml
    /// </summary>
    public partial class PgAlarm : Page
    {
        Timer tmr1 = new Timer(500);
        DispatcherTimer tmr2 = new DispatcherTimer();

        private static MyLogger logger = new MyLogger("Pg.Alarm");

        private const int ALARM_READ_LIMIT = 10;
        private static Brush BT_ACTIVE_BACKGROUND = Brushes.SkyBlue;

        private List<AlarmInfo> alarmList = new List<AlarmInfo>();
        private int selectedIndex = 0;

        public PgAlarm()
        {
            InitializeComponent();

            this.Loaded += PgAlarm_Loaded;
            this.Unloaded += PgAlarm_Unloaded;

            tmr1.AutoReset = true;
            tmr1.Elapsed += Tmr1_Elapsed;

            tmr2.Interval = TimeSpan.FromMilliseconds(500);
            tmr2.Tick += Tmr2_Tick;
        }

        private void PgAlarm_Unloaded(object sender, RoutedEventArgs e)
        {


        }

        private void updateAlarm(AlarmInfo alarm)
        {
            txtTime.Text = alarm.createdTime.ToString("yyyy-MM-dd HH:mm:ss.ff");
            txtCode.Text = alarm.alarmCode.ToString();
            txtSeqId.Text = alarm.id.ToString();
            txtMode.Text = alarm.getMode();
            txtMessage.Text = alarm.message;
            txtSolution.Text = alarm.solution;

            foreach (var obj in ugridJamList.Children)
            {
                var bt = obj as Button;
                if (bt != null)
                {
                    if ((int)bt.Tag == selectedIndex)
                    {
                        bt.Background = BT_ACTIVE_BACKGROUND;
                    }
                    else
                    {
                        bt.ClearValue(Button.BackgroundProperty);
                    }
                }
            }
        }

        private void PgAlarm_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ugridJamList.Children.Clear();
                alarmList = DbRead.GetLatestAlarms(30);
                for (int i = 0; i < alarmList.Count; i++)
                {
                    var alarm = alarmList[i];
                    var bt = new Button();
                    bt.Content = alarm.alarmCode.ToString();
                    bt.Tag = i;
                    bt.Click += this.Bt_Click;

                    ugridJamList.Children.Add(bt);
                }

                // Select first alarm:
                selectedIndex = 0;
                if (alarmList.Count > 0)
                {
                    updateAlarm(alarmList[0]);
                }
            }
            catch (Exception ex)
            {
                logger.CreateLog("PgLastJam_Loaded error:" + ex.Message);
            }
        }

        private void Tmr2_Tick(object sender, EventArgs e)
        {
            ShowData();
        }

        private void Tmr1_Elapsed(object sender, ElapsedEventArgs e)
        {
            

        }

        private void BtnShow_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Bt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var bt = (Button)sender;
                selectedIndex = (int)bt.Tag;
                var alarm = alarmList[selectedIndex];
                updateAlarm(alarm);
            }
            catch (Exception ex)
            {
                logger.CreateLog("Bt_Click error:" + ex.Message);
            }
        }

        private void BtnInsert_Click(object sender, RoutedEventArgs e)
        {
            // B1: Kết nối đến CSDL SQLite
            string conString = String.Format("Data Source = test.db; Version = 3; New = True; Compress = True;");
            Random rnd = new Random();
            int val = rnd.Next(1, 100);
            string time = DateTime.Now.ToString("T");
            using (SQLiteConnection conn = new SQLiteConnection(conString))
            {
                // B2: Mở kết nối
                conn.Open();

                // B3: Tạo câu lệnh SQL để chèn dữ liệu
                string query = String.Format("INSERT INTO apsuat (giatri, thoigian) VALUES({0},'{1}');", val, time);

                // B4: Thực thi câu lệnh SQL
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
        private void ShowData()
        {
            // B1: Kết nối đến CSDL SQLite
            //string conString = String.Format("Data Source=test.db;Version=3;New=True;Compress=True");
            string conString = String.Format("Data Source = qr1.db; Version = 3; New = True; Compress = True");
            using (SQLiteConnection conn = new SQLiteConnection(conString))
            {
                // B2: Mở kết nối
                conn.Open();

                // B3: Tạo câu lệnh SQL để chèn dữ liệu
                string query = String.Format("SELECT * FROM alarm_log ORDER BY id DESC;");

                // B4: Thực thi câu lệnh SQL
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                cmd.ExecuteNonQuery();

                // Show Data Grid
                DataTable dtTable = new DataTable();
                SQLiteDataAdapter dtAdapter = new SQLiteDataAdapter(cmd);
                dtAdapter.Fill(dtTable);

                //this.dtGrid.ItemsSource = dtTable.DefaultView;

                conn.Close();
            }
        }
    }
}
