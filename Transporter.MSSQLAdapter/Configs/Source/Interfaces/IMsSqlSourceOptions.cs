namespace Transporter.MSSQLAdapter.Configs.Source.Interfaces
{
    public interface IMsSqlSourceOptions
    {
        public string Table { get; set; }
        public string Schema { get; set; }
        public string ConnectionString { get; set; }
        public long BatchQuantity { get; set; }
        public string IdColumn { get; set; }
        public bool IsIdAutoIncrementOn { get; set; }
        public string Condition { get; set; }
    }
}