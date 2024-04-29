using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Transporter.Core.Adapters.Interim.Interfaces;
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
    public class TransferJob : IJob
    {
        private readonly IAdapterFactory _adapterFactory;
        private readonly ILogger<TransferJob> _logger;

        public TransferJob(IAdapterFactory adapterFactory, ILogger<TransferJob> logger)
        {
            _adapterFactory = adapterFactory;
            _logger = logger;
        }

        public TransferJobSettings TransferJobSettings { private get; set; }

        public async Task Execute(IJobExecutionContext context)
        {
            IEnumerable<dynamic> sourceData = new List<dynamic>();
            try
            {
                await Console.Error.WriteLineAsync(
                    $"Transfer Job Run => {DateTimeOffset.Now}");
                
                PingSourceAndTargetHosts();

                var interim = await _adapterFactory.GetAsync<IInterimAdapter>(TransferJobSettings);
                var source = await _adapterFactory.GetAsync<ISourceAdapter>(TransferJobSettings);
                var target = await _adapterFactory.GetAsync<ITargetAdapter>(TransferJobSettings);

                var interimData = await interim.GetAsync();
                var interimList = interimData.ToList();
                if (!interimList.Any())
                {
                    return;
                }
                
                sourceData = await source.GetAsync(interimList);
                await target.SetAsync(sourceData.ToJson());
                
                await source.DeleteAsync(interimList);
                await interim.DeleteAsync(interimList);

                await Console.Error.WriteLineAsync(
                    $"{context.FireInstanceId} : {TransferJobSettings.Name} => {DateTimeOffset.Now} => {TransferJobSettings.Source} => {context.JobDetail.Key} => Count : {sourceData.Count()} ");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                _logger.LogError(e, sourceData.ToJson());
            }
        }

        private void PingSourceAndTargetHosts()
        {
            PingHelper.PingHost(TransferJobSettings.Source.Host);
            PingHelper.PingHost(TransferJobSettings.Target.Host);
        }
    }
}