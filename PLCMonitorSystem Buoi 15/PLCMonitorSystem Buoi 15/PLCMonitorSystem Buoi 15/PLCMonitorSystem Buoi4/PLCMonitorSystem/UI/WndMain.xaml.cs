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
    /// Interaction logic for WndMain.xaml
    /// </summary>
    public partial class WndMain : Window
    {
        public WndMain()
        {
            InitializeComponent();

            // Thử gọi hàm KhoiTao và hàm ABC xem khác nhau như thế nào:
            // Hiện tại đang ở ở class WndMain
            //************************************
            // Gọi hàm ABC : Không phải là hàm tĩnh:
            // B1: Gọi thể hiện của lớp UIManager bên lớp của mình (WndMain):
            // ui : Đối tượng
            // UIManager ui = new UIManager();
            // B2: Sau khi gọi thể hiện tên là "ui" mới có thể gọi method ABC
            //   ui.ABC();

            //*************************************
            //Gọi hàm KhoiTao thuộc lớp UIManager là hàm tĩnh:
            //UIManager.KhoiTao();
            btnMain.Click += BtnMain_Click;
            btnMenu.Click += BtnMenu_Click;
            btnIO.Click += BtnIO_Click;
            btnAlarm.Click += BtnAlarm_Click;

        }

        private void BtnAlarm_Click(object sender, RoutedEventArgs e)
        {
            UIManager.ChuyenManHinh(UIManager.MaSoManHinh.PAGE_ALARM_ID);
        }

        private void BtnIO_Click(object sender, RoutedEventArgs e)
        {
            UIManager.ChuyenManHinh(UIManager.MaSoManHinh.PAGE_IO_ID);
        }

        private void BtnMenu_Click(object sender, RoutedEventArgs e)
        {
            UIManager.ChuyenManHinh(UIManager.MaSoManHinh.PAGE_MENU_ID);
        }

        private void BtnMain_Click(object sender, RoutedEventArgs e)
        {
            // Gọi hàm chuyển màn của lớp UIManager:
            UIManager.ChuyenManHinh(UIManager.MaSoManHinh.PAGE_MAIN_ID);
        }
    }
}
