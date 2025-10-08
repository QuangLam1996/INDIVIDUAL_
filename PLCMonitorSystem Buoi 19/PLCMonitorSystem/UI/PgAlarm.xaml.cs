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
using System.Timers;
using System.Windows.Threading;
using System.Data.SQLite;
using System.Data;

namespace PLCMonitorSystem.UI
{
    /// <summary>
    /// Interaction logic for PgAlarm.xaml
    /// </summary>
    public partial class PgAlarm : Page
    {
        Timer tmr1 = new Timer(500);
        DispatcherTimer tmr2 = new DispatcherTimer();
        public PgAlarm()
        {
            InitializeComponent();
            this.btnInsert.Click += BtnInsert_Click;
            this.btnShow.Click += BtnShow_Click;

            this.Loaded += PgAlarm_Loaded;
            this.Unloaded += PgAlarm_Unloaded;

            tmr1.AutoReset = true;
            tmr1.Elapsed += Tmr1_Elapsed;

            tmr2.Interval = TimeSpan.FromMilliseconds(500);
            tmr2.Tick += Tmr2_Tick;
        }

        private void PgAlarm_Unloaded(object sender, RoutedEventArgs e)
        {
            tmr1.Stop();
            tmr2.Stop();
        }

        private void PgAlarm_Loaded(object sender, RoutedEventArgs e)
        {
            tmr1.Start();
            tmr2.Start();

        }

        private void Tmr2_Tick(object sender, EventArgs e)
        {
            ShowData();


        }

        private void Tmr1_Elapsed(object sender, ElapsedEventArgs e)
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

        private void BtnShow_Click(object sender, RoutedEventArgs e)
        {
            ShowData();
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
            string conString = String.Format("Data Source=test.db;Version=3;New=True;Compress=True");
            using (SQLiteConnection conn = new SQLiteConnection(conString))
            {
                // B2: Mở kết nối
                conn.Open();

                // B3: Tạo câu lệnh SQL để chèn dữ liệu
                string query = String.Format("SELECT * FROM apsuat ORDER BY id DESC;");

                // B4: Thực thi câu lệnh SQL
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                cmd.ExecuteNonQuery();

                // Show Data Grid
                DataTable dtTable = new DataTable();
                SQLiteDataAdapter dtAdapter = new SQLiteDataAdapter(cmd);
                dtAdapter.Fill(dtTable);

                this.dtGrid.ItemsSource = dtTable.DefaultView;

                conn.Close();

            }
        }
    }
}
