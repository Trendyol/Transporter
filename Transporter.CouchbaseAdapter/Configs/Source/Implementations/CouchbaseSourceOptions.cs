using Transporter.CouchbaseAdapter.Configs.Source.Interfaces;
using Transporter.CouchbaseAdapter.Utils;

namespace Transporter.CouchbaseAdapter.Configs.Source.Implementations
{
    public class CouchbaseSourceOptions : ICouchbaseSourceOptions
    {
        public ConnectionData ConnectionData { get; set; }
        public string Bucket { get; set; }
        public long BatchQuantity { get; set; }
        public string Condition { get; set; }
    }
}