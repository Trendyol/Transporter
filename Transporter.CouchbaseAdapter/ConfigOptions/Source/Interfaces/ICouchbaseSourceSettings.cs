using Transporter.Core;

namespace Transporter.CouchbaseAdapter.ConfigOptions.Source.Interfaces
{
    public interface ICouchbaseSourceSettings : ISourceOptions
    {
        public ICouchbaseSourceOptions Options { get; set; }
    }
}