using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Transporter.Core
{
    public interface IAdapter : ICloneable
    {
        bool CanHandle(IJobSettings jobSetting);
        bool CanHandle(TemporaryTableOptions.ITemporaryTableJobSettings jobSetting);
        void SetOptions(IJobSettings jobSettings);
        void SetOptions(TemporaryTableOptions.ITemporaryTableJobSettings jobSettings);
    }

    public interface ITargetAdapter : IAdapter
    {
        Task SetAsync(string data);
        Task SetTemporaryTableAsync(string data, string dataSourceName);
    }

    public interface ISourceAdapter : IAdapter
    {
        Task<IEnumerable<dynamic>> GetAsync(IEnumerable<dynamic> ids);
        Task DeleteAsync(IEnumerable<dynamic> ids);
        Task<IEnumerable<dynamic>> GetIdDataAsync();
        string GetDataSourceName();
    }

    public interface IInterimAdapter : IAdapter
    {
        Task<IEnumerable<dynamic>> GetAsync();
        Task DeleteAsync(IEnumerable<dynamic> ids);
    }
}