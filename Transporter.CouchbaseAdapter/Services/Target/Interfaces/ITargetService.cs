using System.Threading.Tasks;
using Transporter.CouchbaseAdapter.Configs.Target.Interfaces;

namespace Transporter.CouchbaseAdapter.Services.Target.Interfaces
{
    public interface ITargetService
    {
        Task SetTargetDataAsync(ICouchbaseTargetSettings settings, string data);

        Task SetInterimDataAsync(ICouchbaseTargetSettings settings, string data,
            string dataSourceName);
    }
}