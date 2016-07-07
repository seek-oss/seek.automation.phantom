using System.Diagnostics.CodeAnalysis;
using System.Web.Http;

namespace SEEK.Automation.Phantom
{
    [ExcludeFromCodeCoverage]
    public class RouteConfig
    {
        public static void RegisterRoutes(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultRoute",
                routeTemplate: "v1/{controller}/{id}",
                defaults: new {id = RouteParameter.Optional}
                );
        }
    }
}
