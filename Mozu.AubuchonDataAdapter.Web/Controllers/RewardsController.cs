using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Mozu.AubuchonDataAdapter.Domain.Handlers;
using System.Threading.Tasks;
using Mozu.Api;

namespace Mozu.AubuchonDataAdapter.Web.Controllers
{
    [AubCors(headers: "origin, accept, x-vol-user-claims, x-vol-master-catalog, x-vol-app-claims, x-vol-currency, x-vol-site, content-type, x-vol-tenant, x-vol-locale, x-vol-catalog,x-vol-accountid, x-vol-user-password,x-vol-user-name", methods: "POST")]
    public class RewardsController : ApiController
    {
        private readonly IAubuchonAccountHandler _aubuchonAccountHandler;

        public RewardsController(IAubuchonAccountHandler aubuchonAccountHandler)
        {
            _aubuchonAccountHandler = aubuchonAccountHandler;
        }
        [HttpPost]
        [ValidateUserClaimFilter]
        public async Task<HttpResponseMessage> Points(int tenantId, int siteId, int accountId)
        {
            var apiContext = new ApiContext(tenantId, siteId);
           
            try
            {
                var pointTotal = new 
                {
                    PointTotal = await _aubuchonAccountHandler.GetCustomerRewardPoints(apiContext, accountId)  
                };
                return Request.CreateResponse(HttpStatusCode.OK, pointTotal);
            }
            catch (Exception exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, exception.Message);   
            }   
        }
    }
}
