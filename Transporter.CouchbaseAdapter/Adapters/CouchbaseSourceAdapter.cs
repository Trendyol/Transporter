using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Transporter.Core.Adapters.Base.Interfaces;
using Transporter.Core.Adapters.Source.Interfaces;
using Transporter.Core.Configs.Base.Interfaces;
using Transporter.Core.Utils;
using Transporter.CouchbaseAdapter.Configs.Source.Interfaces;
using Transporter.CouchbaseAdapter.Services.Source.Interfaces;

namespace Transporter.CouchbaseAdapter.Adapters
{
    public class CouchbaseSourceAdapter : ISourceAdapter
    {
        private readonly ISourceService _sourceService;
        private readonly IConfiguration _configuration;
        private ICouchbaseSourceSettings _settings;

        public CouchbaseSourceAdapter(ISourceService sourceService, IConfiguration configuration)
        {
            _sourceService = sourceService;
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

        public async Task<IEnumerable<dynamic>> GetAsync(IEnumerable<dynamic> ids)
        {
            return await _sourceService.GetSourceDataAsync(_settings, ids);
        }

        public async Task DeleteAsync(IEnumerable<dynamic> ids)
        {
            await _sourceService.DeleteDataByListOfIdsAsync(_settings, ids);
        }

        public async Task<IEnumerable<dynamic>> GetIdsAsync()
        {
            return await _sourceService.GetIdDataAsync(_settings);
        }

        public string GetDataSourceName()
        {
            return $"{_settings.Options.Bucket}";
        }

        private ICouchbaseSourceSettings GetOptions(ITransferJobSettings transferJobSettings)
        {
            var jobOptionsList = JsonConvert.DeserializeObject<List<CouchbaseTransferJobSettings>>(_configuration
                .GetSection(Constants.TransferJobSettings).Get<string>());
            var options = jobOptionsList.First(x => x.Name == transferJobSettings.Name);
            return (ICouchbaseSourceSettings)options.Source;
        }

        private ICouchbaseSourceSettings GetOptions(IPollingJobSettings jobSettings)
        {
            var jobOptionsList = JsonConvert.DeserializeObject<List<CouchbaseTransferJobSettings>>(_configuration
                .GetSection(Constants.PollingJobSettings).Get<string>());
            var options = jobOptionsList.First(x => x.Name == jobSettings.Name);
            return (ICouchbaseSourceSettings)options.Source;
        }

        private string GetTypeBySettings(IPollingJobSettings jobSettings)
        {
            var jobOptionsList = JsonConvert.DeserializeObject<List<CouchbaseTransferJobSettings>>(_configuration
                .GetSection(Constants.PollingJobSettings).Get<string>());
            var options = jobOptionsList.First(x => x.Name == jobSettings.Name);
            return options.Source?.Type;
        }
    }
}