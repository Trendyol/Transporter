using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Transporter.Core;
using Transporter.MSSQLAdapter.Services;

namespace Transporter.MSSQLAdapter
{
    public class MsSqlSourceAdapter : ISourceAdapter
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

        public void SetOptions(IJobSettings jobSettings)
        {
            _settings = GetOptions(jobSettings);
        }

        public async Task<IEnumerable<dynamic>> GetAsync()
        {
            return await _sourceService.GetSourceDataAsync(_settings);
        }

        public virtual object Clone()
        {
            var result = MemberwiseClone() as IAdapter;
            return result;
        }

        private ISqlSourceSettings GetOptions(IJobSettings jobSettings)
        {
            var jobOptionsList = _configuration[Constants.JobListSectionKey].ToObject<ICollection<MsSqlJobSettings>>()
                .ToList();
            var options = jobOptionsList.First(x => x.Name == jobSettings.Name);
            return (ISqlSourceSettings) options.Source;
        }
    }
}