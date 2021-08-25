using Transporter.Core.Configs.Base.Interfaces;

namespace Transporter.Core.Configs.Source.Interfaces
{
    public interface ISourceOptions : IOptions
    {
        public string Host { get; set; }
    }
}