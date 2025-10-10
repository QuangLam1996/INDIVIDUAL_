using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace PLCMonitorSystem
{
    public class MyLogger
    {
        // Field & Property
        string prefix = "";

        // Method

        public string Prefix { get => prefix; set => prefix = value; }

        public MyLogger(string _prefix)
        {
            this.Prefix = _prefix;
        }

        public void CreateLog(string _content)
        {
            try
            {
                // B1: Check File Path
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }
                // B2: Name File
                //string fileName = String.Format("{0}.txt", DateTime.Now.ToString("dd_MM_yyyy"));
                string fileName_ = String.Format(DateTime.Now.ToString("dd_MM_yyyy") + ".txt");

                // B3: Create File Logg
                //string fullPath = Path.Combine(path, fileName);
                string fullPath_ = Path.Combine(path, fileName_);

                // B4: Write File
                string content = String.Format("{0} [{1}]: {2}", DateTime.Now.ToString("HH:mm:ss:fff"), this.Prefix, _content);

                using (StreamWriter strWirter = new StreamWriter(fullPath_, true))
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
