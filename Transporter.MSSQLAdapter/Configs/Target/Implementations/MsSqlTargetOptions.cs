using Transporter.MSSQLAdapter.Configs.Target.Interfaces;

namespace Transporter.MSSQLAdapter.Configs.Target.Implementations
{
    public class MsSqlTargetOptions : IMsSqlTargetOptions
    {
        public string Table { get; set; }
        public string Schema { get; set; }
        public string ConnectionString { get; set; }
        public string ExcludedColumns { get; set; }
        public string IdColumn { get; set; }
    }
}