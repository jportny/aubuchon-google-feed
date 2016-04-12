using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Mozu.AubuchonDataAdapter.Domain.Utility;
using System.Configuration;

namespace Mozu.AubuchonDataAdapter.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {

        public static string AllowedOrigins{ get; set; }

        protected void Application_Start()
        {
            AllowedOrigins = (string) ConfigurationManager.AppSettings["AllowedOrigins"];
            
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            new Bootstrapper().Bootstrap(GlobalConfiguration.Configuration);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
