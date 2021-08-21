using System.Collections.Generic;
using System.Threading.Tasks;

namespace Transporter.MSSQLAdapter.Services
{
    public interface ISourceService
    {
        Task<IEnumerable<dynamic>> GetSourceDataAsync(ISqlSourceSettings setting, IEnumerable<dynamic> ids);
        Task DeleteDataByListOfIdsAsync(ISqlSourceSettings settings, IEnumerable<dynamic> ids);
        Task<IEnumerable<dynamic>> GetIdDataAsync(ISqlSourceSettings settings);
        Task SetSourceDataAsync(ISqlSourceSettings setting, string data);
    }
}