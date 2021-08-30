using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transporter.Core.Adapters.Base.Interfaces;
using Transporter.Core.Adapters.Interim.Interfaces;
using Transporter.Core.Adapters.Source.Interfaces;
using Transporter.Core.Adapters.Target.Interfaces;
using Transporter.Core.Configs.Base.Implementations;
using Transporter.Core.Factories.Adapter.Interfaces;

namespace Transporter.Core.Factories.Adapter.Implementations
{
    public class AdapterFactory : IAdapterFactory
    {
        private readonly IEnumerable<ISourceAdapter> _sourceAdapters;
        private readonly IEnumerable<ITargetAdapter> _targetAdapters;
        private readonly IEnumerable<IInterimAdapter> _interimAdapters;

        public AdapterFactory(IEnumerable<ISourceAdapter> sourceAdapters,
            IEnumerable<ITargetAdapter> targetAdapters, IEnumerable<IInterimAdapter> interimAdapters)
        {
            _sourceAdapters = sourceAdapters;
            _targetAdapters = targetAdapters;
            _interimAdapters = interimAdapters;
        }

        public async Task<T> GetAsync<T>(TransferJobSettings options) where T : IAdapter
        {
            var adapter = await GetAdapterAsync<T>(options);
            if (adapter is null) return default;
            adapter.SetOptions(options);
            return adapter;
        }
        
        public async Task<T> GetAsync<T>(PollingJobSettings options) where T : IAdapter
        {
            var adapter = await GetAdapterAsync<T>(options);
            if (adapter is null) return default;
            adapter.SetOptions(options);
            return adapter;
        }
        
        private async Task<T> GetAdapterAsync<T>(PollingJobSettings options) where T : IAdapter
        {
            var properAdapter = default(T);
            if (typeof(T) == typeof(ITargetAdapter))
                properAdapter = (T) _targetAdapters.FirstOrDefault(x => x.CanHandle(options));

            if (typeof(T) == typeof(ISourceAdapter))
                properAdapter = (T) _sourceAdapters.FirstOrDefault(x => x.CanHandle(options));

            return (T) await Task.Run(() => properAdapter?.Clone());
        }

        private async Task<T> GetAdapterAsync<T>(TransferJobSettings options) where T : IAdapter
        {
            var properAdapter = default(T);
            if (typeof(T) == typeof(ITargetAdapter))
                properAdapter = (T) _targetAdapters.FirstOrDefault(x => x.CanHandle(options));

            if (typeof(T) == typeof(ISourceAdapter))
                properAdapter = (T) _sourceAdapters.FirstOrDefault(x => x.CanHandle(options));
            
            if (typeof(T) == typeof(IInterimAdapter))
                properAdapter = (T) _interimAdapters.FirstOrDefault(x => x.CanHandle(options));

            return (T) await Task.Run(() => properAdapter?.Clone());
        }
    }
}