using System.Threading.Tasks;
using Couchbase;
using Transporter.CouchbaseAdapter.Utils;

namespace Transporter.CouchbaseAdapter.Data.Interfaces
{
    public interface IBucketProvider
    {
        Task<IBucket> GetBucket(ConnectionData connectionData, string bucket);
    }
}