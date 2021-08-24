using Transporter.Core;

namespace Transporter.MSSQLAdapter.ConfigOptions.Interim.Interfaces
{
    public interface IMsSqlInterimSettings : IInterimOptions
    {
        public IMsSqlInterimOptions Options { get; set; }
    }
}