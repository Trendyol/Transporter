using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using NLog.Extensions.Logging;
using Quartz;

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
                    
                    services.Configure<QuartzOptions>(hostContext.Configuration.GetSection("Quartz"));

                    services.AddQuartz(q =>
                    {
                        // handy when part of cluster or you want to otherwise identify multiple schedulers
                        q.SchedulerId = "Scheduler-Core";

                        // we take this from appsettings.json, just show it's possible
                        // q.SchedulerName = "Quartz ASP.NET Core Sample Scheduler";
                        // we could leave DI configuration intact and then jobs need to have public no-arg constructor
                        // the MS DI is expected to produce transient job instances 
                        
                        q.UseMicrosoftDependencyInjectionJobFactory(options =>
                        {
                            // if we don't have the job in DI, allow fallback to configure via default constructor
                            options.AllowDefaultConstructor = false;
                        });
                        
                        q.UseSimpleTypeLoader();
                        q.UseInMemoryStore();
                        q.UseDefaultThreadPool(tp => { tp.MaxConcurrency = 10; });


                        var jobOptionsList = hostContext.Configuration[Constants.JobListSectionKey]
                            .ToObject<ICollection<JobSettings>>();

                        Console.Error.Write("jobOptionsList : " + jobOptionsList.ToJson());
                        Console.Error.Write("hostContext : " +
                                            hostContext.Configuration[Constants.JobListSectionKey]);
                        
                        jobOptionsList.ToList().ForEach(jobOptions => { InitializeQuartzJobs(q, jobOptions); });
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
            var jobDataMap = new JobDataMap((IDictionary) new Dictionary<string, object>
                {{"jobSettings", jobSettings}});
            var jobKey = new JobKey(jobSettings.Name, jobSettings.Source.ToString());

            quartzConfigurator.AddJob<CronJob>(j => j
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