using Transporter.MSSQLAdapter.ConfigOptions.Interim.Interfaces;

namespace Transporter.MSSQLAdapter.ConfigOptions.Interim.Implementations
{
    public class MsSqlInterimOptions : IMsSqlInterimOptions
    {
        public string Table { get; set; }
        public string Schema { get; set; }
        public string ConnectionString { get; set; }
        public long BatchQuantity { get; set; }
        public string DataSourceName { get; set; }
    }
}