using Transporter.Core.Configs.Source.Interfaces;

namespace Transporter.CouchbaseAdapter.Configs.Source.Interfaces
{
    public interface ICouchbaseSourceSettings : ISourceOptions
    {
        public ICouchbaseSourceOptions Options { get; set; }
    }
}