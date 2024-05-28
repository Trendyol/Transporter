using Transporter.PostgreSQLAdapter.Configs.Interim.Interfaces;

namespace Transporter.PostgreSQLAdapter.Configs.Interim.Implementations
{
    public class PostgreSqlInterimSettings : IPostgreSqlInterimSettings
    {
        public PostgreSqlInterimSettings()
        {
            Options = new PostgreSqlInterimOptions();
        }

        public IPostgreSqlInterimOptions Options { get; set; }
        public string Type { get; set; }

        public override string ToString()
        {
            return $"Type : {Type} Options : {Options}";
        }

        public string Host { get; set; }
    }
}