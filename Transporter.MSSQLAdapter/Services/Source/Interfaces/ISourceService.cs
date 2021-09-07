using System.Collections.Generic;
using System.Threading.Tasks;
using Transporter.MSSQLAdapter.Configs.Source.Interfaces;

namespace Transporter.MSSQLAdapter.Services.Source.Interfaces
{
    public interface ISourceService
    {
        Task<IEnumerable<dynamic>> GetSourceDataAsync(IMsSqlSourceSettings setting, IEnumerable<dynamic> ids);
        Task DeleteDataByListOfIdsAsync(IMsSqlSourceSettings settings, IEnumerable<dynamic> ids);
        Task<IEnumerable<dynamic>> GetIdDataAsync(IMsSqlSourceSettings settings);
    }
}