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
    public class CouchbaseSourceAdapter(ISourceService sourceService, IConfiguration configuration) : ISourceAdapter
    {
        private readonly ISourceService _sourceService = sourceService;
        private readonly IConfiguration _configuration = configuration;
        private ICouchbaseSourceSettings _settings;

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

        public void SetOptions(ITransferJobSettings transferJobSettings) => _settings = GetOptions(transferJobSettings);

        public void SetOptions(IPollingJobSettings jobSettings) => _settings = GetOptions(jobSettings);

        public async Task<IEnumerable<dynamic>> GetAsync(IEnumerable<dynamic> ids) =>
            await _sourceService.GetSourceDataAsync(_settings, ids);

        public async Task DeleteAsync(IEnumerable<dynamic> ids) => await _sourceService.DeleteDataByListOfIdsAsync(_settings, ids);

        public async Task<IEnumerable<dynamic>> GetIdsAsync() => await _sourceService.GetIdDataAsync(_settings);

        public string GetDataSourceName() => $"{_settings.Options.Bucket}";

        private ICouchbaseSourceSettings GetOptions(ITransferJobSettings transferJobSettings)
        {
            var jobOptionsList = _configuration
                .GetSection(Constants.TransferJobSettings).Get<List<CouchbaseTransferJobSettings>>();
            var options = jobOptionsList.First(x => x.Name == transferJobSettings.Name);
            return (ICouchbaseSourceSettings)options.Source;
        }

        private ICouchbaseSourceSettings GetOptions(IPollingJobSettings jobSettings)
        {
            var jobOptionsList = _configuration
                .GetSection(Constants.PollingJobSettings).Get<List<CouchbaseTransferJobSettings>>();
            var options = jobOptionsList.First(x => x.Name == jobSettings.Name);
            return (ICouchbaseSourceSettings)options.Source;
        }

        private string GetTypeBySettings(IPollingJobSettings jobSettings)
        {
            var jobOptionsList = _configuration
                .GetSection(Constants.PollingJobSettings).Get<List<CouchbaseTransferJobSettings>>();
            var options = jobOptionsList.First(x => x.Name == jobSettings.Name);
            return options.Source?.Type;
        }
    }
}