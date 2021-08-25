using Transporter.Core.Configs.Interim.Implementations;
using Transporter.Core.Configs.Interim.Interfaces;
using Transporter.Core.Configs.Source.Implementations;
using Transporter.Core.Configs.Source.Interfaces;
using Transporter.Core.Configs.Target.Implementations;
using Transporter.Core.Configs.Target.Interfaces;

namespace Transporter.Core
{
    public interface IJobSettings
    {
        public string Name { get; set; }
        public string Cron { get; set; }
        public ISourceOptions Source { get; set; }
        public ITargetOptions Target { get; set; }
    }

    public class JobSettings : IJobSettings
    {
        public JobSettings()
        {
            Source = new SourceOptions();
            Target = new TargetOptions();
            Interim = new InterimOptions();
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