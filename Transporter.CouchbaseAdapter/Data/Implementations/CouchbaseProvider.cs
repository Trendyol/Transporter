using System.Threading.Tasks;
using Couchbase;
using Octopus.Transporter.CouchbaseAdapter.Data.Interfaces;
using Transporter.CouchbaseAdapter.Utils;

namespace Transporter.CouchbaseAdapter.Data.Implementations
{
    public class CouchbaseProvider : ICouchbaseProvider
    {
        private ICluster _cluster;

        public async Task<ICluster> GetCluster(ConnectionData connectionData)
        {
            if (_cluster != null) return _cluster;

            var clusterOptions = new ClusterOptions
            {
                UserName = connectionData.Username, Password = connectionData.Password,
                BootstrapHttpPort = connectionData.UiPort
            };

            _cluster = await Cluster.ConnectAsync($"{connectionData.Hosts}", clusterOptions);

            return _cluster;
        }
    }
}