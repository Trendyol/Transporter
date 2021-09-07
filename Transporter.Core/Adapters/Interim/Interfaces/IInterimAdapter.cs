using System.Collections.Generic;
using System.Threading.Tasks;
using Transporter.Core.Adapters.Base.Interfaces;

namespace Transporter.Core.Adapters.Interim.Interfaces
{
    public interface IInterimAdapter : IAdapter
    {
        Task<IEnumerable<dynamic>> GetAsync();
        Task DeleteAsync(IEnumerable<dynamic> ids);
    }
}