using Transporter.PostgreSQLAdapter.Configs.Source.Interfaces;

namespace Transporter.PostgreSQLAdapter.Configs.Source.Implementations
{
    public class PostgreSqlSourceOptions : IPostgreSqlSourceOptions
    {
        public string Table { get; set; }
        public string Schema { get; set; }
        public string ConnectionString { get; set; }
        public long BatchQuantity { get; set; }
        public string IdColumn { get; set; }
        public string Condition { get; set; }

        public override string ToString()
        {
            return
                $"Schema : {Schema} Table : {Table} ConnectionString : {ConnectionString} IdColumn : {IdColumn} BatchCount : {BatchQuantity} Condition : {Condition}";
        }
    }
}