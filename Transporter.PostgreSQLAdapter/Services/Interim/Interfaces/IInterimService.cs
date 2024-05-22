using System.Collections.Generic;
using System.Threading.Tasks;
using Transporter.PostgreSQLAdapter.Configs.Interim.Interfaces;

namespace Transporter.PostgreSQLAdapter.Services.Interim.Interfaces
{
    public interface IInterimService
    {
        Task<IEnumerable<dynamic>> GetInterimDataAsync(IPostgreSqlInterimSettings settings);
        Task DeleteAsync(IPostgreSqlInterimSettings settings, IEnumerable<dynamic> ids);        
    }
}