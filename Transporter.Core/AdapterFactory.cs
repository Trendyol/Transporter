using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transporter.Core
{
    public class AdapterFactory : IAdapterFactory
    {
        private readonly IEnumerable<ISourceAdapter> _sourceAdapters;
        private readonly IEnumerable<ITargetAdapter> _targetAdapters;

        public AdapterFactory(IEnumerable<ISourceAdapter> sourceAdapters,
            IEnumerable<ITargetAdapter> targetAdapters)
        {
            _sourceAdapters = sourceAdapters;
            _targetAdapters = targetAdapters;
        }

        public async Task<T> GetAsync<T>(JobSettings options) where T : IAdapter
        {
            var adapter = await GetAdapterAsync<T>(options);
            if (adapter is null) return default;
            adapter.SetOptions(options);
            return adapter;
        }
        
        public async Task<T> GetAsync<T>(TemporaryTableOptions.TemporaryTableJobSettings options) where T : IAdapter
        {
            var adapter = await GetAdapterAsync<T>(options);
            if (adapter is null) return default;
            adapter.SetOptions(options);
            return adapter;
        }
        
        private async Task<T> GetAdapterAsync<T>(TemporaryTableOptions.TemporaryTableJobSettings options) where T : IAdapter
        {
            var properAdapter = default(T);
            if (typeof(T) == typeof(ITargetAdapter))
                properAdapter = (T) _targetAdapters.FirstOrDefault(x => x.CanHandle(options));

            if (typeof(T) == typeof(ISourceAdapter))
                properAdapter = (T) _sourceAdapters.FirstOrDefault(x => x.CanHandle(options));

            return (T) await Task.Run(() => properAdapter?.Clone());
        }

        private async Task<T> GetAdapterAsync<T>(JobSettings options) where T : IAdapter
        {
            var properAdapter = default(T);
            if (typeof(T) == typeof(ITargetAdapter))
                properAdapter = (T) _targetAdapters.FirstOrDefault(x => x.CanHandle(options));

            if (typeof(T) == typeof(ISourceAdapter))
                properAdapter = (T) _sourceAdapters.FirstOrDefault(x => x.CanHandle(options));

            return (T) await Task.Run(() => properAdapter?.Clone());
        }
    }
}