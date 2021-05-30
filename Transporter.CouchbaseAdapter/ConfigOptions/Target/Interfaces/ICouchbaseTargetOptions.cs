using Transporter.CouchbaseAdapter.Utils;

namespace Transporter.CouchbaseAdapter.ConfigOptions.Target.Interfaces
{
    public interface ICouchbaseTargetOptions
    {
        public ConnectionData ConnectionData { get; set; }
        public string Bucket { get; set; }
    }
}