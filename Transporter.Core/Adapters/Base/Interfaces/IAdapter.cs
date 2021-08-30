using System;
using Transporter.Core.Configs.Base.Interfaces;

namespace Transporter.Core.Adapters.Base.Interfaces
{
    public interface IAdapter : ICloneable
    {
        bool CanHandle(ITransferJobSettings transferJobSetting);
        bool CanHandle(IPollingJobSettings jobSetting);
        void SetOptions(ITransferJobSettings transferJobSettings);
        void SetOptions(IPollingJobSettings jobSettings);
    }
}