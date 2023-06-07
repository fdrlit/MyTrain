using System.Data.SqlClient;

namespace MyTrain
{
    public static class DatabaseHelper
    {
        public static SqlConnection GetSqlConnection()
        {
            var connectionString = "Data Source=SQL6031.site4now.net;Initial Catalog=db_a9a5c3_mytrain;User Id=db_a9a5c3_mytrain_admin;Password=admin435";
            var connection = new SqlConnection(connectionString);

            return connection;
        }
    }
}
