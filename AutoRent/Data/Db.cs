using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace AutoRent.Data
{
    public static class Db
    {
        private static SqlConnection CreateOpenConnection()
        {
            var connection = new SqlConnection(DbConfig.ConnectionString);
            connection.Open();
            return connection;
        }

        private static void AddParameters(SqlCommand command, IDictionary<string, object> parameters)
        {
            if (parameters == null) return;
            foreach (var kvp in parameters)
            {
                command.Parameters.AddWithValue(kvp.Key, kvp.Value ?? DBNull.Value);
            }
        }

        public static DataTable ExecuteQuery(string sql, IDictionary<string, object> parameters = null)
        {
            using (var connection = CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                AddParameters(command, parameters);
                var table = new DataTable();
                using (var adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }
                return table;
            }
        }

        public static int ExecuteNonQuery(string sql, IDictionary<string, object> parameters = null)
        {
            using (var connection = CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                AddParameters(command, parameters);
                return command.ExecuteNonQuery();
            }
        }

        public static object ExecuteScalar(string sql, IDictionary<string, object> parameters = null)
        {
            using (var connection = CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                AddParameters(command, parameters);
                return command.ExecuteScalar();
            }
        }

        public static int ExecuteInsertReturnId(string sql, IDictionary<string, object> parameters = null)
        {
            var result = ExecuteScalar(sql + "; SELECT CAST(SCOPE_IDENTITY() AS INT);", parameters);
            return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
        }
    }
}
