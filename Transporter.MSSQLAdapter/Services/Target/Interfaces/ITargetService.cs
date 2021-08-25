using System.Threading.Tasks;
using Transporter.MSSQLAdapter.Configs.Target.Interfaces;

namespace Transporter.MSSQLAdapter.Services.Target.Interfaces
{
    public interface ITargetService
    {
        Task SetTargetDataAsync(IMsSqlTargetSettings setting, string data);
        Task SetTargetTemporaryDataAsync(IMsSqlTargetSettings setting, string data, string dataSourceName);
    }
}