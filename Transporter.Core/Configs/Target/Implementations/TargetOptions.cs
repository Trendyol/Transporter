using Transporter.Core.Configs.Target.Interfaces;

namespace Transporter.Core.Configs.Target.Implementations
{
    public class TargetOptions : ITargetOptions
    {
        public string Type { get; set; }
        public string Host { get; set; }
    }
}