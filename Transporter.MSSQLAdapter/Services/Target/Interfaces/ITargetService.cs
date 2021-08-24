using System.Threading.Tasks;

namespace Transporter.MSSQLAdapter.Services.Target.Interfaces
{
    public interface ITargetService
    {
        Task SetTargetDataAsync(ISqlTargetSettings setting, string data);
        Task SetTargetTemporaryDataAsync(ISqlTargetSettings setting, string data, string dataSourceName);
    }
}