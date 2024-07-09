using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Management.Buckets;
using Transporter.CouchbaseAdapter.Data.Interfaces;
using Transporter.CouchbaseAdapter.Utils;

namespace Transporter.CouchbaseAdapter.Data.Implementations
{
    public class BucketProvider(ICouchbaseProvider couchbaseProvider) : IBucketProvider
    {
        private readonly ICouchbaseProvider _couchbaseProvider = couchbaseProvider;
        private readonly Dictionary<string, IBucket> _buckets = new();

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

        public async Task CreateIndexAsync(ConnectionData connectionData, string bucketName, string indexName, IEnumerable<string> fields)
        {
            var cluster = await _couchbaseProvider.GetCluster(connectionData);
            await cluster.QueryIndexes.CreateIndexAsync(bucketName, indexName, fields);
        }

        public async Task CreatePrimaryIndexAsync(ConnectionData connectionData, string bucketName)
        {
            var cluster = await _couchbaseProvider.GetCluster(connectionData);
            await cluster.QueryIndexes.CreatePrimaryIndexAsync(bucketName);
            
        }
        public async Task CreateBucketAsync(ConnectionData connectionData, string bucketName)
        {
            var cluster = await _couchbaseProvider.GetCluster(connectionData);
            var bucketManager = cluster.Buckets;
            try
            {
                await bucketManager.CreateBucketAsync(new BucketSettings()
                {
                    BucketType = BucketType.Couchbase,
                    Name = bucketName,
                    RamQuotaMB = 100,
                    NumReplicas = 1
                });
            }
            catch (AggregateException ex)
            {
                foreach (var innerException in ex.InnerExceptions)
                {
                    Console.WriteLine($"Failed to create bucket '{bucketName}': {innerException.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create bucket: {ex.Message}");
            }
        }
    }
}