using Transporter.PostgreSQLAdapter.Configs.Target.Interfaces;

namespace Transporter.PostgreSQLAdapter.Configs.Target.Implementations
{
    public class PostgreSqlTargetSettings : IPostgreSqlTargetSettings
    {
        public PostgreSqlTargetSettings()
        {
            Options = new PostgreSqlTargetOptions();
        }

        public string Type { get; set; }
        public IPostgreSqlTargetOptions Options { get; set; }

        public override string ToString()
        {
            return
                $"Type : {Type} Options : {Options}";
        }

        public string Host { get; set; }
    }
}