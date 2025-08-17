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
        public PgMain()
        {
            InitializeComponent();
            this.Loaded += PgMain_Loaded;
            this.btnGiam.Click += BtnGiam_Click;
            this.btnTang.Click += BtnTang_Click;
        }
        List<TextBox> lstTextBox = new List<TextBox>();
        List<TextBox> lstTextBox1 = new List<TextBox>();


        private void BtnTang_Click(object sender, RoutedEventArgs e)
        {
            this.stp2.Children.Clear();
            lstTextBox1.Clear();
            List<int> lstInt = new List<int>();
            List<string> lstStr = new List<string>();
            for (int i = 0; i < lstTextBox.Count; i++)
            {
                lstStr.Add(lstTextBox[i].Text);
                lstInt.Add(int.Parse(lstStr[i]));

            }
            for (int i = 0; i < lstInt.Count; i++)
            {
                for (int j = 0;j<lstInt.Count-i-1; j++)
                {
                    if (lstInt[j] > lstInt[j + 1])
                    {
                        int max = lstInt[j];
                        lstInt[j] = lstInt[j + 1];
                        lstInt[j+1] = max;
                    }
                }

            }
            for (int i = 0; i < lstInt.Count; i++)
            {
                TextBox txt2 = new TextBox();
                txt2.Text = lstInt[i].ToString();
                this.stp2.Children.Add(txt2);
                this.lstTextBox1.Add(txt2);
            }
        }

        private void BtnGiam_Click(object sender, RoutedEventArgs e)
        {
            this.stp2.Children.Clear();
            lstTextBox1.Clear();
            List<int> lstInt = new List<int>();
            List<string> lstStr = new List<string>();
            for (int i = 0; i < lstTextBox.Count; i++)
            {
                lstStr.Add(lstTextBox[i].Text);
                lstInt.Add(int.Parse(lstStr[i]));

            }
            for (int i = 0; i < lstInt.Count; i++)
            {
                for (int j = 0; j < lstInt.Count - i - 1; j++)
                {
                    if (lstInt[j] < lstInt[j + 1])
                    {
                        int max = lstInt[j];
                        lstInt[j] = lstInt[j + 1];
                        lstInt[j + 1] = max;
                    }
                }

            }
            for (int i = 0; i < lstInt.Count; i++)
            {
                TextBox txt2 = new TextBox();
                txt2.Text = lstInt[i].ToString();
                this.stp2.Children.Add(txt2);
                this.lstTextBox1.Add(txt2);
            }

        }

        private void PgMain_Loaded(object sender, RoutedEventArgs e)
        {
            // Thêm đối tượng dưới behind code mà không dùng code phía .xaml 
            //B1: Xóa hết các phần tử bên trong Stackpanel
            this.stp1.Children.Clear();
            this.lstTextBox.Clear();

            for (int i = 0; i < 10; i++)
            {
                TextBox txt1 = new TextBox();
                txt1.Text = i.ToString();
                this.stp1.Children.Add(txt1);
                this.lstTextBox.Add(txt1);
            }
            Console.WriteLine(lstTextBox.Count);

        }

    }
}
