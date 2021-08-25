using Transporter.Core.Configs.Source.Interfaces;

namespace Transporter.MSSQLDeleteAdapter.Configs.Source.Interfaces
{
    public interface IMsSqlSourceSettings : ISourceOptions
    {
        public IMsSqlSourceOptions Options { get; set; }
    }
}