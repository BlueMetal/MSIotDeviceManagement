using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MS.IoT.DeviceManagementPortal.Web.Helpers
{
    public class QuartzJobFactory : IJobFactory
    {
        private readonly IServiceProvider _JobFactory;

        public QuartzJobFactory(IServiceProvider jobFactory)
        {
            _JobFactory = jobFactory;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _JobFactory.GetService(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
