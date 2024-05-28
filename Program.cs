﻿using System;
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
using Transporter.Core.Configs.Base.Implementations;
using Transporter.Core.Factories.Adapter.Implementations;
using Transporter.Core.Factories.Adapter.Interfaces;
using Transporter.Core.Utils;
using Transporter.CouchbaseAdapter;
using Transporter.MSSQLAdapter;
using Transporter.PostgreSqlAdapter;
using TransporterService.Daemon;
using TransporterService.Jobs;

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
                .ConfigureAppConfiguration((_, configurationBuilder) =>
                {
                    configurationBuilder.AddJsonFile("configs/secrets.json", optional: true, true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    ConfigureLogger(hostContext, services);

                    services.TransporterMsSqlAdapterRegister();
                    services.TransporterCouchbaseAdapterRegister();
                    services.TransporterPostgreSqlAdapterRegister();

                    services.Configure<QuartzOptions>(hostContext.Configuration.GetSection("Quartz"));
                    services.AddSingleton<IAdapterFactory, AdapterFactory>();
                    services.AddTransient<IDateTimeProvider, LocalDateTimeProvider>();
                    services.AddHostedService<HealthCheckDaemon>();

                    services.AddQuartz(quartz =>
                    {
                        quartz.SchedulerId = "Scheduler-Core";

                        quartz.UseMicrosoftDependencyInjectionJobFactory(options =>
                        {
                            options.AllowDefaultConstructor = false;
                        });

                        quartz.UseSimpleTypeLoader();
                        quartz.UseInMemoryStore();
                        quartz.UseDefaultThreadPool(tp => { tp.MaxConcurrency = 10; });

                        InitializePollingJobs(hostContext, quartz);
                        InitializeTransferJobs(hostContext, quartz);
                    });

                    // Quartz.Extensions.Hosting hosting
                    services.AddQuartzHostedService(options =>
                    {
                        // when shutting down we want jobs to complete gracefully
                        options.WaitForJobsToComplete = true;
                    });
                });
        }

        private static void InitializePollingJobs(HostBuilderContext hostContext,
            IServiceCollectionQuartzConfigurator quartz)
        {
            var pollingJobSettings = new List<PollingJobSettings>();
            hostContext.Configuration.GetSection(Constants.PollingJobSettings).Bind(pollingJobSettings);
            pollingJobSettings.ToList().ForEach(jobOptions =>
            {
                Console.WriteLine($"Creating Job {jobOptions.Name}");
                InitializeQuartzJobsForTemporaryTable(quartz, jobOptions);
            });
        }

        private static void InitializeTransferJobs(HostBuilderContext hostContext,
            IServiceCollectionQuartzConfigurator quartz)
        {
            var transferJobSettings = new List<TransferJobSettings>();
            hostContext.Configuration.GetSection(Constants.TransferJobSettings).Bind(transferJobSettings);
            transferJobSettings.ToList().ForEach(jobOptions => { InitializeQuartzJobs(quartz, jobOptions); });
        }

        private static void InitializeQuartzJobs(IServiceCollectionQuartzConfigurator quartzConfigurator,
            TransferJobSettings transferJobSettings)
        {
            var jobDataMap = new JobDataMap((IDictionary)new Dictionary<string, object>
                { { "transferJobSettings", transferJobSettings } });
            var jobKey = new JobKey(transferJobSettings.Name, transferJobSettings.Source.ToString());

            quartzConfigurator.AddJob<TransferJob>(j => j
                .StoreDurably()
                .WithIdentity(jobKey)
                .WithDescription(
                    $"{transferJobSettings.Name} {transferJobSettings.Source} => {transferJobSettings.Target}")
                .UsingJobData(jobDataMap)
            );

            quartzConfigurator.AddTrigger(t => t
                .WithIdentity($"{jobKey} Cron Trigger")
                .WithCronSchedule(transferJobSettings.Cron)
                .ForJob(jobKey)
                .StartNow());
        }

        private static void InitializeQuartzJobsForTemporaryTable(
            IServiceCollectionQuartzConfigurator quartzConfigurator,
            PollingJobSettings jobSettings)
        {
            var jobDataMap = new JobDataMap((IDictionary)new Dictionary<string, object>
                { { "pollingJobSettings", jobSettings } });
            var jobKey = new JobKey(jobSettings.Name, jobSettings.Source.ToString());

            quartzConfigurator.AddJob<PollingJob>(j => j
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