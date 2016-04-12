using Mozu.Api;
using Mozu.Api.Resources.Platform;
using Mozu.Api.WebToolKit.Filters;
using System;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Mozu.AubuchonDataAdapter.Web.Controllers
{
    public class HomeController : Controller
    {

        [ConfigurationAuthFilter]
        [HttpPost]
        public async Task<ActionResult> Index(int tenantId)
        {
          
            ViewBag.Title = "Aubuchon";
            try
            {
                var tenantResource = new TenantResource();

                var apiContext = new ApiContext(tenantId);


                ////Call to make sure app has access to tenant
                await tenantResource.GetTenantAsync(apiContext.TenantId);

                string cookieToken;
                string formToken;

                AntiForgery.GetTokens(null, out cookieToken, out formToken);
                ViewBag.cookieToken = cookieToken;
                ViewBag.formToken = formToken;
                ViewBag.tenantId = apiContext.TenantId;
                ViewBag.siteId = apiContext.SiteId;

            }
            catch (ApiException ex)
            {

                ModelState.AddModelError("Error", ex.Message);
            }

            return View();
        }

        [HttpGet]
        public ActionResult Test()
        {
            return Content(DateTime.Now.ToLongTimeString());
        }

      
    }
}