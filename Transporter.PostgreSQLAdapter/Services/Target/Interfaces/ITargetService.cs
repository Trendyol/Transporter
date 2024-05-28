using System.Threading.Tasks;
using Transporter.PostgreSQLAdapter.Configs.Target.Interfaces;

namespace Transporter.PostgreSQLAdapter.Services.Target.Interfaces
{
    public interface ITargetService
    {
        Task SetTargetDataAsync(IPostgreSqlTargetSettings setting, string data);
        Task SetTargetTemporaryDataAsync(IPostgreSqlTargetSettings setting, string data, string dataSourceName);
    }
}