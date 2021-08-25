using System.Collections.Generic;
using System.Threading.Tasks;
using Transporter.MSSQLAdapter.Configs.Interim.Interfaces;

namespace Transporter.MSSQLAdapter.Services.Interim.Interfaces
{
    public interface IInterimService
    {
        Task<IEnumerable<dynamic>> GetInterimDataAsync(IMsSqlInterimSettings settings);
        Task DeleteAsync(IMsSqlInterimSettings settings, IEnumerable<dynamic> ids);        
    }
}