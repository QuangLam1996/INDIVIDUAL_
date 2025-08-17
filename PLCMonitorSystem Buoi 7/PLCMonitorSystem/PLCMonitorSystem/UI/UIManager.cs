using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PLCMonitorSystem.UI
{
    class UIManager
    {
        public enum MaSoManHinh
        {
            PAGE_MAIN_ID,
            PAGE_MENU_ID,
            PAGE_IO_ID,
            PAGE_ALARM_ID,

            PAGE_SERIAL_ID,
        }
        private static Hashtable danhSachManHinh = new Hashtable();
        private static WndMain wndMain;

        //public:  Phạm vi truy cập
        //void:    Kiểu Return
        //Initial: Tên Method
        //():      Parameter
        //{}:      Command
        public static void Initial()
        {
            SwitchPage();
            wndMain = new WndMain();
            ChangeScreen(MaSoManHinh.PAGE_MAIN_ID);
            wndMain.ShowDialog();
        }
        public static void SwitchPage()
        {
            danhSachManHinh.Add(MaSoManHinh.PAGE_MAIN_ID, new PgMain());
            danhSachManHinh.Add(MaSoManHinh.PAGE_MENU_ID, new PgMenu());
            danhSachManHinh.Add(MaSoManHinh.PAGE_IO_ID, new PgIO());
            danhSachManHinh.Add(MaSoManHinh.PAGE_ALARM_ID, new PgAlarm());
            danhSachManHinh.Add(MaSoManHinh.PAGE_SERIAL_ID, new PgSerial());
        }
        public static void ChangeScreen(MaSoManHinh _maSoManHinh)
        {
            //B1: Lấy màn hình từ mã số màn hình
            var page = (Page)danhSachManHinh[_maSoManHinh];
            ////B2 Gán màn hình đã lấy gán vào khung
            wndMain.frmMain.Content = page;
        }

    }
}
