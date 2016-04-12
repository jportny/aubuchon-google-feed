using System;
using Autofac;
using Mozu.Api.Logging;
using Mozu.AubuchonDataAdapter.Domain.Utility;
using Mozu.Integration.Scheduler;

namespace Mozu.AubuchonDataAdapter.Service
{

    public class AubuchonService
    {
        private readonly ILogger _logger = LogManager.GetLogger(typeof(AubuchonService));
        private readonly IContainer _container;
        public AubuchonService() 
        {
            try
            {
                var bootstrapper = new Bootstrapper();
                _container = bootstrapper.Bootstrap().Container;
                
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }
        public void Start() 
        {
            try
            {
                var jobScheduler = _container.Resolve<IJobScheduler>();
                jobScheduler.Start();
            } 
            catch (Exception exception)
            {
                _logger.Error(exception);
                throw;
            }
        }
        public void Stop() 
        {
            try
            {
                var jobScheduelr = _container.Resolve<IJobScheduler>();
                jobScheduelr.Stop();
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
            
        }
    }
}
