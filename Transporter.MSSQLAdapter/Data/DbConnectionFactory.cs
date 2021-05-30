using System;
using System.Data;
using System.Data.SqlClient;

namespace Transporter.MSSQLAdapter.Data
{ 
    public class DbConnectionFactory : IDbConnectionFactory
    {
        public IDbConnection GetConnection(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}