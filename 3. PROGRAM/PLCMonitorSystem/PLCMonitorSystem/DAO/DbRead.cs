using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PLCMonitorSystem
{
    class DbRead
    {
        public const String QR1_STATUS_KEY = "run";
        public const String SCANNER_STATUS_KEY = "scanner";

        //private static MyLogger logger = new MyLogger("DbRead");

        public static List<AlarmInfo> GetLatestAlarms(int limit) {
            var ret = new List<AlarmInfo>();

            using (var conn = Dba.GetConnection()) {
                var sql = "SELECT * FROM alarm_log ORDER BY created_time DESC LIMIT @limit";
                using (var sqlCmd = conn.CreateCommand()) {
                    try {
                        sqlCmd.CommandText = sql;
                        sqlCmd.Parameters.AddWithValue("@limit", limit);
                        conn.Open();
                        using (var reader = sqlCmd.ExecuteReader()) {
                            while (reader.Read()) {
                                var x = new AlarmInfo();
                                Object obj;

                                if ((obj = reader["id"]) != DBNull.Value) {
                                    x.id = int.Parse(obj.ToString());
                                }
                                if ((obj = reader["created_time"]) != DBNull.Value) {
                                    x.createdTime = DateTime.Parse(obj.ToString());
                                }
                                if ((obj = reader["alarm_code"]) != DBNull.Value) {
                                    x.alarmCode = int.Parse(obj.ToString());
                                }
                                if ((obj = reader["message"]) != DBNull.Value) {
                                    x.message = obj.ToString();
                                }
                                if ((obj = reader["solution"]) != DBNull.Value) {
                                    x.solution = obj.ToString();
                                }
                                if ((obj = reader["mode"]) != DBNull.Value) {
                                    x.mode = int.Parse(obj.ToString());
                                }
                                ret.Add(x);
                            }
                        }
                    } catch (Exception ex) {
                        //logger.CreateLog("GetLatestAlarms error:" + ex.Message);
                    }
                }
            }
            return ret;
        }

    }
}
