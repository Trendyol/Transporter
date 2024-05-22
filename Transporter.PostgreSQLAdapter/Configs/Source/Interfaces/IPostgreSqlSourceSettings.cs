using Transporter.Core.Configs.Source.Interfaces;
using Transporter.PostgreSQLAdapter.Configs.Source.Interfaces;

namespace Transporter.PostgreSQLAdapter.Configs.Source.Interfaces
{
    public interface IPostgreSqlSourceSettings : ISourceOptions
    {
        public IPostgreSqlSourceOptions Options { get; set; }
    }
}