using Transporter.MSSQLAdapter.Configs.Source.Interfaces;

namespace Transporter.MSSQLAdapter.Configs.Source.Implementations
{
    public class MsSqlSourceOptions : IMsSqlSourceOptions
    {
        public string Table { get; set; }
        public string Schema { get; set; }
        public string ConnectionString { get; set; }
        public long BatchQuantity { get; set; }
        public string IdColumn { get; set; }
        public string Condition { get; set; }

        public override string ToString() => 
            $"Schema : {Schema} Table : {Table} ConnectionString : {ConnectionString} IdColumn : {IdColumn} BatchCount : {BatchQuantity} Condition : {Condition}";
    }
}