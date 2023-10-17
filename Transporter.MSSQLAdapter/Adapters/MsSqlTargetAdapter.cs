using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Transporter.Core.Adapters.Base.Interfaces;
using Transporter.Core.Adapters.Target.Interfaces;
using Transporter.Core.Configs.Base.Interfaces;
using Transporter.Core.Utils;
using Transporter.MSSQLAdapter.Configs.Target.Interfaces;
using Transporter.MSSQLAdapter.Services.Target.Interfaces;
using Transporter.MSSQLAdapter.Utils;

namespace Transporter.MSSQLAdapter.Adapters
{
    public class MsSqlTargetAdapter : ITargetAdapter
    {
        private readonly IConfiguration _configuration;
        private readonly ITargetService _targetService;
        private IMsSqlTargetSettings _settings;

        public MsSqlTargetAdapter(ITargetService targetService, IConfiguration configuration)
        {
            _targetService = targetService;
            _configuration = configuration;
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

        public async Task SetAsync(string data)
        {
            await _targetService.SetTargetDataAsync(_settings, data);
        }

        public async Task SetInterimTableAsync(string data, string dataSourceName)
        {
            await _targetService.SetTargetTemporaryDataAsync(_settings, data, dataSourceName);
        }

        public virtual object Clone()
        {
            var result = MemberwiseClone() as IAdapter;
            return result;
        }

        private IMsSqlTargetSettings GetOptions(IPollingJobSettings jobSettings)
        {
            var jobOptionsList = _configuration
                .GetSection(Constants.PollingJobSettings).Get<List<MsSqlTransferJobSettings>>();
            var options = jobOptionsList.First(x => x.Name == jobSettings.Name);
            return (IMsSqlTargetSettings)options.Target;
        }

        private IMsSqlTargetSettings GetOptions(ITransferJobSettings transferJobSettings)
        {
            var jobOptionsList = _configuration
                .GetSection(Constants.TransferJobSettings).Get<List<MsSqlTransferJobSettings>>();
            var options = jobOptionsList.First(x => x.Name == transferJobSettings.Name);
            return (IMsSqlTargetSettings)options.Target;
        }

        private string GetTypeBySettings(IPollingJobSettings jobSettings)
        {
            var jobOptionsList = _configuration
                .GetSection(Constants.PollingJobSettings).Get<List<MsSqlTransferJobSettings>>();
            var options = jobOptionsList.First(x => x.Name == jobSettings.Name);
            return options.Target?.Type;
        }
    }
}