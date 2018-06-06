using Microsoft.Extensions.Logging;
using MS.IoT.Domain.Interface;
using Quartz;
using System;
using System.Threading.Tasks;

namespace MS.IoT.DeviceManagementPortal.Web.Helpers
{
    public class QuartzRefreshDBJob : IJob
    {
        private readonly ILogger<QuartzRefreshDBJob> _LoggerJob;
        private IDeviceDBService _DeviceDBService = null;

        public QuartzRefreshDBJob(IDeviceDBService deviceDBService, ILogger<QuartzRefreshDBJob> logger)
        {
            _DeviceDBService = deviceDBService;
            _LoggerJob = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await _DeviceDBService.RefreshDeviceDBCache();
            }
            catch (Exception e)
            {
                _LoggerJob.LogError(e.Message);
            }
        }
    }
}
