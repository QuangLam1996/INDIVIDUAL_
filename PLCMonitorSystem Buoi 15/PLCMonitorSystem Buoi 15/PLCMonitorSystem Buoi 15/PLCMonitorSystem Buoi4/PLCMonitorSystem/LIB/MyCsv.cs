using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCMonitorSystem.LIB
{
    public class MyCsv
    {
        // Field Property:
        string prefix = "";

        // Method:
        public MyCsv(string _prefix)
        {
            this.prefix = _prefix;
        }

        public void Create(int _value, string _position)
        {
            try
            {
                //B1: Kiểm tra đã có đường dẫn nơi lưu file log chưa:
                string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log","Nhiet_Do");
                if (System.IO.Directory.Exists(path) == false)
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                //B2: Tạo tên file:
                string fileName = String.Format("{0}.csv", DateTime.Now.ToString("dd_MM_yyyy"));
                // fileName = "02_03_2024.log"
                //B3: Tạo đường dẫn full:
                string fullPath = System.IO.Path.Combine(path, fileName);

                // B4: Viết xuống file:
                string content = String.Format("{0},{1},{2}",DateTime.Now.ToString("HH:mm:ss:fff"),
                    _value,_position
                    );
                string header = String.Format("TIME,VALUE,POSITION");
                if (System.IO.File.Exists(fullPath)==false)
                {
                    using (System.IO.StreamWriter strWriter = new System.IO.StreamWriter(fullPath, true))
                    {
                        strWriter.WriteLine(header);
                        strWriter.Flush();
                        strWriter.Close();
                    }
                }
                using (System.IO.StreamWriter strWriter = new System.IO.StreamWriter(fullPath, true))
                {
                    strWriter.WriteLine(content);
                    strWriter.Flush();
                    strWriter.Close();
                }
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine("Create Error get error: " + err.Message);
            }
        }
    }
}
