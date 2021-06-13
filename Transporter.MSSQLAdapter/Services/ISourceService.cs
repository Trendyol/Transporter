using System.Collections.Generic;
using System.Threading.Tasks;

namespace Transporter.MSSQLAdapter.Services
{
    public interface ISourceService
    {
        Task<IEnumerable<dynamic>> GetSourceDataAsync(ISqlSourceSettings setting);
        Task SetSourceDataAsync(ISqlSourceSettings setting, string data);
    }
}