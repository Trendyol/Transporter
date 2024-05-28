using Transporter.PostgreSQLAdapter.Configs.Source.Interfaces;

namespace Transporter.PostgreSQLAdapter.Configs.Source.Implementations
{
    public class PostgreSqlSourceSettings : IPostgreSqlSourceSettings
    {
        public PostgreSqlSourceSettings()
        {
            Options = new PostgreSqlSourceOptions();
        }

        public string Type { get; set; }
        public IPostgreSqlSourceOptions Options { get; set; }

        public override string ToString()
        {
            return $"Type : {Type} Options : {Options}";
        }

        public string Host { get; set; }
    }
}