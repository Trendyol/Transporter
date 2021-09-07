using Transporter.Core.Configs.Interim.Interfaces;

namespace Transporter.MSSQLAdapter.Configs.Interim.Interfaces
{
    public interface IMsSqlInterimSettings : IInterimOptions
    {
        public IMsSqlInterimOptions Options { get; set; }
    }
}