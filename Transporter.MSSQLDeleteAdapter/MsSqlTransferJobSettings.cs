using Transporter.Core.Configs.Base.Interfaces;
using Transporter.Core.Configs.Source.Interfaces;
using Transporter.Core.Configs.Target.Interfaces;
using Transporter.MSSQLDeleteAdapter.Configs.Source.Implementations;

namespace Transporter.MSSQLDeleteAdapter
{
    public class MsSqlTransferJobSettings : ITransferJobSettings
    {
        public MsSqlTransferJobSettings()
        {
            Source = new MsSqlSourceSettings();
        }

        public string Name { get; set; }
        public string Cron { get; set; }
        public ISourceOptions Source { get; set; }
        public ITargetOptions Target { get; set; }

        public override string ToString()
        {
            return
                $"Name : {Name}\tCron : {Cron}\tSource : {Source}";
        }
    }
}