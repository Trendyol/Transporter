using Transporter.Core;

namespace Transporter.CouchbaseAdapter.ConfigOptions.Target.Interfaces
{
    public interface ICouchbaseTargetSettings : ITargetOptions
    {
        public ICouchbaseTargetOptions Options { get; set; }
    }
}