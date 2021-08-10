using System.Collections.Generic;
using System.Threading.Tasks;
using Transporter.CouchbaseAdapter.ConfigOptions.Source.Interfaces;

namespace Transporter.CouchbaseAdapter.Services.Interfaces
{
    public interface ISourceService
    {
        Task<IEnumerable<dynamic>> GetSourceDataAsync(ICouchbaseSourceSettings settings);
        Task<IEnumerable<dynamic>> GetIdDataAsync(ICouchbaseSourceSettings settings);
        Task SetTargetDataAsync(ICouchbaseSourceSettings settings, string data);
    }
}