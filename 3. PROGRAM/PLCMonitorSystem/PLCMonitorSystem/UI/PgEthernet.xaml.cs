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
    /// Interaction logic for PgEthernet.xaml
    /// </summary>
    public partial class PgEthernet : Page
    {
        public PgEthernet()
        {
            InitializeComponent();
            this.Loaded += PgEthernet_Loaded;
            this.btnSaveAppSetting.Click += BtnSaveAppSetting_Click;
        }

        private void BtnSaveAppSetting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UIManager.appSetting.RunSetting.WordName = tbxWord.Text;
                UIManager.appSetting.RunSetting.BitName = tbxBit.Text;
                UIManager.appSetting.RunSetting.Cycle = Convert.ToInt16(tbxCycle.Text);
                UIManager.appSetting.TimeSetting.IndicatorTime();
                UIManager.SaveAppSetting();
            }
            catch (Exception err) {}
        }

        private void PgEthernet_Loaded(object sender, RoutedEventArgs e)
        {
            this.tbxWord.Text = UIManager.appSetting.RunSetting.WordName;
            this.tbxBit.Text = UIManager.appSetting.RunSetting.BitName;
            this.tbxCycle.Text = UIManager.appSetting.RunSetting.Cycle.ToString();
        }
    }
}
