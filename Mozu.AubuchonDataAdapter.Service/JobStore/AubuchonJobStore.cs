using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz.Impl.AdoJobStore;
using System.Reflection;
namespace Mozu.AubuchonDataAdapter.Service.JobStore
{
    public class AubuchonJobStore : JobStoreTX
    {
        private static readonly PropertyInfo[] _baseTypeProperties = typeof(AubuchonJobStore).BaseType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic);

        public override IList<Quartz.Spi.IOperableTrigger> AcquireNextTriggers(DateTimeOffset noLaterThan, int maxCount, TimeSpan timeWindow)
        {
            return base.AcquireNextTriggers(noLaterThan, maxCount, timeWindow);
        }
    }
}
