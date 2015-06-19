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
                name: "application",
                url: "{controller}/{action}/{id}/{application}",
                defaults: new
                {
                    application = UrlParameter.Optional,
                },
                namespaces: new[] { "Quilt4.Web.Controllers" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new
                {
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional
                },
                namespaces: new[] { "Quilt4.Web.Controllers" }
            );

            routes.MapRoute(
                name: "version",
                url: "{controller}/{action}/{id}/{application}/{version}",
                defaults: new
                {
                    application = UrlParameter.Optional,
                    version = UrlParameter.Optional,
                },
                namespaces: new[] { "Quilt4.Web.Controllers" }
            );

            routes.MapRoute(
                name: "issueType",
                url: "{controller}/{action}/{id}/{application}/{version}/{issueType}",
                defaults: new
                {
                    application = UrlParameter.Optional,
                    version = UrlParameter.Optional,
                    issueType = UrlParameter.Optional,
                },
                namespaces: new[] { "Quilt4.Web.Controllers" }
            );
        }
    }
}
