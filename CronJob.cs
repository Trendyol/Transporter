using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;

namespace TransporterService
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class CronJob : IJob
    {
        private readonly IAdapterFactory _adapterFactory;
        private readonly ILogger<CronJob> _logger;

        public CronJob(IAdapterFactory adapterFactory, ILogger<CronJob> logger)
        {
            _adapterFactory = adapterFactory;
            _logger = logger;
        }

        public JobSettings JobSettings { private get; set; }

        public async Task Execute(IJobExecutionContext context)
        {
            IEnumerable<dynamic> sourceData = new List<dynamic>();
            try
            {
                var source = await _adapterFactory.GetAsync<ISourceAdapter>(JobSettings);
                var target = await _adapterFactory.GetAsync<ITargetAdapter>(JobSettings);
                sourceData = await source.GetAsync();
                if (target is not null) await target.SetAsync(sourceData.ToJson());

                await Console.Error.WriteLineAsync(
                    $"{context.FireInstanceId} : {JobSettings.Name} => {DateTimeOffset.Now} => {JobSettings.Source} => {context.JobDetail.Key} => Count : {sourceData.Count()} ");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                _logger.LogError(e, sourceData.ToJson());
            }
        }
    }
}