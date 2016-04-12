using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using Common.Logging;
using Mozu.Api;
using Mozu.Api.Clients.Commerce.Customer;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;

namespace Mozu.AubuchonDataAdapter.Web
{
    public class ValidateUserClaimFilter : ActionFilterAttribute
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(ValidateUserClaimFilter));
        public async override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
//#if RELEASE
            var request = actionContext.Request;


            var tenantId = request.Headers.Contains(Headers.X_VOL_TENANT) ? Convert.ToInt32(request.Headers.GetValues(Headers.X_VOL_TENANT).FirstOrDefault()) : 0;

            var siteId = request.Headers.Contains(Headers.X_VOL_SITE) ? Convert.ToInt32(request.Headers.GetValues(Headers.X_VOL_SITE).FirstOrDefault()) : 0;

            var accountId = request.Headers.Contains("x-vol-accountid") ? Convert.ToInt32(request.Headers.GetValues("x-vol-accountid").FirstOrDefault()) : 0;



            if (tenantId ==0 && siteId ==0 && accountId ==0)
            {
                accountId= actionContext.ActionArguments.ContainsKey("accountId")
                    ? Convert.ToInt32(actionContext.ActionArguments["accountId"])
                    : 0;
                tenantId = actionContext.ActionArguments.ContainsKey("tenantId")
                    ? Convert.ToInt32(actionContext.ActionArguments["tenantId"])
                    : 0;

                siteId = actionContext.ActionArguments.ContainsKey("siteId")
                    ? Convert.ToInt32(actionContext.ActionArguments["siteId"])
                    : 0;
            }

            if(accountId == 0 || tenantId == 0|| siteId == 0)
            {
                actionContext.Response = request.CreateErrorResponse(HttpStatusCode.BadRequest, "Request is missing required parameters accountId or tenantId or siteId");
            }

           
            var apiContext = new ApiContext(tenantId,siteId);
            if (!request.Headers.Contains(Headers.X_VOL_USER_CLAIMS))
            {
                actionContext.Response = request.CreateErrorResponse(HttpStatusCode.BadRequest, "Request header must contain " + Headers.X_VOL_USER_CLAIMS);
            }

            try
            {
                if(request.Headers.Contains("x-vol-user-password") && request.Headers.Contains("x-vol-user-name")){
                var userPassword = request.Headers.GetValues("x-vol-user-password");
                var userName = request.Headers.GetValues("x-vol-user-name");
                 var authProfile =  Api.Security.CustomerAuthenticator.Authenticate(new Api.Contracts.Customer.CustomerUserAuthInfo()
                    {
                      Password = userPassword.FirstOrDefault(), 
                      Username = userName.FirstOrDefault()
                    }, tenantId, siteId);
                 if (authProfile.CustomerAccount != null && authProfile.CustomerAccount.Id != accountId)
                     throw new Exception("Invalid parameters");
                }
                else
                {
                    var userClaims = request.Headers.GetValues(Headers.X_VOL_USER_CLAIMS);

                    // just use this request to validate the user claims
                    var accountClient = CustomerAccountClient.GetAccountClient(accountId, "id");
                    accountClient.AddHeader(Headers.X_VOL_USER_CLAIMS, userClaims.First());
                    await accountClient.WithContext(apiContext).ExecuteAsync().ConfigureAwait(false);
                }

            }
            catch (Exception ex)
            {
                _log.Error(ex.Message,ex);
                actionContext.Response = request.CreateErrorResponse(HttpStatusCode.Forbidden, "Account not found or request has invalid " + Headers.X_VOL_USER_CLAIMS);
            }
//#endif
        }

    }
}