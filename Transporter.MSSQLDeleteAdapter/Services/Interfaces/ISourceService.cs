using System.Collections.Generic;
using System.Threading.Tasks;
using Transporter.MSSQLDeleteAdapter.Configs.Source.Interfaces;

namespace Transporter.MSSQLDeleteAdapter.Services.Interfaces
{
    public interface ISourceService
    {
        Task<IEnumerable<dynamic>> GetSourceDataAsync(IMsSqlSourceSettings setting);
    }
}