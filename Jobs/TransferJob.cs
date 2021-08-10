using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Transporter.Core;

namespace TransporterService.Jobs
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class TransferJob : IJob
    {
        private readonly IAdapterFactory _adapterFactory;
        private readonly ILogger<TransferJob> _logger;

        public TransferJob(IAdapterFactory adapterFactory, ILogger<TransferJob> logger)
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
                PingSourceAndTargetHosts();

                var source = await _adapterFactory.GetAsync<ISourceAdapter>(JobSettings);
                var target = await _adapterFactory.GetAsync<ITargetAdapter>(JobSettings);
                sourceData = await source.GetAsync();
                
                try
                {
                    if (target is not null) await target.SetAsync(sourceData.ToJson());
                }
                catch (Exception)
                {
                    if (!JobSettings.Source.IsInsertableOnFailure || source is not IInsertable insertable) throw;
                    
                    Console.WriteLine("Could not insert data to Target, inserting back to Source");
                    await insertable.SetAsync(sourceData.ToJson());

                    throw;
                }

                await Console.Error.WriteLineAsync(
                    $"{context.FireInstanceId} : {JobSettings.Name} => {DateTimeOffset.Now} => {JobSettings.Source} => {context.JobDetail.Key} => Count : {sourceData.Count()} ");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                _logger.LogError(e, sourceData.ToJson());
            }
        }

        private void PingSourceAndTargetHosts()
        {
            var pingSender = new Ping();

            PingSourceHost(pingSender);
            PingTargetHost(pingSender);
        }

        private void PingTargetHost(Ping pingSender)
        {
            var targetPingReply = pingSender.Send(JobSettings.Target.Host, 1000);
            if (targetPingReply?.Status != IPStatus.Success)
            {
                throw new Exception("Target is unreachable");
            }
        }

        private void PingSourceHost(Ping pingSender)
        {
            var sourcePingReply = pingSender.Send(JobSettings.Source.Host, 1000);
            if (sourcePingReply?.Status != IPStatus.Success)
            {
                throw new Exception("Source is unreachable");
            }
        }
    }
}