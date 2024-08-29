using Transporter.Core.Configs.Base.Interfaces;
using Transporter.Core.Configs.Interim.Interfaces;
using Transporter.Core.Configs.Source.Interfaces;
using Transporter.Core.Configs.Target.Interfaces;
using Transporter.MSSQLAdapter.Configs.Interim.Implementations;
using Transporter.MSSQLAdapter.Configs.Source.Implementations;
using Transporter.MSSQLAdapter.Configs.Target.Implementations;

namespace Transporter.MSSQLAdapter
{
    public class MsSqlTransferJobSettings : ITransferJobSettings
    {
        public MsSqlTransferJobSettings()
        {
            Source = new MsSqlSourceSettings();
            Target = new MsSqlTargetSettings();
            Interim = new MsSqlInterimSettings();
        }

        public string Name { get; set; }
        public string Cron { get; set; }
        public ISourceOptions Source { get; set; }
        public ITargetOptions Target { get; set; }
        public IInterimOptions Interim { get; set; }

        public override string ToString() => $"Name : {Name}\tCron : {Cron}\tSource : {Source}\tTarget : {Target}";
    }
}