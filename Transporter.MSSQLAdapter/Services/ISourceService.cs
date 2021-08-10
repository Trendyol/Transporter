using System.Collections.Generic;
using System.Threading.Tasks;

namespace Transporter.MSSQLAdapter.Services
{
    public interface ISourceService
    {
        Task<IEnumerable<dynamic>> GetSourceDataAsync(ISqlSourceSettings setting);
        Task<IEnumerable<dynamic>> GetIdDataAsync(ISqlSourceSettings settings);
        Task SetSourceDataAsync(ISqlSourceSettings setting, string data);
    }
}