using Transporter.CouchbaseAdapter.Utils;

namespace Transporter.CouchbaseAdapter.Configs.Target.Interfaces
{
    public interface ICouchbaseTargetOptions
    {
        public ConnectionData ConnectionData { get; set; }
        public string Bucket { get; set; }
        public string KeyProperty { get; set; }
    }
}