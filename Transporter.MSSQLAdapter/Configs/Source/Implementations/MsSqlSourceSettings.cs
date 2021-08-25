using Transporter.MSSQLAdapter.Configs.Source.Interfaces;

namespace Transporter.MSSQLAdapter.Configs.Source.Implementations
{
    public class MsSqlSourceSettings : IMsSqlSourceSettings
    {
        public MsSqlSourceSettings()
        {
            Options = new MsSqlSourceOptions();
        }

        public string Type { get; set; }
        public IMsSqlSourceOptions Options { get; set; }

        public override string ToString()
        {
            return
                $"Type : {Type} Options : {Options}";
        }

        public string Host { get; set; }
        public bool IsInsertableOnFailure { get; set; }   
    }
}