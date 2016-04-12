using System.Web.Http;

namespace Mozu.AubuchonDataAdapter.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.EnableCors();
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute("ActionApi", "api/{controller}/{action}/{tenantId}", new { tenantId = RouteParameter.Optional }, new { action = @"[a-zA-Z]+" });

            config.Routes.MapHttpRoute("Version", "api/version", new { controller = "Version", action = "Get" });
            
            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{tenantId}", new { tenantId = RouteParameter.Optional, action = "Default" }, new { id = @"\d*" });

        }
    }
}