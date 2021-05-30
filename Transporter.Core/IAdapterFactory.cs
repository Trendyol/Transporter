using System.Threading.Tasks;

namespace Transporter.Core
{
    public interface IAdapterFactory
    {
        Task<T> GetAsync<T>(JobSettings options) where T : IAdapter;
    }
}