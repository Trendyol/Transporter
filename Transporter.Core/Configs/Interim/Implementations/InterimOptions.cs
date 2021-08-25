using Transporter.Core.Configs.Interim.Interfaces;

namespace Transporter.Core.Configs.Interim.Implementations
{
    public class InterimOptions : IInterimOptions
    {
        public string Host { get; set; }
        public string Type { get; set; }
    }
}