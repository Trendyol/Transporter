using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic.CompilerServices;
using Transporter.Core;
using Transporter.MSSQLAdapter.Services;

namespace Transporter.MSSQLAdapter
{
    public class MsSqlSourceAdapter : ISourceAdapter, IInsertable
    {
        private readonly IConfiguration _configuration;
        private readonly ISourceService _sourceService;
        private ISqlSourceSettings _settings;


        public MsSqlSourceAdapter(ISourceService sourceService, IConfiguration configuration)
        {
            _sourceService = sourceService;
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
            await _sourceService.SetSourceDataAsync(_settings, data);
        }

        public async Task<IEnumerable<dynamic>> GetAsync(IEnumerable<dynamic> ids)
        {
            return await _sourceService.GetSourceDataAsync(_settings, ids);
        }
        
        public async Task DeleteAsync(IEnumerable<dynamic> ids)
        {
            await _sourceService.DeleteDataByListOfIdsAsync(_settings, ids);
        }
        
        public string GetDataSourceName()
        {
            return $"{_settings.Options.Schema}.{_settings.Options.Table}";
        }
        
        public async Task<IEnumerable<dynamic>> GetIdDataAsync()
        {
            return await _sourceService.GetIdDataAsync(_settings);
        }

        public virtual object Clone()
        {
            var result = MemberwiseClone() as IAdapter;
            return result;
        }

        private ISqlSourceSettings GetOptions(IJobSettings jobSettings)
        {
            var jobOptionsList = _configuration.GetSection(Constants.JobListSectionKey).Get<List<MsSqlJobSettings>>();
            var options = jobOptionsList.First(x => x.Name == jobSettings.Name);
            return (ISqlSourceSettings) options.Source;
        }
        
        private ISqlSourceSettings GetOptions(TemporaryTableOptions.ITemporaryTableJobSettings jobSettings)
        {
            var jobOptionsList = _configuration.GetSection(Constants.TemporaryJobListSectionKey).Get<List<MsSqlJobSettings>>();
            var options = jobOptionsList.First(x => x.Name == jobSettings.Name);
            return (ISqlSourceSettings) options.Source;
        }
        
        private string GetTypeBySettings(TemporaryTableOptions.ITemporaryTableJobSettings jobSettings)
        {
            var jobOptionsList = _configuration.GetSection(Constants.TemporaryJobListSectionKey)
                .Get<List<MsSqlJobSettings>>();
            var options = jobOptionsList.First(x => x.Name == jobSettings.Name);
            return options.Source?.Type;
        }
    }
}