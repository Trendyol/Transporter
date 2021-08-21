using Transporter.Core;

namespace Transporter.CouchbaseAdapter.ConfigOptions.Interim.Interfaces
{
    public interface ICouchbaseInterimSettings : IInterimOptions
    {
        public ICouchbaseInterimOptions Options { get; set; }
    }
}