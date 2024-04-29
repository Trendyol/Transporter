using System.Collections.Generic;
using System.Threading.Tasks;
using Transporter.CouchbaseAdapter.Utils;

namespace Transporter.IntegrationTests.Helpers.Couchbase.Interfaces
{
    public interface ICouchbaseProviderHelper
    {
        Task Insert<T>(ConnectionData connectionData, string bucket, T data, string id);
        Task<T> GetByIdAsync<T>(ConnectionData connectionData, string bucket, string id) where T : class;
        Task CreateIndexAsync(ConnectionData connectionData, string bucketName, string indexName,
            IEnumerable<string> fields);
        Task CreatePrimaryIndexAsync(ConnectionData connectionData, string bucketName);
        Task CreateBucketAsync(ConnectionData connectionData, string bucketName);
    }
}