using System.Data;

namespace Transporter.PostgreSQLAdapter.Data.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection GetConnection(string connectionString);
    }
}