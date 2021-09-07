using Transporter.Core.Configs.Base.Interfaces;

namespace Transporter.Core.Configs.Target.Interfaces
{
    public interface ITargetOptions : IOptions
    {
        public string Host { get; set; }
    }
}