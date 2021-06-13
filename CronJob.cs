using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Transporter.Core;

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
                // source ve target ulasilabilir mi 
                // herhangi birine ulasamiyorsak exception verip cikalim
                Ping pingSender = new Ping ();

                var sourcePingReply = pingSender.Send(JobSettings.Source.Host, 5);
                if (sourcePingReply?.Status != IPStatus.Success)
                {
                    throw new Exception("Source is unreachable");
                }
                
                var targetPingReply = pingSender.Send(JobSettings.Target.Host, 5);
                if (targetPingReply?.Status != IPStatus.Success)
                {
                    throw new Exception("Source is unreachable");
                }
                
                var source = await _adapterFactory.GetAsync<ISourceAdapter>(JobSettings);
                var target = await _adapterFactory.GetAsync<ITargetAdapter>(JobSettings);
                sourceData = await source.GetAsync();
                
                //set edemezsek herhangi bi durumda 
                
                // tablo bulunamazsa napicak? 
                // tablo kolonlari uyusmuyorsa napicak? 
                // targeta yazamaidigm herhangi bir durumda configten aldigim variable'a gore ya loglayip gecicez ya da source'a geri yazicaz. 
                
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