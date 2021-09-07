using Transporter.Core.Configs.Source.Interfaces;

namespace Transporter.Core.Configs.Source.Implementations
{
    public class SourceOptions : ISourceOptions
    {
        public string Type { get; set; }
        public string Host { get; set; }
    }
}