using System.Collections.Generic;
using System.Threading.Tasks;
using Transporter.CouchbaseAdapter.ConfigOptions.Source.Interfaces;

namespace Transporter.CouchbaseAdapter.Services.Source.Interfaces
{
    public interface ISourceService
    {
        Task<IEnumerable<dynamic>> GetSourceDataAsync(ICouchbaseSourceSettings settings, IEnumerable<dynamic> ids);
        Task<IEnumerable<dynamic>> GetIdDataAsync(ICouchbaseSourceSettings settings);
        Task SetTargetDataAsync(ICouchbaseSourceSettings settings, string data);

        Task DeleteDataByListOfIdsAsync(ICouchbaseSourceSettings settings,
            IEnumerable<dynamic> ids);
    }
}