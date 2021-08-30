using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Transporter.Core.Adapters.Base.Interfaces;
using Transporter.Core.Adapters.Source.Interfaces;
using Transporter.Core.Configs.Base.Interfaces;
using Transporter.Core.Utils;
using Transporter.MSSQLDeleteAdapter.Configs.Source.Interfaces;
using Transporter.MSSQLDeleteAdapter.Services.Interfaces;
using Constants = Transporter.MSSQLDeleteAdapter.Utils.Constants;

namespace Transporter.MSSQLDeleteAdapter.Adapters
{
    public class SourceAdapter : ISourceAdapter
    {
        private readonly IConfiguration _configuration;
        private readonly ISourceService _sourceService;
        private IMsSqlSourceSettings _settings;


        public SourceAdapter(ISourceService sourceService, IConfiguration configuration)
        {
            _sourceService = sourceService;
            _configuration = configuration;
        }

        public bool CanHandle(ITransferJobSettings transferJobSettings)
        {
            var options = GetOptions(transferJobSettings);
            return string.Equals(options.Type, Constants.OptionsType,
                StringComparison.InvariantCultureIgnoreCase);
        }

        public bool CanHandle(IPollingJobSettings jobSetting)
        {
            var type = GetTypeBySettings(jobSetting);
            return string.Equals(type, Constants.OptionsType, StringComparison.InvariantCultureIgnoreCase);
        }

        public void SetOptions(ITransferJobSettings transferJobSettings)
        {
            _settings = GetOptions(transferJobSettings);
        }

        public void SetOptions(IPollingJobSettings jobSettings)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<dynamic>> GetAsync()
        {
            return await _sourceService.GetSourceDataAsync(_settings);
        }

        public Task<IEnumerable<dynamic>> GetAsync(IEnumerable<dynamic> ids)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(IEnumerable<dynamic> ids)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<dynamic>> GetIdsAsync()
        {
            throw new NotImplementedException();
        }

        public string GetDataSourceName()
        {
            return $"{_settings.Options.Schema}.{_settings.Options.Table}";
        }

        public string GetIdColumn()
        {
            return $"{_settings.Options.IdColumn}";
        }

        public virtual object Clone()
        {
            var result = MemberwiseClone() as IAdapter;
            return result;
        }

        private IMsSqlSourceSettings GetOptions(ITransferJobSettings transferJobSettings)
        {
            var jobOptionsList = _configuration[Core.Utils.Constants.TransferJobSettings]
                .ToObject<ICollection<MsSqlTransferJobSettings>>().ToList();
            var options = jobOptionsList.First(x => x.Name == transferJobSettings.Name);
            return (IMsSqlSourceSettings) options.Source;
        }

        private string GetTypeBySettings(IPollingJobSettings jobSettings)
        {
            var jobOptionsList = _configuration.GetSection(Core.Utils.Constants.PollingJobSettings)
                .Get<List<MsSqlTransferJobSettings>>();
            var options = jobOptionsList.First(x => x.Name == jobSettings.Name);
            return options.Source?.Type;
        }
    }
}