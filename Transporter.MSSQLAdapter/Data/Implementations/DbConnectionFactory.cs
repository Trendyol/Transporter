using System;
using System.Data;
using System.Data.SqlClient;
using Transporter.MSSQLAdapter.Data.Interfaces;

namespace Transporter.MSSQLAdapter.Data.Implementations
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