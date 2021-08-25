using System.Threading.Tasks;
using Transporter.Core.Adapters.Base.Interfaces;

namespace Transporter.Core.Adapters.Target.Interfaces
{
    public interface ITargetAdapter : IAdapter
    {
        Task SetAsync(string data);
        Task SetTemporaryTableAsync(string data, string dataSourceName);
    }
}