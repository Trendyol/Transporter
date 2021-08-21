using System.Collections.Generic;
using System.Threading.Tasks;
using Transporter.CouchbaseAdapter.ConfigOptions.Interim.Interfaces;

namespace Transporter.CouchbaseAdapter.Services.Interfaces
{
    public interface IInterimService
    {
        Task<IEnumerable<dynamic>> GetInterimDataAsync(ICouchbaseInterimSettings settings);
        Task DeleteAsync(ICouchbaseInterimSettings settings, IEnumerable<dynamic> ids);
    }
}