using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                if (String.IsNullOrEmpty(lstTextBox[i].Text) == false)
                {
                    lstStr.Add(lstTextBox[i].Text);
                }

            }
            for (int i = 0; i < lstStr.Count; i++)
            {
                lstInt.Add(int.Parse(lstStr[i]));
            }

            // 4321756

            // 3421756
            // 2431
            // 1432

            // 1342
            // 1243
            // 1234 756

            // 1234 576
            // 1234 567
            for (int i = 0; i < lstInt.Count - 1; i++)
            {
                for (int j = i + 1; j < lstInt.Count; j++)
                {
                    if (lstInt[i] > lstInt[j])
                    {
                        int max = lstInt[i];
                        lstInt[i] = lstInt[j];
                        lstInt[j] = max;
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
                if (String.IsNullOrEmpty(lstTextBox[i].Text) == false)
                {
                    lstStr.Add(lstTextBox[i].Text);
                }
            }
            for (int i = 0; i < lstStr.Count; i++)
            {
                lstInt.Add(int.Parse(lstStr[i]));
            }

            for (int i = 0; i < lstInt.Count-1; i++)
            {
                for (int j = i + 1; j < lstInt.Count; j++)
                {
                    if (lstInt[i] < lstInt[j])
                    {
                        int max = lstInt[i];
                        lstInt[i] = lstInt[j];
                        lstInt[j] = max;
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
                txt1.PreviewTextInput += Txt1_PreviewTextInput;
                txt1.Text = i.ToString();
                this.stp1.Children.Add(txt1);
                this.lstTextBox.Add(txt1);
            }
            Console.WriteLine(lstTextBox.Count);

        }

        private void Txt1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex rgx = new Regex("[^0-9]+");
            e.Handled = rgx.IsMatch(e.Text);
        }
    }
}
