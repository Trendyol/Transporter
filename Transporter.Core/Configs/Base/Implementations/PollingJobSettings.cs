using Transporter.Core.Configs.Base.Interfaces;
using Transporter.Core.Configs.Source.Implementations;
using Transporter.Core.Configs.Source.Interfaces;
using Transporter.Core.Configs.Target.Implementations;
using Transporter.Core.Configs.Target.Interfaces;

namespace Transporter.Core.Configs.Base.Implementations
{
    public class PollingJobSettings : IPollingJobSettings
    {
        public PollingJobSettings()
        {
            Source = new SourceOptions();
            Target = new TargetOptions();
        }

        public string Name { get; set; }
        public string Cron { get; set; }
        public ISourceOptions Source { get; set; }
        public ITargetOptions Target { get; set; }

        public override string ToString() => $"Name : {Name}\tCron : {Cron}\tSource : {Source}\tTarget : {Target}";
    }
}