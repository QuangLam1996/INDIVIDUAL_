using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PLCMonitorSystem.LIB;
using PLCMonitorSystem.DATA;
namespace PLCMonitorSystem.DAO
{
    class DbWrite
    {
        private static MyLogger logger = new MyLogger("DbWrite");

        public static bool createAlarm(AlarmInfo alarm) {
            var ret = false;
            using (var conn = Dba.GetConnection()) {
                var sql = "INSERT INTO alarm_log (created_time, alarm_code, message, solution, mode) VALUES (@time, @code, @message, @solution, @mode)";
                using (var sqlCmd = conn.CreateCommand()) {                    
                    try { 
                        sqlCmd.CommandText = sql;
                        sqlCmd.Parameters.AddWithValue("@time", alarm.createdTime.ToString("yyyy-MM-dd HH:mm:ss.ff"));
                        sqlCmd.Parameters.AddWithValue("@code", alarm.alarmCode);
                        sqlCmd.Parameters.AddWithValue("@message", alarm.message);
                        sqlCmd.Parameters.AddWithValue("@solution", alarm.solution);
                        sqlCmd.Parameters.AddWithValue("@mode", alarm.mode); 
                        
                        conn.Open();
                        ret = sqlCmd.ExecuteNonQuery() > 0;
                    } catch (Exception ex) {
                        logger.CreateLog("createAlarm error:" + ex.Message);
                    }
                }
            }
            return ret;
        }
    }
}
