using System.Collections.Generic;
using System.Threading.Tasks;
using Couchbase;
using Transporter.CouchbaseAdapter.Utils;

namespace Transporter.CouchbaseAdapter.Data.Interfaces
{
    public interface IBucketProvider
    {
        Task<IBucket> GetBucket(ConnectionData connectionData, string bucketName);

        Task CreateIndexAsync(ConnectionData connectionData, string bucketName, string indexName, 
            IEnumerable<string> fields);

        Task CreatePrimaryIndexAsync(ConnectionData connectionData, string bucketName);
        
        Task CreateBucketAsync(ConnectionData connectionData, string bucketName);
    }
}