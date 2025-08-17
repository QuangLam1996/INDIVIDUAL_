using PLCMonitorSystem.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PLCMonitorSystem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Khi ứng dụng được bật, thì hàm này chạy đầu tiên:
            // Gọi hàm khởi tạo của lớp UIManager ở đây:
            UIManager.KhoiTao();
        }
    }
}
