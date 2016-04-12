using System.Web.Mvc;
using System.Web.Routing;
using Mozu.Api.WebToolKit.Events;


namespace Mozu.AubuchonDataAdapter.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            RouteTable.Routes.Add(new Route(url: "{resource}.event/{*pathInfo}", routeHandler: DependencyResolver.Current.GetService<EventRouteHandler>()));


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
