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

namespace PLCMonitorSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            btnMain.Click += BtnMain_Click;
            btnCong.MouseMove += BtnCong_MouseMove;
            btnCong.MouseLeave += BtnCong_MouseLeave;
            btnCong.Click += BtnCong_Click;
        }

        private void BtnCong_Click(object sender, RoutedEventArgs e)
        {
            //B1: Lấy được dữ liệu trên 2 ô textbox:
            string strO1 = txtSoThuNhat.Text;
            string strO2 = txtSoThuHai.Text;
            //B2: Chuyển kiểu dữ liệu từ chuỗi string sang số là int
            int intSoThuNhat = int.Parse(strO1);
            int intSoThuHai = int.Parse(strO2);
            // Cách thứ 2 để chuyển chuỗi sang số
            int intSo1 = Convert.ToInt32(strO1);
            int intSo2 = Convert.ToInt32(strO2);

            // B3: Thực hiện phép cộng:
            int ketQua = intSoThuNhat + intSoThuHai;

            //B4: Hiển thị kết quả lên màn hình:
            txtSoThuBa.Text = ketQua.ToString();

        }

        private void BtnCong_MouseLeave(object sender, MouseEventArgs e)
        {
            MessageBox.Show("Đã rời khỏi nút bấm Cộng");
        }

        private void BtnCong_MouseMove(object sender, MouseEventArgs e)
        {
            // Thay đổi màu nền của nút bấm cộng:
            btnCong.Background = Brushes.GreenYellow;
        }

        private void BtnMain_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Đã bấm vào nút bấm Main");
        }
    }
}
