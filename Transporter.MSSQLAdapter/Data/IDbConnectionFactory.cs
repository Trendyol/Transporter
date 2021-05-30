using System.Data;

namespace Transporter.MSSQLAdapter.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection GetConnection(string connectionString);
    }
}