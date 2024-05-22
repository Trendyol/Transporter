using Transporter.PostgreSQLAdapter.Configs.Target.Interfaces;

namespace Transporter.PostgreSQLAdapter.Configs.Target.Implementations
{
    public class PostgreSqlTargetOptions : IPostgreSqlTargetOptions
    {
        public string Table { get; set; }
        public string Schema { get; set; }
        public string ConnectionString { get; set; }
        public string ExcludedColumns { get; set; }
        public string IdColumn { get; set; }
    }
}