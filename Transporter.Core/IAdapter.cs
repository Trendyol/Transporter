using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Octopus.Transporter.Core
{
    public interface IAdapter : ICloneable
    {
        bool CanHandle(IJobSettings jobSetting);
        void SetOptions(IJobSettings jobSettings);
    }

    public interface ITargetAdapter : IAdapter
    {
        Task SetAsync(string data);
    }

    public interface ISourceAdapter : IAdapter
    {
        Task<IEnumerable<dynamic>> GetAsync();
    }
}