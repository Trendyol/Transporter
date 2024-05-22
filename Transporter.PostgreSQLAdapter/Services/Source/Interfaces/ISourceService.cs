using System.Collections.Generic;
using System.Threading.Tasks;
using Transporter.PostgreSQLAdapter.Configs.Source.Interfaces;

namespace Transporter.PostgreSQLAdapter.Services.Source.Interfaces
{
    public interface ISourceService
    {
        Task<IEnumerable<dynamic>> GetSourceDataAsync(IPostgreSqlSourceSettings setting, IEnumerable<dynamic> ids);
        Task DeleteDataByListOfIdsAsync(IPostgreSqlSourceSettings settings, IEnumerable<dynamic> ids);
        Task<IEnumerable<dynamic>> GetIdDataAsync(IPostgreSqlSourceSettings settings);
    }
}