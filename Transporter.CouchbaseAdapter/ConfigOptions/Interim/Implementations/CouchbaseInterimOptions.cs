using Transporter.CouchbaseAdapter.ConfigOptions.Interim.Interfaces;
using Transporter.CouchbaseAdapter.Utils;

namespace Transporter.CouchbaseAdapter.ConfigOptions.Interim.Implementations
{
    public class CouchbaseInterimOptions : ICouchbaseInterimOptions
    {
        public ConnectionData ConnectionData { get; set; }
        public string Bucket { get; set; }
        public long BatchQuantity { get; set; }
        public string DataSourceName { get; set; }
    }
}