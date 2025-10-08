using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCMonitorSystem.LIB
{
    public class MyCsv
    {
        string prefix = "";
        public string Prefix { get => prefix; set => prefix = value; }

        public MyCsv(string _prefix) { this.Prefix = _prefix; }
        public void Create(int _value, string _position)
        {
            try
            {
                // B1: Check File Path
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Lot.No");
                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }
                // B2: Name File
                string fileName = String.Format("{0}.csv", DateTime.Now.ToString("dd_MM_yyyy"));

                // B3: Create File Logg
                string fullPath = Path.Combine(path, fileName);

                // B4: Write File
                string content = String.Format("{0},{1},{2}", DateTime.Now.ToString("HH:mm:ss"), _value, _position);
                string header = String.Format("{0},{1},{2}", "TIME", "VALUE", "POSITION");
                if (File.Exists(fullPath) == false)
                {
                    using (StreamWriter strWirter = new StreamWriter(fullPath, true))
                    {
                        strWirter.WriteLine(header);
                        strWirter.Flush();
                        strWirter.Close();
                    }
                }
                using (StreamWriter strWirter = new StreamWriter(fullPath, true))
                {
                    strWirter.WriteLine(content);
                    strWirter.Flush();
                    strWirter.Close();
                }

            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine("Create Error get error: " + err.Message);

            }

        }

    }
}
