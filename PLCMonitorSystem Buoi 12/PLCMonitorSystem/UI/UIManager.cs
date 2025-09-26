using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;

namespace PLCMonitorSystem.UI
{
    class UIManager
    {
        private static Hashtable danhSachManHinh = new Hashtable();
        private static WndMain wndMain = new WndMain();

        public enum MaSoManHinh
        {
            PAGE_MAIN_ID = 1,

            PAGE_MENU_ID = 10,
            PAGE_SERIAL_ID = 11,
            PAGE_ETHERNET_ID = 12,
            PAGE_TEACHING_ID = 13,
            PAGE_MANUAL_ID = 14,
            PAGE_STATUS_ID = 15,
            PAGE_MODEL_ID = 16,
            PAGE_USER_ID = 17,

            PAGE_IO_ID = 20,

            PAGE_ALARM_ID = 30,

        }
        //public:  Phạm vi truy cập
        //void:    Kiểu Return
        //Initial: Tên Method
        //():      Parameter
        //{}:      Command
        public static void SwitchPage()
        {
            danhSachManHinh.Add(MaSoManHinh.PAGE_MAIN_ID, new PgMain());
            danhSachManHinh.Add(MaSoManHinh.PAGE_MENU_ID, new PgMenu());
            danhSachManHinh.Add(MaSoManHinh.PAGE_IO_ID, new PgIO());
            danhSachManHinh.Add(MaSoManHinh.PAGE_ALARM_ID, new PgAlarm());
            danhSachManHinh.Add(MaSoManHinh.PAGE_SERIAL_ID, new PgSerial());
        }
        public static void Switch_Pg(MaSoManHinh pgId)
        {
            if (danhSachManHinh.ContainsKey(pgId))
            {
                var pg = (Page)danhSachManHinh[pgId];
                wndMain.frmMain.Content = pg;
            }

            if (pgId == MaSoManHinh.PAGE_MAIN_ID)
            {
                wndMain.btnMain.Background = Brushes.Orange;
            }
            else
            {
                wndMain.btnMain.ClearValue(Button.BackgroundProperty);
            }

            if (pgId >= MaSoManHinh.PAGE_MENU_ID && pgId <= MaSoManHinh.PAGE_USER_ID)
            {
                wndMain.btnMenu.Background = Brushes.Orange;
            }
            else
            {
                wndMain.btnMenu.ClearValue(Button.BackgroundProperty);
            }

            if (pgId == MaSoManHinh.PAGE_IO_ID)
            {
                wndMain.btnIO.Background = Brushes.Orange;
            }
            else
            {
                wndMain.btnIO.ClearValue(Button.BackgroundProperty);
            }

            if (pgId == MaSoManHinh.PAGE_ALARM_ID)
            {
                wndMain.btnAlarm.Background = Brushes.Orange;
            }
            else
            {
                wndMain.btnAlarm.ClearValue(Button.BackgroundProperty);
            }
        }
        public static void Initial()
        {
            SwitchPage();
            //ChangeScreen(MaSoManHinh.PAGE_MAIN_ID);
            Switch_Pg(MaSoManHinh.PAGE_MAIN_ID);
            wndMain.ShowDialog();
        }
        public static void ChangeScreen(MaSoManHinh _maSoManHinh)
        {
            //B1: Lấy màn hình từ mã số màn hình
            var page = (Page)danhSachManHinh[_maSoManHinh];
            ////B2 Gán màn hình đã lấy gán vào khung
            //wndMain.frmMain.Content = page;
        }
    }
}
