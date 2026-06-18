using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace ScheduleApp
{
    public static class Database
    {
        private static readonly string connString =
// Connection string redacted for portfolio purposes

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connString);
        }

        public static DataTable Query(string sql, params MySqlParameter[] parameters)
        {
            DataTable dt = new DataTable();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }

            return dt;
        }

        public static int Execute(string sql, params MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }
}