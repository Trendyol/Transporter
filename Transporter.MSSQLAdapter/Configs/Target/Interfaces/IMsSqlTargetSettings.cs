using Transporter.Core.Configs.Target.Interfaces;

namespace Transporter.MSSQLAdapter.Configs.Target.Interfaces
{
    public interface IMsSqlTargetSettings : ITargetOptions
    {
        public IMsSqlTargetOptions Options { get; set; }
    }
}