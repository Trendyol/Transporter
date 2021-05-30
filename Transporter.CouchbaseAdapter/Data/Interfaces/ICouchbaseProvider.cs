using System.Threading.Tasks;
using Couchbase;
using Transporter.CouchbaseAdapter.Utils;

namespace Transporter.CouchbaseAdapter.Data.Interfaces
{
    public interface ICouchbaseProvider
    {
        Task<ICluster> GetCluster(ConnectionData connectionData);
    }
}