using System;
using System.Data.SQLite;
using System.IO;

namespace PLCMonitorSystem.DAO
{
    class Dba
    {
        public static SQLiteConnection GetConnection() {
            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "qr1.db");
            var dbConnectionString = String.Format("Data Source={0};Mode=ReadWrite;", dbPath);
            var conn = new SQLiteConnection(dbConnectionString);
            conn.DefaultTimeout = 10;
            return conn;
        }
    }
}
