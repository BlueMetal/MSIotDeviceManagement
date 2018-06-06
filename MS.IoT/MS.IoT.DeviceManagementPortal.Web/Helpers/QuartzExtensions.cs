using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System.Collections.Specialized;

namespace MS.IoT.DeviceManagementPortal.Web.Helpers
{
    public static class QuartzExtensions
    {
        public static void UseQuartz(this IApplicationBuilder app, string intervalMinutesConfig)
        {
            int intervalMinutes;
            if (!int.TryParse(intervalMinutesConfig, out intervalMinutes))
                intervalMinutes = 10;

            app.ApplicationServices.GetService<IScheduler>();
            var scheduler = app.ApplicationServices.GetService<IScheduler>();
            scheduler.StartJob<QuartzRefreshDBJob>(intervalMinutes);
        }

        public static void AddQuartz(this IServiceCollection services)
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
            IJobFactory jobFactory = new QuartzJobFactory(services.BuildServiceProvider());
            scheduler.JobFactory = jobFactory;
            scheduler.Start().Wait();
            services.AddSingleton(scheduler);
        }

        public static void StartJob<T>(this IScheduler scheduler, int intervalMinutes) where T : IJob
        {
            IJobDetail job = JobBuilder.Create<T>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .StartNow()
                .WithSimpleSchedule
                  (s => s.WithIntervalInMinutes(intervalMinutes).RepeatForever())
                .Build();

            scheduler.ScheduleJob(job, trigger).Wait();
        }
    }
}
