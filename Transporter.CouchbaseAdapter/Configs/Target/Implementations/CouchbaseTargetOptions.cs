using Transporter.CouchbaseAdapter.Configs.Target.Interfaces;
using Transporter.CouchbaseAdapter.Utils;

namespace Transporter.CouchbaseAdapter.Configs.Target.Implementations
{
    public class CouchbaseTargetOptions : ICouchbaseTargetOptions
    {
        public ConnectionData ConnectionData { get; set; }
        public string Bucket { get; set; }
        public string KeyProperty { get; set; }
        public string ExcludedProperties { get; set; }
    }
}