using Transporter.Core.Configs.Target.Interfaces;

namespace Transporter.PostgreSQLAdapter.Configs.Target.Interfaces
{
    public interface IPostgreSqlTargetSettings : ITargetOptions
    {
        public IPostgreSqlTargetOptions Options { get; set; }
    }
}