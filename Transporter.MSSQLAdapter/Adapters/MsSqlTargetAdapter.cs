using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Transporter.Core;
using Transporter.MSSQLAdapter.Services;
using Transporter.MSSQLAdapter.Services.Target.Interfaces;

namespace Transporter.MSSQLAdapter.Adapters
{
    public class MsSqlTargetAdapter : ITargetAdapter
    {
        private readonly IConfiguration _configuration;
        private readonly ITargetService _targetService;
        private ISqlTargetSettings _settings;


        public MsSqlTargetAdapter(ITargetService targetService, IConfiguration configuration)
        {
            _targetService = targetService;
            _configuration = configuration;
        }

        public bool CanHandle(IJobSettings jobSettings)
        {
            var options = GetOptions(jobSettings);
            return string.Equals(options.Type, MsSqlAdapterConstants.OptionsType,
                StringComparison.InvariantCultureIgnoreCase);
        }

        public bool CanHandle(TemporaryTableOptions.ITemporaryTableJobSettings jobSetting)
        {
            var type = GetTypeBySettings(jobSetting);
            return string.Equals(type, MsSqlAdapterConstants.OptionsType, StringComparison.InvariantCultureIgnoreCase);
        }

        public void SetOptions(IJobSettings jobSettings)
        {
            _settings = GetOptions(jobSettings);
        }

        public void SetOptions(TemporaryTableOptions.ITemporaryTableJobSettings jobSettings)
        {
            _settings = GetOptions(jobSettings);
        }

        public async Task SetAsync(string data)
        {
            await _targetService.SetTargetDataAsync(_settings, data);
        }

        public async Task SetTemporaryTableAsync(string data, string dataSourceName)
        {
            await _targetService.SetTargetTemporaryDataAsync(_settings, data, dataSourceName);
        }

        public virtual object Clone()
        {
            var result = MemberwiseClone() as IAdapter;
            return result;
        }
        
        private ISqlTargetSettings GetOptions(TemporaryTableOptions.ITemporaryTableJobSettings jobSettings)
        {
            var jobOptionsList = _configuration.GetSection(Constants.TemporaryJobListSectionKey).Get<List<MsSqlJobSettings>>();
            var options = jobOptionsList.First(x => x.Name == jobSettings.Name);
            return (ISqlTargetSettings) options.Target;
        }

        private ISqlTargetSettings GetOptions(IJobSettings jobSettings)
        {
            var jobOptionsList = _configuration.GetSection(Constants.JobListSectionKey).Get<List<MsSqlJobSettings>>();
            var options = jobOptionsList.First(x => x.Name == jobSettings.Name);
            return (ISqlTargetSettings) options.Target;
        }
        
        private string GetTypeBySettings(TemporaryTableOptions.ITemporaryTableJobSettings jobSettings)
        {
            var jobOptionsList = _configuration.GetSection(Constants.TemporaryJobListSectionKey)
                .Get<List<MsSqlJobSettings>>();
            var options = jobOptionsList.First(x => x.Name == jobSettings.Name);
            return options.Target?.Type;
        }
    }
}