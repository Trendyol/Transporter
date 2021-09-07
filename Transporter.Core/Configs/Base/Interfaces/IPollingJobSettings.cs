using Transporter.Core.Configs.Source.Interfaces;
using Transporter.Core.Configs.Target.Interfaces;

namespace Transporter.Core.Configs.Base.Interfaces
{
    public interface IPollingJobSettings
    {
        public string Name { get; set; }
        public string Cron { get; set; }
        public ISourceOptions Source { get; set; }
        public ITargetOptions Target { get; set; }
    }
}