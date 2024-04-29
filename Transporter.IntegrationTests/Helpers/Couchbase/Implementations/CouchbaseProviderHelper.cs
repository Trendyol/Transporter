using System.Collections.Generic;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Core.Exceptions.KeyValue;
using Couchbase.KeyValue;
using Transporter.CouchbaseAdapter.Data.Interfaces;
using Transporter.CouchbaseAdapter.Utils;
using Transporter.IntegrationTests.Helpers.Couchbase.Interfaces;

namespace Transporter.IntegrationTests.Helpers.Couchbase.Implementations
{
    public class CouchbaseProviderHelper : ICouchbaseProviderHelper
    {
        private readonly IBucketProvider _bucketProvider;

        public CouchbaseProviderHelper(IBucketProvider bucketProvider)
        {
            _bucketProvider = bucketProvider;
        }

        public async Task Insert<T>(ConnectionData connectionData, string bucket, T data, string id)
        {
            var collection = await GetCollectionAsync(connectionData, bucket);
            await collection.InsertAsync(id, data);
        }

        public async Task<T> GetByIdAsync<T>(ConnectionData connectionData, string bucket, string id) where T : class
        {
            var collection = await GetCollectionAsync(connectionData, bucket);
            var getResult = await GetResultAsync(collection, id);
            return getResult?.ContentAs<T>();
        }
        
        public async Task CreateIndexAsync(ConnectionData connectionData, string bucketName, string indexName, IEnumerable<string> fields)
        {
            await _bucketProvider.CreateIndexAsync(connectionData, bucketName, indexName, fields);
        }
        
        public async Task CreatePrimaryIndexAsync(ConnectionData connectionData, string bucketName)
        {
            await _bucketProvider.CreatePrimaryIndexAsync(connectionData, bucketName);
        }
        
        public async Task CreateBucketAsync(ConnectionData connectionData, string bucketName)
        {
            await _bucketProvider.CreateBucketAsync(connectionData, bucketName);
        }
        
        private async Task<IGetResult> GetResultAsync(ICouchbaseCollection collection, string id)
        {
            try
            {
                var getResult = await collection.GetAsync(id);
                return getResult;
            }
            catch (DocumentNotFoundException)
            {
                return null;
            }
        }

        private async Task<ICouchbaseCollection> GetCollectionAsync(ConnectionData connectionData, string bucket)
        {
            return await (await GetBucketAsync(connectionData, bucket)).DefaultCollectionAsync();
        }

        public Task<IBucket> GetBucketAsync(ConnectionData connectionData, string bucket)
        {
            return _bucketProvider.GetBucket(connectionData, bucket);
        }
    }
}