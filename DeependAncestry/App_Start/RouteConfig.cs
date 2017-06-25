using System.Web.Mvc;
using System.Web.Routing;

namespace DeependAncestry
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Ancestry", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Advanced",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Advanced", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
