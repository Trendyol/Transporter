using Transporter.MSSQLDeleteAdapter.Configs.Source.Interfaces;

namespace Transporter.MSSQLDeleteAdapter.Configs.Source.Implementations
{
    public class MsSqlSourceOptions : IMsSqlSourceOptions
    {
        public string Table { get; set; }
        public string Schema { get; set; }
        public string ConnectionString { get; set; }
        public long BatchQuantity { get; set; }
        public string StatusIdColumnName { get; set; }
        public string IdColumn { get; set; }
        public string Condition { get; set; }

        public override string ToString()
        {
            return
                $"Schema : {Schema} Table : {Table} ConnectionString : {ConnectionString} IdColumn : {IdColumn} BatchCount : {BatchQuantity} StatusIdColumnName : {StatusIdColumnName} Condition : {Condition}";
        }
    }
}