using System.Configuration;

namespace AutoRent.Data
{
    public static class DbConfig
    {
        private static string _connectionString =
            ConfigurationManager.ConnectionStrings["AutoRentDb"]?.ConnectionString;

        public static string ConnectionString => _connectionString;

        public static void SetConfiguration(string connectionString)
        {
            _connectionString = connectionString;
        }
    }
}
