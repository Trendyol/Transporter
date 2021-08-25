using Transporter.Core.Configs.Target.Interfaces;

namespace Transporter.CouchbaseAdapter.Configs.Target.Interfaces
{
    public interface ICouchbaseTargetSettings : ITargetOptions
    {
        public ICouchbaseTargetOptions Options { get; set; }
    }
}