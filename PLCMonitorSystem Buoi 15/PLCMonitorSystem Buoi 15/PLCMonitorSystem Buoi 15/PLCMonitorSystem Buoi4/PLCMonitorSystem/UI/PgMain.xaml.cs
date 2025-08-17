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

namespace PLCMonitorSystem.UI
{
    /// <summary>
    /// Interaction logic for PgMain.xaml
    /// </summary>
    public partial class PgMain : Page
    {
        List<TextBox> lstTextBox = new List<TextBox>();
        public PgMain()
        {
            InitializeComponent();
            this.Loaded += PgMain_Loaded;
            this.btnTangDan.Click += BtnTangDan_Click;
        }

        private void BtnTangDan_Click(object sender, RoutedEventArgs e)
        {
            //B1: Lấy dữ liệu của 10 ô textbox bên tay trái:
            List<string> lstTxt = new List<string>();
            for (int i = 0; i < lstTextBox.Count; i++)
            {
                lstTxt.Add(lstTextBox[i].Text);
            }

            // B2: Chuyển cái dữ liệu từ kiểu string sang kiểu int để sắp xếp:
            List<int> lstInt = new List<int>();
            for (int i = 0; i < lstTxt.Count; i++)
            {
                lstInt.Add(int.Parse(lstTxt[i]));
            }
            // B3: Tiến hành sắp xếp: BÀI TẬP VỀ NHÀ:

            List<int> lstKetQua = new List<int>();
            lstKetQua = lstInt;

            //B4: Hiển thị kết quả sắp xếp lên stackpanel bên phải:
            this.stp2.Children.Clear();
            for (int i = 0; i < lstKetQua.Count; i++)
            {
                Label lbl = new Label();
                lbl.Content = lstKetQua[i].ToString();
                this.stp2.Children.Add(lbl);
            }

        }

        private void PgMain_Loaded(object sender, RoutedEventArgs e)
        {
            // B1: Xóa hết các phần tử bên trong stackpanel:
            this.stp1.Children.Clear();
            lstTextBox.Clear();
            // B2: Thêm các phần tử vào:
            for (int i = 0; i < 10; i++)
            {
                TextBox txt = new TextBox();
                txt.Text = i.ToString();
                this.stp1.Children.Add(txt);
                lstTextBox.Add(txt);
            }

        }
    }
}
