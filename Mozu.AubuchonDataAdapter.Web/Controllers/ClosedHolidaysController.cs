using System.Web.Http.Cors;
using Mozu.Api.WebToolKit.Filters;
using Mozu.AubuchonDataAdapter.Domain.Contracts;
using Mozu.AubuchonDataAdapter.Domain.Handlers;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Text.RegularExpressions;

namespace Mozu.AubuchonDataAdapter.Web.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "GET,DELETE,POST")]
    [ApiContextFilter]
    public class ClosedHolidaysController : ApiController
    {

        private readonly IClosedHolidaysHandler _closedHolidaysHandler;
        public ClosedHolidaysController(IClosedHolidaysHandler closedHolidayshandler)
        {
            _closedHolidaysHandler = closedHolidayshandler;
        }

        //[ConfigurationAuthFilter]
        [AubCors(headers: "origin, accept, x-vol-user-claims, x-vol-master-catalog, x-vol-app-claims, x-vol-currency, x-vol-site, content-type, x-vol-tenant, x-vol-locale, x-vol-catalog,x-vol-accountid", methods: "GET,POST,DELETE")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetHolidays(int tenantId = 0)
        {
            if (tenantId == 0) return Request.CreateResponse(HttpStatusCode.OK, new { Success = false, Error = "Wrong tenantId param." });
            try
            {
                var data = await _closedHolidaysHandler.GetClosedHolidaysListAsinc(tenantId).ConfigureAwait(false);
                return Request.CreateResponse(HttpStatusCode.OK, new { Success = true, data = data.OrderBy(a => a.HolidayName).ToArray() });
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }


        [ConfigurationAuthFilter]
        [HttpPost]
        public async Task<HttpResponseMessage> PostHoliday(HolidayRequest paramObj)
        {
            var tenantId = paramObj.TenantId ?? 0;
            if (tenantId == 0) return Request.CreateResponse(HttpStatusCode.OK, new { Success = false, Error = "Wrong tenantId param." });
            var result = true;
            try
            {
                var closedHoliday = new ClosedHoliday
                {
                    Id = Regex.Replace((paramObj.HolidayName + paramObj.HolidayDate), @"[^A-Za-z0-9]+", ""),
                    HolidayDate = paramObj.HolidayDate,
                    HolidayName = paramObj.HolidayName.Trim()
                };

                result = await _closedHolidaysHandler.AddClosedHolidayAsync(tenantId, closedHoliday).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { Success = result });
        }

        [ConfigurationAuthFilter]
        [HttpDelete]
        public async Task<HttpResponseMessage> DeleteHoliday(HolidayRequest paramObj)
        {
            var tenantId = paramObj.TenantId ?? 0;
            if (tenantId == 0) return Request.CreateResponse(HttpStatusCode.OK, new { Success = false, Error = "Wrong tenantId param." });

            var result = false;
            var closedHoliday = new ClosedHoliday
            {
                Id = paramObj.HolidayId
            };

            try
            {
                result = await _closedHolidaysHandler.DeleteClosedHolidayAsync(tenantId, closedHoliday).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { Success = result });
        }
    }

    public class HolidayRequest
    {
        public int? TenantId { get; set; }
        public string HolidayName { get; set; }
        public string HolidayDate { get; set; }
        public string HolidayId { get; set; }
    }
}