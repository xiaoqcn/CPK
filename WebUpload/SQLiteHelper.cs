using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SQLite;

namespace WebUpload
{
    public class SQLiteHelper
    {
        public static SQLiteConnectionStringBuilder GetConnection(string database)
        {
            SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
            connstr.DataSource = database;
            return connstr;
        }
        public static int ExecuteNonQuery(string sql,SQLiteConnectionStringBuilder connStrBuilder)
        {
            int result = -1;
            using (System.Data.SQLite.SQLiteConnection conn = new System.Data.SQLite.SQLiteConnection())
            using (System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand())
            {
                conn.ConnectionString = connStrBuilder.ToString();
                conn.Open();

                cmd.Connection = conn;

                //string sql = "INSERT INTO people(pid,name,nickname) VALUES(null,'{0}','{1}')";
                //sql = string.Format(sql, name, nickname);

                cmd.CommandText = sql;
                result = cmd.ExecuteNonQuery();

            }
            return result;
        }
    }
}