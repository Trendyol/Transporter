using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Transporter.Core;
using Transporter.MSSQLAdapter.Services;

namespace Transporter.MSSQLAdapter
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

        public void SetOptions(IJobSettings jobSettings)
        {
            _settings = GetOptions(jobSettings);
        }

        public async Task SetAsync(string data)
        {
            await _targetService.SetTargetDataAsync(_settings, data);
        }

        public virtual object Clone()
        {
            var result = MemberwiseClone() as IAdapter;
            return result;
        }

        private ISqlTargetSettings GetOptions(IJobSettings jobSettings)
        {
            var jobOptionsList = _configuration[Constants.JobListSectionKey].ToObject<ICollection<MsSqlJobSettings>>()
                .ToList();
            var options = jobOptionsList.First(x => x.Name == jobSettings.Name);
            return (ISqlTargetSettings) options.Target;
        }
    }
}