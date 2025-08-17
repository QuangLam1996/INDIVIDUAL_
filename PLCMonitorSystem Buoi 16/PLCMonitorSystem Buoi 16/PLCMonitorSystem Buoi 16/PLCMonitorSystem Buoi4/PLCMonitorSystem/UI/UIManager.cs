using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCMonitorSystem.UI
{
     class UIManager
    {
        public enum MaSoManHinh
        {
            PAGE_MAIN_ID  = 111,
            PAGE_MENU_ID  = 222,
            PAGE_IO_ID    = 333,
            PAGE_ALARM_ID = 444,
            PAGE_SERIAL_ID = 555,
        }
        private static Hashtable danhSachManHinh = new Hashtable();
        private static WndMain wndMain;
        // public: Phạm vi truy cập
        // void: Kiểu trả về
        // KhoiTao : Tên của method (phương thức, hàm)
        //() Bên trong của dấu () là Parameter (thông số)
        // {} Bên trong dấu {} Các câu lệnh của hàm
        public static void  KhoiTao()
        {
            // ***** Thêm các màn hình vào danh sách màn hình:
            danhSachManHinh.Add(MaSoManHinh.PAGE_MAIN_ID, new PgMain());
            danhSachManHinh.Add(MaSoManHinh.PAGE_MENU_ID, new PgMenu());
            danhSachManHinh.Add(MaSoManHinh.PAGE_IO_ID, new PgIO());
            danhSachManHinh.Add(MaSoManHinh.PAGE_ALARM_ID, new PgAlarm());
            danhSachManHinh.Add(MaSoManHinh.PAGE_SERIAL_ID, new PgSerial());
            // Trong lớp WndMain có hàm ShowDialog();
            // Hàm này có ý nghĩa là show cái cửa sổ lên:
            // Hàm này không phải hàm tĩnh:
            // Đang đứng ở lớp UIManager.
            // B1: Gọi thể hiện của lớp WndMain bên lớp của mình UIManager:
            wndMain = new WndMain();
            // B2: Gọi hàm:
            wndMain.ShowDialog();
        }
        // Định nghĩa hàm ABC
        public void ABC()
        {

        }

        public static void ChuyenManHinh(MaSoManHinh _maSoManHinh)
        {
            // B1: Lấy ra được cái màn hình từ mã số màn hình:
            var page =(System.Windows.Controls.Page)danhSachManHinh[_maSoManHinh];
            // B2: Gắn cái màn hình mới lấy ra cho vào khung:
            wndMain.frmMain.Content = page;
        }
    }
}
