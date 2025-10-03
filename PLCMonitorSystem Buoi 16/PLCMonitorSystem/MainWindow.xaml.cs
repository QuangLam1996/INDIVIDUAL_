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
            btnMenu.MouseLeave += BtnMenu_MouseLeave;
            btnMenu.PreviewMouseMove += BtnMenu_PreviewMouseMove;
        }

        private void BtnMenu_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            //btnMenu.Background = Brushes.Green;
        }

        private void BtnMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            //btnMenu.Background = Brushes.Blue;
        }

        private void BtnMain_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
