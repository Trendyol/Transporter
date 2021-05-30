using System.Threading.Tasks;
using Couchbase;
using Transporter.CouchbaseAdapter.Data.Interfaces;
using Transporter.CouchbaseAdapter.Utils;

namespace Transporter.CouchbaseAdapter.Data.Implementations
{
    public class BucketProvider : IBucketProvider
    {
        private readonly ICouchbaseProvider _couchbaseProvider;
        private IBucket _bucket;

        public BucketProvider(ICouchbaseProvider couchbaseProvider)
        {
            _couchbaseProvider = couchbaseProvider;
        }

        public async Task<IBucket> GetBucket(ConnectionData connectionData, string bucket)
        {
            if (_bucket != null) return _bucket;

            var cluster = await _couchbaseProvider.GetCluster(connectionData);
            _bucket = await cluster.BucketAsync(bucket);

            return _bucket;
        }
    }
}