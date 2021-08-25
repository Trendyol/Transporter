using System;

namespace Transporter.Core.Adapters.Base.Interfaces
{
    public interface IAdapter : ICloneable
    {
        bool CanHandle(IJobSettings jobSetting);
        bool CanHandle(TemporaryTableOptions.ITemporaryTableJobSettings jobSetting);
        void SetOptions(IJobSettings jobSettings);
        void SetOptions(TemporaryTableOptions.ITemporaryTableJobSettings jobSettings);
    }
}