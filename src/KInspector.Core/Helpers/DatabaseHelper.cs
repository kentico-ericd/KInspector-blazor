using System.Data;
using System.Data.SqlClient;

using KInspector.Core.Models;

namespace KInspector.Core.Helpers
{
    public static class DatabaseHelper
    {
        public static IDbConnection GetSqlConnection(DatabaseSettings? databaseSettings, string? connectionString)
        {
            if (connectionString is not null)
            {
                return GetSqlConnection(connectionString);
            }
            else
            {
                var dbSettingsString = GetConnectionString(databaseSettings);

                return GetSqlConnection(dbSettingsString);
            }
        }

        private static string GetConnectionString(DatabaseSettings databaseSettings)
        {
            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder();

            if (databaseSettings.IntegratedSecurity)
            {
                sb.IntegratedSecurity = true;
            }
            else
            {
                sb.UserID = databaseSettings.User;
                sb.Password = databaseSettings.Password;
            }

            sb["Server"] = databaseSettings.Server;
            sb["Database"] = databaseSettings.Database;

            return sb.ConnectionString;
        }

        private static IDbConnection GetSqlConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}