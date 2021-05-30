using System.Collections.Generic;
using System.Threading.Tasks;

namespace Transporter.MSSQLDeleteAdapter.Services
{
    public interface ISourceService
    {
        Task<IEnumerable<dynamic>> GetSourceDataAsync(ISqlSourceSettings setting);
    }
}