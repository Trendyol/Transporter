using System;
using System.Data;
using Npgsql;
using Transporter.PostgreSQLAdapter.Data.Interfaces;

namespace Transporter.PostgreSqlAdapter.Data.Implementations
{ 
    public class DbConnectionFactory : IDbConnectionFactory
    {
        public IDbConnection GetConnection(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}