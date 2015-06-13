using System.Web.Mvc;
using System.Web.Routing;

namespace Quilt4.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Details",
                url: "{controller}/{action}/{id}/{application}/{version}/{issueType}",
                defaults: new
                {
                    application = UrlParameter.Optional,
                    version = UrlParameter.Optional,
                    issueType = UrlParameter.Optional,
                }
            );
        }
    }
}
