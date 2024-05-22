using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Transporter.Core.Adapters.Base.Interfaces;
using Transporter.Core.Adapters.Interim.Interfaces;
using Transporter.Core.Configs.Base.Interfaces;
using Transporter.Core.Utils;
using Transporter.PostgreSQLAdapter.Configs.Interim.Interfaces;
using Transporter.PostgreSQLAdapter.Services.Interim.Interfaces;
using Transporter.PostgreSQLAdapter.Utils;

namespace Transporter.PostgreSQLAdapter.Adapters
{
    public class PostgreSqlInterimAdapter : IInterimAdapter
    {
        private readonly IConfiguration _configuration;
        private readonly IInterimService _interimService;
        private IPostgreSqlInterimSettings _settings;
        public PostgreSqlInterimAdapter(IConfiguration configuration, IInterimService interimService)
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
            return string.Equals(options.Type, PostgreSqlAdapterConstants.OptionsType,
                StringComparison.InvariantCultureIgnoreCase);
        }

        public bool CanHandle(IPollingJobSettings jobSetting)
        {
            var type = GetTypeBySettings(jobSetting);
            return string.Equals(type, PostgreSqlAdapterConstants.OptionsType, StringComparison.InvariantCultureIgnoreCase);
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

        private IPostgreSqlInterimSettings GetOptions(ITransferJobSettings transferJobSettings)
        {
            var jobOptionsList = _configuration
                .GetSection(Constants.TransferJobSettings).Get<List<PostgreSqlTransferJobSettings>>();
            var options = jobOptionsList.First(x => x.Name == transferJobSettings.Name);
            return (IPostgreSqlInterimSettings)options.Interim;
        }

        private IPostgreSqlInterimSettings GetOptions(IPollingJobSettings jobSettings)
        {
            var jobOptionsList = _configuration
                .GetSection(Constants.PollingJobSettings).Get<List<PostgreSqlTransferJobSettings>>();
            var options = jobOptionsList.First(x => x.Name == jobSettings.Name);
            return (IPostgreSqlInterimSettings)options.Interim;
        }

        private string GetTypeBySettings(IPollingJobSettings jobSettings)
        {
            var jobOptionsList = _configuration
                .GetSection(Constants.PollingJobSettings).Get<List<PostgreSqlTransferJobSettings>>();
            var options = jobOptionsList.First(x => x.Name == jobSettings.Name);
            return options.Interim?.Type;
        }
    }
}
