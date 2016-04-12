using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Logging;
using Mozu.Api;
using Mozu.Api.Events;
using Mozu.Api.Resources.Commerce.Settings;
using Mozu.AubuchonDataAdapter.Domain.Handlers;
using Mozu.AubuchonDataAdapter.Domain.Jobs;
using Mozu.Integration.Scheduler;


namespace Mozu.AubuchonDataAdapter.Domain.Events
{
    public class ApplicationEvents : IApplicationEvents
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(ApplicationEvents));
        private readonly IAccountPhoneHandler _accountPhoneHandler;
        private readonly IEventHandler _eventHandler;
        private readonly IJobScheduler _jobScheduler;
        private readonly ILogHandler _logHandler;
        private readonly IAubuchonLocationHandler _locationHandler;
        private readonly IClosedHolidaysHandler _closedHolidaysHandler;
        public ApplicationEvents(IAccountPhoneHandler accountPhoneHandler, IEventHandler eventHandler, IJobScheduler jobScheduler, ILogHandler logHandler, IAubuchonLocationHandler locationHandler, IClosedHolidaysHandler closedHolidaysHandler)
        {
            _accountPhoneHandler = accountPhoneHandler;
            _eventHandler = eventHandler;
            _jobScheduler = jobScheduler;
            _logHandler = logHandler;
            _locationHandler = locationHandler;
            _closedHolidaysHandler = closedHolidaysHandler;
        }
        public void Disabled(IApiContext apiContext, Api.Contracts.Event.Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public Task DisabledAsync(IApiContext apiContext, Api.Contracts.Event.Event eventPayLoad)
        {
            var tenantId = apiContext.TenantId;
            _jobScheduler.RemoveJob(tenantId, "ProcessEventQueueJob");
            _jobScheduler.RemoveJob(tenantId, "ProcessMissedEventsJob");
            _jobScheduler.RemoveJob(tenantId, "ImportEdgeMemberJob");
            //_jobScheduler.RemoveJob(tenantId, "DownloadMemberXmlJob");
            //_jobScheduler.RemoveJob(tenantId, "SftpUpdateJob");
            return Task.Delay(1);
        }

        public void Enabled(IApiContext apiContext, Api.Contracts.Event.Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public async Task EnabledAsync(IApiContext apiContext, Api.Contracts.Event.Event eventPayLoad)
        {
            var tenantId = apiContext.TenantId;

            log4net.GlobalContext.Properties["tenantId"] = tenantId;

            await _eventHandler.InstallSchema(tenantId);
            await _accountPhoneHandler.InstallAccountPhoneSchema(tenantId);
            await _logHandler.InstallSchema(tenantId);
            await _locationHandler.InstallLocationExtrasSchema(tenantId);
            await _closedHolidaysHandler.InstallClosedHolidaysSchemaAsync(tenantId);

            _jobScheduler.RemoveJob(tenantId, "ProcessEventQueueJob");
            _jobScheduler.RemoveJob(tenantId, "ProcessMissedEventsJob");
            _jobScheduler.RemoveJob(tenantId, "ImportEdgeMemberJob");
            //_jobScheduler.RemoveJob(tenantId, "DownloadMemberXmlJob");
            //_jobScheduler.RemoveJob(tenantId, "SftpUpdateJob");


            _jobScheduler.AddJob(tenantId, "ProcessEventQueueJob", typeof(ProcessEventQueueJob), null, "0 0/2 * * * ?");

            _jobScheduler.AddJob(tenantId, "ProcessMissedEventsJob", typeof(ProcessMissedEventsJob), null, "0 0/5 * * * ?");

            var jobParam = new Dictionary<string, object> { { "ProcessedDownloadedFiles", false } , {"siteId", apiContext.SiteId} };
            _jobScheduler.AddJob(tenantId, "ImportEdgeMemberJob", typeof(ImportEdgeMemberJob), jobParam, "0 0/3 * * * ?",true);


            _log.Info("Job Added!");
        }

        public void Installed(IApiContext apiContext, Api.Contracts.Event.Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public async Task InstalledAsync(IApiContext apiContext, Api.Contracts.Event.Event eventPayLoad)
        {
            await ProcessApplicationAsync(apiContext);
        }

        public void Uninstalled(IApiContext apiContext, Api.Contracts.Event.Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public Task UninstalledAsync(IApiContext apiContext, Api.Contracts.Event.Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public void Upgraded(IApiContext apiContext, Api.Contracts.Event.Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public async Task UpgradedAsync(IApiContext apiContext, Api.Contracts.Event.Event eventPayLoad)
        {
            await ProcessApplicationAsync(apiContext);
        }

        private async Task ProcessApplicationAsync(IApiContext apiContext)
        {
            var applicationResource = new ApplicationResource(apiContext);

            var application = await applicationResource.ThirdPartyGetApplicationAsync();
            application.Initialized = true;

            await applicationResource.ThirdPartyUpdateApplicationAsync(application);

        }
    }
}
