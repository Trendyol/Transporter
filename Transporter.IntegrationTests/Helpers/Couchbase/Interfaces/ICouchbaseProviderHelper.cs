using System.Threading.Tasks;
using Transporter.CouchbaseAdapter.Utils;

namespace Transporter.IntegrationTests.Helpers.Couchbase.Interfaces
{
    public interface ICouchbaseProviderHelper
    {
        Task Insert<T>(ConnectionData connectionData, string bucket, T data, string id);
        Task<T> GetByIdAsync<T>(ConnectionData connectionData, string bucket, string id) where T : class;
    }
}