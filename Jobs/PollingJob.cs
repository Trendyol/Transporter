using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Transporter.Core.Adapters.Source.Interfaces;
using Transporter.Core.Adapters.Target.Interfaces;
using Transporter.Core.Configs.Base.Implementations;
using Transporter.Core.Factories.Adapter.Interfaces;
using Transporter.Core.Utils;
using TransporterService.Helpers;

namespace TransporterService.Jobs
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class PollingJob(IAdapterFactory adapterFactory, ILogger<PollingJob> logger) : IJob
    {
        private readonly IAdapterFactory _adapterFactory = adapterFactory;
        private readonly ILogger<PollingJob> _logger = logger;

        public PollingJobSettings PollingJobSettings { private get; set; }

        public async Task Execute(IJobExecutionContext context)
        {
            IEnumerable<dynamic> sourceData = new List<dynamic>();
            try
            {
                PingSourceAndTargetHosts();

                var source = await _adapterFactory.GetAsync<ISourceAdapter>(PollingJobSettings);
                var target = await _adapterFactory.GetAsync<ITargetAdapter>(PollingJobSettings);

                sourceData = await source.GetIdsAsync();

                if (target is not null)
                {
                    await target.SetInterimTableAsync(sourceData.ToJson(), source.GetDataSourceName());
                }

                await Console.Error.WriteLineAsync(
                    $"{context.FireInstanceId} : {PollingJobSettings.Name} => {DateTimeOffset.Now} => {PollingJobSettings.Source} => {context.JobDetail.Key}");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                _logger.LogError(e, sourceData.ToJson());
            }
        }

        private void PingSourceAndTargetHosts()
        {
            PingHelper.PingHost(PollingJobSettings.Source.Host);
            PingHelper.PingHost(PollingJobSettings.Target.Host);
        }
    }
}