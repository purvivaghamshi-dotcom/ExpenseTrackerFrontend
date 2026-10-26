
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace ExpenseTrackerFrontend.Helpers
{
    public static class DbHelper
    {
        private static readonly string connectionString =
            ConfigurationManager
                .ConnectionStrings["ExpenseTrackerDB"]
                .ConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        public static bool TestConnection()
        {
            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
