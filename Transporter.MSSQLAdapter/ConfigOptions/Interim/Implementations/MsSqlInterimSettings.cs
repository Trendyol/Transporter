using Transporter.MSSQLAdapter.ConfigOptions.Interim.Interfaces;

namespace Transporter.MSSQLAdapter.ConfigOptions.Interim.Implementations
{
    public class MsSqlInterimSettings : IMsSqlInterimSettings
    {
        public MsSqlInterimSettings()
        {
            Options = new MsSqlInterimOptions();
        }

        public IMsSqlInterimOptions Options { get; set; }
        public string Type { get; set; }

        public override string ToString()
        {
            return
                $"Type : {Type} Options : {Options}";
        }

        public string Host { get; set; }
    }
}