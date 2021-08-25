using System.Collections.Generic;
using System.Threading.Tasks;
using Transporter.CouchbaseAdapter.Configs.Interim.Interfaces;

namespace Transporter.CouchbaseAdapter.Services.Interim.Interfaces
{
    public interface IInterimService
    {
        Task<IEnumerable<dynamic>> GetInterimDataAsync(ICouchbaseInterimSettings settings);
        Task DeleteAsync(ICouchbaseInterimSettings settings, IEnumerable<dynamic> ids);
    }
}