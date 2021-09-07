using System.Collections.Generic;
using System.Threading.Tasks;
using Transporter.CouchbaseAdapter.Configs.Source.Interfaces;

namespace Transporter.CouchbaseAdapter.Services.Source.Interfaces
{
    public interface ISourceService
    {
        Task<IEnumerable<dynamic>> GetSourceDataAsync(ICouchbaseSourceSettings settings, IEnumerable<dynamic> ids);
        Task<IEnumerable<dynamic>> GetIdDataAsync(ICouchbaseSourceSettings settings);
        Task DeleteDataByListOfIdsAsync(ICouchbaseSourceSettings settings,
            IEnumerable<dynamic> ids);
    }
}