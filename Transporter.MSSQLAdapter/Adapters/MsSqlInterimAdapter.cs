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
using Transporter.MSSQLAdapter.Configs.Interim.Interfaces;
using Transporter.MSSQLAdapter.Services.Interim.Interfaces;
using Transporter.MSSQLAdapter.Utils;

namespace Transporter.MSSQLAdapter.Adapters
{
    public class MsSqlInterimAdapter : IInterimAdapter
    {
        private readonly IConfiguration _configuration;
        private readonly IInterimService _interimService;
        private IMsSqlInterimSettings _settings;

        public MsSqlInterimAdapter(IConfiguration configuration, IInterimService interimService)
        {
            _configuration = configuration;
            _interimService = interimService;
        }

        public object Clone()
        {
            var result = MemberwiseClone() as IAdapter;
            return result;
        }

        public bool CanHandle(ITransferJobSettings transferJobSettings)
        {
            var options = GetOptions(transferJobSettings);
            return string.Equals(options.Type, MsSqlAdapterConstants.OptionsType,
                StringComparison.InvariantCultureIgnoreCase);
        }

        public bool CanHandle(IPollingJobSettings jobSetting)
        {
            var type = GetTypeBySettings(jobSetting);
            return string.Equals(type, MsSqlAdapterConstants.OptionsType, StringComparison.InvariantCultureIgnoreCase);
        }

        public void SetOptions(ITransferJobSettings transferJobSettings)
        {
            _settings = GetOptions(transferJobSettings);
        }

        public void SetOptions(IPollingJobSettings jobSettings)
        {
            _settings = GetOptions(jobSettings);
        }

        public async Task<IEnumerable<dynamic>> GetAsync()
        {
            return await _interimService.GetInterimDataAsync(_settings);
        }

        public async Task DeleteAsync(IEnumerable<dynamic> ids)
        {
            await _interimService.DeleteAsync(_settings, ids);
        }

        private IMsSqlInterimSettings GetOptions(ITransferJobSettings transferJobSettings)
        {
            var jobOptionsList = JsonConvert.DeserializeObject<List<MsSqlTransferJobSettings>>(_configuration
                .GetSection(Constants.TransferJobSettings).Get<string>());
            var options = jobOptionsList.First(x => x.Name == transferJobSettings.Name);
            return (IMsSqlInterimSettings)options.Interim;
        }

        private IMsSqlInterimSettings GetOptions(IPollingJobSettings jobSettings)
        {
            var jobOptionsList = JsonConvert.DeserializeObject<List<MsSqlTransferJobSettings>>(_configuration
                .GetSection(Constants.PollingJobSettings).Get<string>());
            var options = jobOptionsList.First(x => x.Name == jobSettings.Name);
            return (IMsSqlInterimSettings)options.Interim;
        }

        private string GetTypeBySettings(IPollingJobSettings jobSettings)
        {
            var jobOptionsList = JsonConvert.DeserializeObject<List<MsSqlTransferJobSettings>>(_configuration
                .GetSection(Constants.PollingJobSettings).Get<string>());
            var options = jobOptionsList.First(x => x.Name == jobSettings.Name);
            return options.Interim?.Type;
        }
    }
}