using Transporter.CouchbaseAdapter.Configs.Interim.Interfaces;
using Transporter.CouchbaseAdapter.Utils;

namespace Transporter.CouchbaseAdapter.Configs.Interim.Implementations
{
    public class CouchbaseInterimOptions : ICouchbaseInterimOptions
    {
        public ConnectionData ConnectionData { get; set; }
        public string Bucket { get; set; }
        public long BatchQuantity { get; set; }
        public string DataSourceName { get; set; }
    }
}