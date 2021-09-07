using Transporter.Core.Configs.Interim.Interfaces;

namespace Transporter.CouchbaseAdapter.Configs.Interim.Interfaces
{
    public interface ICouchbaseInterimSettings : IInterimOptions
    {
        public ICouchbaseInterimOptions Options { get; set; }
    }
}