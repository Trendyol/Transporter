using Transporter.MSSQLAdapter.Configs.Target.Interfaces;

namespace Transporter.MSSQLAdapter.Configs.Target.Implementations
{
    public class MsSqlTargetSettings : IMsSqlTargetSettings
    {
        public MsSqlTargetSettings() => Options = new MsSqlTargetOptions();

        public string Type { get; set; }
        public IMsSqlTargetOptions Options { get; set; }

        public override string ToString() => $"Type : {Type} Options : {Options}";

        public string Host { get; set; }
    }
}