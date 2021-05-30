using System.Data;

namespace Transporter.MSSQLDeleteAdapter.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection GetConnection(string connectionString);
    }
}