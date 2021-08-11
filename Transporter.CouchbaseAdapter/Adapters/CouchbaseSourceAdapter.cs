using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Transporter.Core;
using Transporter.CouchbaseAdapter.ConfigOptions.Source.Interfaces;
using Transporter.CouchbaseAdapter.Services.Interfaces;

namespace Transporter.CouchbaseAdapter.Adapters
{
    public class CouchbaseSourceAdapter : ISourceAdapter, IInsertable
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

        public bool CanHandle(IJobSettings jobSetting)
        {
            var options = GetOptions(jobSetting);
            return string.Equals(options.Type, Utils.Constants.OptionsType,
                StringComparison.InvariantCultureIgnoreCase);
        }

        public void SetOptions(IJobSettings jobSettings)
        {
            _settings = GetOptions(jobSettings);
        }

        public async Task<IEnumerable<dynamic>> GetAsync()
        {
            return await _sourceService.GetSourceDataAsync(_settings);
        }

        private ICouchbaseSourceSettings GetOptions(IJobSettings jobSettings)
        {
            var jobOptionsList = _configuration.GetSection(Constants.JobListSectionKey)
                .Get<List<CouchbaseJobSettings>>();
            var options = jobOptionsList.First(x => x.Name == jobSettings.Name);
            return (ICouchbaseSourceSettings) options.Source;
        }

        public async Task SetAsync(string data)
        {
            await _sourceService.SetTargetDataAsync(_settings, data);
        }
    }
}