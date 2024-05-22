using Transporter.PostgreSQLAdapter.Configs.Interim.Interfaces;

namespace Transporter.PostgreSQLAdapter.Configs.Interim.Implementations
{
    public class PostgreSqlInterimOptions : IPostgreSqlInterimOptions
    {
        public string Table { get; set; }
        public string Schema { get; set; }
        public string ConnectionString { get; set; }
        public long BatchQuantity { get; set; }
        public string DataSourceName { get; set; }
    }
}