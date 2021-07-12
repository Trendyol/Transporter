using System.Collections.Generic;
using System.Threading.Tasks;
using Couchbase;
using Transporter.CouchbaseAdapter.Data.Interfaces;
using Transporter.CouchbaseAdapter.Utils;

namespace Transporter.CouchbaseAdapter.Data.Implementations
{
    public class BucketProvider : IBucketProvider
    {
        private readonly ICouchbaseProvider _couchbaseProvider;
        private readonly Dictionary<string, IBucket> _buckets = new();

        public BucketProvider(ICouchbaseProvider couchbaseProvider)
        {
            _couchbaseProvider = couchbaseProvider;
        }

        public async Task<IBucket> GetBucket(ConnectionData connectionData, string bucketName)
        {
            if (_buckets.ContainsKey(bucketName))
            {
                return _buckets[bucketName];
            }

            var cluster = await _couchbaseProvider.GetCluster(connectionData);
            var bucket = await cluster.BucketAsync(bucketName);

            _buckets.Add(bucketName, bucket);

            return bucket;
        }
    }
}