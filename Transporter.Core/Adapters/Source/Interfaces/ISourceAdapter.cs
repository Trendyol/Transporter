using System.Collections.Generic;
using System.Threading.Tasks;
using Transporter.Core.Adapters.Base.Interfaces;

namespace Transporter.Core.Adapters.Source.Interfaces
{
    public interface ISourceAdapter : IAdapter
    {
        Task<IEnumerable<dynamic>> GetAsync(IEnumerable<dynamic> ids);
        Task DeleteAsync(IEnumerable<dynamic> ids);
        Task<IEnumerable<dynamic>> GetIdDataAsync();
        string GetDataSourceName();
    }
}