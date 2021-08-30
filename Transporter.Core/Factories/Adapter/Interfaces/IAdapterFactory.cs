using System.Threading.Tasks;
using Transporter.Core.Adapters.Base.Interfaces;
using Transporter.Core.Configs.Base.Implementations;

namespace Transporter.Core.Factories.Adapter.Interfaces
{
    public interface IAdapterFactory
    {
        Task<T> GetAsync<T>(TransferJobSettings options) where T : IAdapter;
        Task<T> GetAsync<T>(PollingJobSettings options) where T : IAdapter;
    }
}