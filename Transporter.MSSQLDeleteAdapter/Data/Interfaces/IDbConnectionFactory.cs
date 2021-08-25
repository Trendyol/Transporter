using System.Data;

namespace Transporter.MSSQLDeleteAdapter.Data.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection GetConnection(string connectionString);
    }
}