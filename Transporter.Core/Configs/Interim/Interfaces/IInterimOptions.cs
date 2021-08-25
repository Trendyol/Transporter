using Transporter.Core.Configs.Base.Interfaces;

namespace Transporter.Core.Configs.Interim.Interfaces
{
    public interface IInterimOptions : IOptions
    {
        public string Host { get; set; }
    }
}