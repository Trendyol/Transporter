using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Transporter.Core.Adapters.Base.Interfaces;
using Transporter.Core.Adapters.Target.Interfaces;
using Transporter.Core.Configs.Base.Interfaces;
using Transporter.Core.Utils;
using Transporter.CouchbaseAdapter.Configs.Target.Interfaces;
using Transporter.CouchbaseAdapter.Services.Target.Interfaces;

namespace Transporter.CouchbaseAdapter.Adapters
{
    public class CouchbaseTargetAdapter : ITargetAdapter
    {
        private readonly ITargetService _targetService;
        private readonly IConfiguration _configuration;
        private ICouchbaseTargetSettings _settings;

        public CouchbaseTargetAdapter(ITargetService targetService, IConfiguration configuration)
        {
            _targetService = targetService;
            _configuration = configuration;
        }

        public object Clone()
        {
            var result = MemberwiseClone() as IAdapter;
            return result;
        }

        public bool CanHandle(ITransferJobSettings transferJobSetting)
        {
            var options = GetOptions(transferJobSetting);
            return string.Equals(options.Type, Utils.Constants.OptionsType,
                StringComparison.InvariantCultureIgnoreCase);
        }

        public bool CanHandle(IPollingJobSettings jobSetting)
        {
            var type = GetTypeBySettings(jobSetting);
            return string.Equals(type, Utils.Constants.OptionsType, StringComparison.InvariantCultureIgnoreCase);
        }

        public void SetOptions(ITransferJobSettings transferJobSettings)
        {
            _settings = GetOptions(transferJobSettings);
        }

        public void SetOptions(IPollingJobSettings jobSettings)
        {
            _settings = GetOptions(jobSettings);
        }

        public async Task SetAsync(string data)
        {
            await _targetService.SetTargetDataAsync(_settings, data);
        }

        public async Task SetInterimTableAsync(string data, string dataSourceName)
        {
            await _targetService.SetInterimDataAsync(_settings, data, dataSourceName);
        }

        private ICouchbaseTargetSettings GetOptions(IPollingJobSettings jobSettings)
        {
            var jobOptionsList = _configuration.GetSection(Constants.PollingJobSettings)
                .Get<List<CouchbaseTransferJobSettings>>();
            var options = jobOptionsList.First(x => x.Name == jobSettings.Name);
            return (ICouchbaseTargetSettings)options.Target;
        }
        
        private string GetTypeBySettings(IPollingJobSettings jobSettings)
        {
            var jobOptionsList = _configuration.GetSection(Constants.PollingJobSettings)
                .Get<List<CouchbaseTransferJobSettings>>();
            var options = jobOptionsList.First(x => x.Name == jobSettings.Name);
            return options.Target?.Type;
        }

        private ICouchbaseTargetSettings GetOptions(ITransferJobSettings transferJobSettings)
        {
            var jobOptionsList = _configuration.GetSection(Constants.TransferJobSettings)
                .Get<List<CouchbaseTransferJobSettings>>();
            var options = jobOptionsList.First(x => x.Name == transferJobSettings.Name);
            return (ICouchbaseTargetSettings) options.Target;
        }
    }
}