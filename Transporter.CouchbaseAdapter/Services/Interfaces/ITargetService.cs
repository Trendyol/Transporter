using System.Threading.Tasks;
using Transporter.CouchbaseAdapter.ConfigOptions.Target.Interfaces;

namespace Transporter.CouchbaseAdapter.Services.Interfaces
{
    public interface ITargetService
    {
        Task SetTargetDataAsync(ICouchbaseTargetSettings settings, string data);
    }
}