using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Mozu.Api;

namespace Mozu.AubuchonDataAdapter.Web
{
    public class ApiContextFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var request = actionContext.Request;


            var tenantId = request.Headers.Contains(Headers.X_VOL_TENANT) ? Convert.ToInt32(request.Headers.GetValues(Headers.X_VOL_TENANT).FirstOrDefault()) : 0;

            var siteId = request.Headers.Contains(Headers.X_VOL_SITE) ? Convert.ToInt32(request.Headers.GetValues(Headers.X_VOL_SITE).FirstOrDefault()) : 0;

            var accountId = request.Headers.Contains("x-vol-accountid") ? Convert.ToInt32(request.Headers.GetValues("x-vol-accountid").FirstOrDefault()) : 0;


            if (tenantId == 0)
            {
                tenantId = actionContext.ActionArguments.ContainsKey("tenantId")
                    ? Convert.ToInt32(actionContext.ActionArguments["tenantId"])
                    : 0;

                siteId = actionContext.ActionArguments.ContainsKey("siteId")
                    ? Convert.ToInt32(actionContext.ActionArguments["siteId"])
                    : 0;
            }

            if (tenantId == 0)
            {
                actionContext.Response = request.CreateErrorResponse(HttpStatusCode.BadRequest, "Request is missing required parameter tenantId");
            }
            var apiContext = new ApiContext(tenantId);

            if (siteId > 0)
            {
                apiContext.SiteId = siteId;
            }
            if (accountId > 0)
            {
                actionContext.Request.Properties.Add("AccountId", accountId);
            }
            actionContext.Request.Properties.Add("ApiContext", apiContext);
            base.OnActionExecuting(actionContext);
        }

    }
}