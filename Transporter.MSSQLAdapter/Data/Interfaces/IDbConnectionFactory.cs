using System.Data;

namespace Transporter.MSSQLAdapter.Data.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection GetConnection(string connectionString);
    }
}