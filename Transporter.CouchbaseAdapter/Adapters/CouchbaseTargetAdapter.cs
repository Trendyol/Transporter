using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Transporter.Core;
using Transporter.CouchbaseAdapter.ConfigOptions.Target.Interfaces;
using Transporter.CouchbaseAdapter.Services.Interfaces;

namespace Transporter.CouchbaseAdapter.Adapters
{
    public class CouchbaseTargetAdapter : ITargetAdapter
    {
        private readonly ITargetService _targetService;
        private readonly IConfiguration _configuration;
        private ICouchbaseTargetSettings _settings;

        public CouchbaseTargetAdapter(ITargetService targetService, IConfiguration configuration)
        {
            _targetService = targetService;
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

        public async Task SetAsync(string data)
        {
            await _targetService.SetTargetDataAsync(_settings, data);
        }

        private ICouchbaseTargetSettings GetOptions(IJobSettings jobSettings)
        {
            var jobOptionsList = _configuration.GetSection(Constants.JobListSectionKey)
                .Get<List<CouchbaseJobSettings>>();
            var options = jobOptionsList.First(x => x.Name == jobSettings.Name);
            return (ICouchbaseTargetSettings) options.Target;
        }
    }
}