using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.IO;
using Newtonsoft.Json;

namespace PLCMonitorSystem.UI
{
    public static class UIManager
    {
        private static Hashtable danhSachManHinh = new Hashtable();
        private static WndMain wndMain = new WndMain();
        public static AppSetting appSetting = new AppSetting();

        public static MyLogger theLog = new MyLogger();
        public static Define theDefine = new Define();
        public static DefineAlarm theAlarm = new DefineAlarm();
        public static DefineIO theIO = new DefineIO();
        public static DefineServo theServo = new DefineServo(); 
        public static Function theFunction = new Function();
        public static Global theGlobal = new Global();

        public static SeqThread threadSequence = new SeqThread();
        public static SafetyThread threadSafety = new SafetyThread();

        public enum MaSoManHinh
        {
            PAGE_MAIN_ID = 1,

            PAGE_MENU_ID = 10,
            PAGE_SERIAL_ID = 11,
            PAGE_ETHERNET_ID = 12,
            PAGE_TEACHING_ID = 13,
            PAGE_MANUAL_ID = 14,
            PAGE_STATUS_ID = 15,
            PAGE_MODEL_ID = 16,
            PAGE_USER_ID = 17,

            PAGE_IO_ID = 20,

            PAGE_ALARM_ID = 30,
        }
        public static void SwitchPage()
        {
            danhSachManHinh.Add(MaSoManHinh.PAGE_MAIN_ID, new PgMain());
            danhSachManHinh.Add(MaSoManHinh.PAGE_MENU_ID, new PgMenu());
            danhSachManHinh.Add(MaSoManHinh.PAGE_IO_ID, new PgIO());
            danhSachManHinh.Add(MaSoManHinh.PAGE_ALARM_ID, new PgAlarm());
            danhSachManHinh.Add(MaSoManHinh.PAGE_SERIAL_ID, new PgSerial());
            danhSachManHinh.Add(MaSoManHinh.PAGE_ETHERNET_ID, new PgEthernet());
        }
        public static void Switch_Pg(MaSoManHinh pgId)
        {
            if (danhSachManHinh.ContainsKey(pgId))
            {
                var page = (Page)danhSachManHinh[pgId];
                wndMain.frmMain.Content = page;
            }

            if (pgId == MaSoManHinh.PAGE_MAIN_ID)
            {
                wndMain.btnMain.Background = Brushes.Orange;
            }
            else
            {
                wndMain.btnMain.ClearValue(Button.BackgroundProperty);
            }

            if (pgId >= MaSoManHinh.PAGE_MENU_ID && pgId <= MaSoManHinh.PAGE_USER_ID)
            {
                wndMain.btnMenu.Background = Brushes.Orange;
            }
            else
            {
                wndMain.btnMenu.ClearValue(Button.BackgroundProperty);
            }

            if (pgId == MaSoManHinh.PAGE_IO_ID)
            {
                wndMain.btnIO.Background = Brushes.Orange;
            }
            else
            {
                wndMain.btnIO.ClearValue(Button.BackgroundProperty);
            }

            if (pgId == MaSoManHinh.PAGE_ALARM_ID)
            {
                wndMain.btnAlarm.Background = Brushes.Orange;
            }
            else
            {
                wndMain.btnAlarm.ClearValue(Button.BackgroundProperty);
            }
        }
        public static void Initial()
        {
            SwitchPage();
            //timeSetting.IndicatorTime();
            appSetting = LoadAppSetting();

            Switch_Pg(MaSoManHinh.PAGE_MAIN_ID);
            wndMain.ShowDialog();
        }
        public static void ChangeScreen(MaSoManHinh _maSoManHinh)
        {
            //B1: Lấy màn hình từ mã số màn hình
            var page = (Page)danhSachManHinh[_maSoManHinh];
            //B2 Gán màn hình đã lấy gán vào khung
            //wndMain.frmMain.Content = page;
        }

        public static AppSetting LoadAppSetting()
        {
            AppSetting _appSetting = new AppSetting();
            // B1: Tạo đường dẫn đến File
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings");

            // B2: Kiểm tra xem đã có đường dẫn
            if (Directory.Exists(path) == false)
            {
                return _appSetting;
            }

            // B3: Tạo đường dẫn đầy đủ
            string fullPath = Path.Combine(path, "setting.json");

            // B4:  Kiểm tra có tồn tại file cài đặt
            if (File.Exists(fullPath) == false)
            {
                return _appSetting;
            }

            // B5: Đọc toàn bộ nội dung
            string strContent = "";
            using (StreamReader strReader = new StreamReader(fullPath))
            {
                strContent = strReader.ReadToEnd();
            }

            // B6: Kiểm tra nội dung có null
            if (String.IsNullOrEmpty(strContent) == true)
            {
                return _appSetting;
            }

            // B7: Chuyển data.txt thành các cài đặt
            try
            {
                _appSetting = JsonConvert.DeserializeObject<AppSetting>(strContent);
            }
            catch (Exception err) { }

            return _appSetting;
        }

        public static void SaveAppSetting()
        {
            // B1: Tạo đường dẫn đến File
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings");

            // B2: Kiểm tra xem đã có đường dẫn
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            // B3: Tạo đường dẫn đầy đủ
            string fullPath = Path.Combine(path, "setting.json");

            // B4:  Chuyển App Setting thành [string] để ghi vào [.json]
            string strContent = "";
            strContent = JsonConvert.SerializeObject(appSetting);

            // B5: Ghi nội dung vào [.json]
            using (StreamWriter strWriter = new StreamWriter(fullPath,false))
            {
                strWriter.Write(strContent);
                strWriter.Flush();
                strWriter.Close();
            }
        }
    }
}
