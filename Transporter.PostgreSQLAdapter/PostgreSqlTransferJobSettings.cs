using Transporter.Core.Configs.Base.Interfaces;
using Transporter.Core.Configs.Interim.Interfaces;
using Transporter.Core.Configs.Source.Interfaces;
using Transporter.Core.Configs.Target.Interfaces;
using Transporter.PostgreSQLAdapter.Configs.Interim.Implementations;
using Transporter.PostgreSQLAdapter.Configs.Source.Implementations;
using Transporter.PostgreSQLAdapter.Configs.Target.Implementations;

namespace Transporter.PostgreSQLAdapter
{
    public class PostgreSqlTransferJobSettings : ITransferJobSettings
    {
        public PostgreSqlTransferJobSettings()
        {
            Source = new PostgreSqlSourceSettings();
            Target = new PostgreSqlTargetSettings();
            Interim = new PostgreSqlInterimSettings();
        }

        public string Name { get; set; }
        public string Cron { get; set; }
        public ISourceOptions Source { get; set; }
        public ITargetOptions Target { get; set; }
        public IInterimOptions Interim { get; set; }

        public override string ToString()
        {
            return
                $"Name : {Name}\tCron : {Cron}\tSource : {Source}\tTarget : {Target}";
        }
    }
}