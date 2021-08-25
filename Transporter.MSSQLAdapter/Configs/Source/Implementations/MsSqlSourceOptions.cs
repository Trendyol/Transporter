using Transporter.MSSQLAdapter.Configs.Source.Interfaces;

namespace Transporter.MSSQLAdapter.Configs.Source.Implementations
{
    public class MsSqlSourceOptions : IMsSqlSourceOptions
    {
        private long _batchQuantity;
        public string Table { get; set; }
        public string Schema { get; set; }
        public string ConnectionString { get; set; }

        public long BatchQuantity
        {
            get => _batchQuantity;
            set => _batchQuantity = value > 1000 ? 1000 : value;
        }

        public string StatusIdColumnName { get; set; }
        public string IdColumn { get; set; }
        public bool IsIdAutoIncrementOn { get; set; }
        public string Condition { get; set; }
        public override string ToString()
        {
            return
                $"Schema : {Schema} Table : {Table} ConnectionString : {ConnectionString} IdColumn : {IdColumn} BatchCount : {BatchQuantity} StatusIdColumnName : {StatusIdColumnName} Condition : {Condition}";
        }
    }
}