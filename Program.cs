using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Quartz;
using Transporter.Core;
using Transporter.CouchbaseAdapter;
using Transporter.MSSQLAdapter;
using TransporterService.Jobs;
using ServiceRegisterer = Transporter.MSSQLDeleteAdapter.ServiceRegisterer;

namespace TransporterService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    ConfigureLogger(hostContext, services);

                    services.TransporterMsSqlAdapterRegister();
                    services.TransporterCouchbaseAdapterRegister();
                    ServiceRegisterer.TransporterMsSqlDeleteAdapterRegister(services);

                    services.Configure<QuartzOptions>(hostContext.Configuration.GetSection("Quartz"));
                    services.AddSingleton<IAdapterFactory, AdapterFactory>();

                    services.AddQuartz(quartz =>
                    {
                        // handy when part of cluster or you want to otherwise identify multiple schedulers
                        quartz.SchedulerId = "Scheduler-Core";

                        // we take this from appsettings.json, just show it's possible
                        // q.SchedulerName = "Quartz ASP.NET Core Sample Scheduler";
                        // we could leave DI configuration intact and then jobs need to have public no-arg constructor
                        // the MS DI is expected to produce transient job instances 

                        quartz.UseMicrosoftDependencyInjectionJobFactory(options =>
                        {
                            // if we don't have the job in DI, allow fallback to configure via default constructor
                            options.AllowDefaultConstructor = false;
                        });

                        quartz.UseSimpleTypeLoader();
                        quartz.UseInMemoryStore();
                        quartz.UseDefaultThreadPool(tp => { tp.MaxConcurrency = 10; });
                        
                        // var jobOptionsList = hostContext.Configuration.GetSection(Constants.JobListSectionKey)
                        //     .Get<List<JobSettings>>();
                        var temporaryJobOptionsList = hostContext.Configuration
                            .GetSection(Constants.TemporaryJobListSectionKey)
                            .Get<List<TemporaryTableOptions.TemporaryTableJobSettings>>();

                        // Console.Error.Write("jobOptionsList : " + jobOptionsList.ToJson());
                        Console.Error.Write("hostContext : " +
                                            hostContext.Configuration[Constants.JobListSectionKey]);

                        // jobOptionsList.ToList().ForEach(jobOptions => { InitializeQuartzJobs(quartz, jobOptions); });
                        temporaryJobOptionsList.ToList().ForEach(jobOptions =>
                        {
                            InitializeQuartzJobsForTemporaryTable(quartz, jobOptions);
                        });
                    });

                    // Quartz.Extensions.Hosting hosting
                    services.AddQuartzHostedService(options =>
                    {
                        // when shutting down we want jobs to complete gracefully
                        options.WaitForJobsToComplete = true;
                    });
                });
        }

        private static void InitializeQuartzJobs(IServiceCollectionQuartzConfigurator quartzConfigurator,
            JobSettings jobSettings)
        {
            var jobDataMap = new JobDataMap((IDictionary)new Dictionary<string, object>
                { { "jobSettings", jobSettings } });
            var jobKey = new JobKey(jobSettings.Name, jobSettings.Source.ToString());

            quartzConfigurator.AddJob<TransferJob>(j => j
                .StoreDurably()
                .WithIdentity(jobKey)
                .WithDescription($"{jobSettings.Name} {jobSettings.Source} => {jobSettings.Target}")
                .UsingJobData(jobDataMap)
            );

            quartzConfigurator.AddTrigger(t => t
                .WithIdentity($"{jobKey} Cron Trigger")
                .WithCronSchedule(jobSettings.Cron)
                .ForJob(jobKey)
                .StartNow());
        }

        private static void InitializeQuartzJobsForTemporaryTable(
            IServiceCollectionQuartzConfigurator quartzConfigurator,
            TemporaryTableOptions.TemporaryTableJobSettings jobSettings)
        {
            var jobDataMap = new JobDataMap((IDictionary)new Dictionary<string, object>
                { { "jobSettingsForTemporaryTable", jobSettings } });
            var jobKey = new JobKey(jobSettings.Name, jobSettings.Source.ToString());

            quartzConfigurator.AddJob<TransferTemporaryJob>(j => j
                .StoreDurably()
                .WithIdentity(jobKey)
                .WithDescription($"{jobSettings.Name} {jobSettings.Source} => {jobSettings.Target}")
                .UsingJobData(jobDataMap)
            );

            quartzConfigurator.AddTrigger(t => t
                .WithIdentity($"{jobKey} Cron Trigger")
                .WithCronSchedule(jobSettings.Cron)
                .ForJob(jobKey)
                .StartNow());
        }

        private static void ConfigureLogger(HostBuilderContext hostContext, IServiceCollection services)
        {
            var nlogConfig = hostContext.Configuration["NLogConfig"];
            Console.Error.Write(nlogConfig);
            if (!string.IsNullOrWhiteSpace(nlogConfig))
            {
                File.WriteAllText("nlog.config", nlogConfig, Encoding.UTF8);
                services.AddLogging(builder =>
                {
                    builder.ClearProviders();
                    builder.AddNLog();
                });
            }
        }
    }
}