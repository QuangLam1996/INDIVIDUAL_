using PLCMonitorSystem.LIB;
using PLCMonitorSystem.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows;

namespace PLCMonitorSystem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //NonProcedure nonProcedure = new NonProcedure();
        //Test test = new Test();
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            UIManager.Initial();
        }
    }
}


