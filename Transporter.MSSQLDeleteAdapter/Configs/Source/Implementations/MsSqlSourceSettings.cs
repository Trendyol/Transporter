using Transporter.MSSQLDeleteAdapter.Configs.Source.Interfaces;

namespace Transporter.MSSQLDeleteAdapter.Configs.Source.Implementations
{
    public class MsSqlSourceSettings : IMsSqlSourceSettings
    {
        public MsSqlSourceSettings()
        {
            Options = new MsSqlSourceOptions();
        }

        public string Type { get; set; }
        public IMsSqlSourceOptions Options { get; set; }
        public string Host { get; set; }

        public override string ToString()
        {
            return
                $"Type : {Type} Options : {Options}";
        }
    }
}