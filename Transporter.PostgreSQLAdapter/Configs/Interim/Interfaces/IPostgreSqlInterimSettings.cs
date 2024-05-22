using Transporter.Core.Configs.Interim.Interfaces;
using Transporter.PostgreSQLAdapter.Configs.Interim.Interfaces;

namespace Transporter.PostgreSQLAdapter.Configs.Interim.Interfaces
{
    public interface IPostgreSqlInterimSettings : IInterimOptions
    {
        public IPostgreSqlInterimOptions Options { get; set; }
    }
}