using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Transporter.Core.Adapters.Base.Interfaces;
using Transporter.Core.Adapters.Interim.Interfaces;
using Transporter.Core.Configs.Base.Interfaces;
using Transporter.Core.Utils;
using Transporter.CouchbaseAdapter.Configs.Interim.Interfaces;
using Transporter.CouchbaseAdapter.Services.Interim.Interfaces;

namespace Transporter.CouchbaseAdapter.Adapters
{
    public class CouchbaseInterimAdapter(IConfiguration configuration, IInterimService interimService) : IInterimAdapter
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IInterimService _interimService = interimService;
        private ICouchbaseInterimSettings _settings;

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

        public async Task<IEnumerable<dynamic>> GetAsync() => await _interimService.GetInterimDataAsync(_settings);

        public async Task DeleteAsync(IEnumerable<dynamic> ids) => await _interimService.DeleteAsync(_settings, ids);

        private ICouchbaseInterimSettings GetOptions(ITransferJobSettings transferJobSettings)
        {
            var jobOptionsList = _configuration
                .GetSection(Constants.TransferJobSettings).Get<List<CouchbaseTransferJobSettings>>();
            var options = jobOptionsList.First(x => x.Name == transferJobSettings.Name);
            return (ICouchbaseInterimSettings)options.Interim;
        }

        private ICouchbaseInterimSettings GetOptions(IPollingJobSettings jobSettings)
        {
            var jobOptionsList = _configuration
                .GetSection(Constants.PollingJobSettings).Get<List<CouchbaseTransferJobSettings>>();
            var options = jobOptionsList.First(x => x.Name == jobSettings.Name);
            return (ICouchbaseInterimSettings)options.Interim;
        }

        private string GetTypeBySettings(IPollingJobSettings jobSettings)
        {
            var jobOptionsList = _configuration
                .GetSection(Constants.PollingJobSettings).Get<List<CouchbaseTransferJobSettings>>();
            var options = jobOptionsList.First(x => x.Name == jobSettings.Name);
            return options.Interim?.Type;
        }
    }
}