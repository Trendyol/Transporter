using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Transporter.Core;
using Transporter.Core.Adapters.Source.Interfaces;
using Transporter.Core.Adapters.Target.Interfaces;
using Transporter.Core.Factories.Adapter.Interfaces;
using Transporter.Core.Utils;

namespace TransporterService.Jobs
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class TransferTemporaryJob : IJob
    {
        private readonly IAdapterFactory _adapterFactory;
        private readonly ILogger<TransferJob> _logger;

        public TransferTemporaryJob(IAdapterFactory adapterFactory, ILogger<TransferJob> logger)
        {
            _adapterFactory = adapterFactory;
            _logger = logger;
        }

        public TemporaryTableOptions.TemporaryTableJobSettings JobSettingsForTemporaryTable { private get; set; }

        public async Task Execute(IJobExecutionContext context)
        {
            IEnumerable<dynamic> sourceData = new List<dynamic>();
            try
            {
                
                PingSourceAndTargetHosts();

                var source = await _adapterFactory.GetAsync<ISourceAdapter>(JobSettingsForTemporaryTable);
                var target = await _adapterFactory.GetAsync<ITargetAdapter>(JobSettingsForTemporaryTable);
                sourceData = await source.GetIdDataAsync();
                
                try
                {
                     if (target is not null) await target.SetTemporaryTableAsync(sourceData.ToJson(), source.GetDataSourceName());
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e} Could not insert data to Target, inserting back to Source");
                    throw;
                }

                await Console.Error.WriteLineAsync(
                    $"{context.FireInstanceId} : {JobSettingsForTemporaryTable.Name} => {DateTimeOffset.Now} => {JobSettingsForTemporaryTable.Source} => {context.JobDetail.Key}");
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
            var targetPingReply = pingSender.Send(JobSettingsForTemporaryTable.Target.Host, 1000);
            if (targetPingReply?.Status != IPStatus.Success)
            {
                throw new Exception("Target is unreachable");
            }
        }

        private void PingSourceHost(Ping pingSender)
        {
            var sourcePingReply = pingSender.Send(JobSettingsForTemporaryTable.Source.Host, 1000);
            if (sourcePingReply?.Status != IPStatus.Success)
            {
                throw new Exception("Source is unreachable");
            }
        }
    }
}