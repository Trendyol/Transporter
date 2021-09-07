namespace Transporter.MSSQLAdapter.Configs.Interim.Interfaces
{
    public interface IMsSqlInterimOptions
    {
        public string Table { get; set; }
        public string Schema { get; set; }
        public string ConnectionString { get; set; }
        public long BatchQuantity { get; set; }
        public string DataSourceName { get; set; }
    }
}