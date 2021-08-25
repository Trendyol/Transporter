using System.Threading.Tasks;
using Transporter.Core.Adapters.Base.Interfaces;

namespace Transporter.Core.Factories.Adapter.Interfaces
{
    public interface IAdapterFactory
    {
        Task<T> GetAsync<T>(JobSettings options) where T : IAdapter;
        Task<T> GetAsync<T>(TemporaryTableOptions.TemporaryTableJobSettings options) where T : IAdapter;
    }
}