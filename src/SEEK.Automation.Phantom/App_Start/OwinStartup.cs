using System.Diagnostics.CodeAnalysis;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Autofac;
using Owin;
using Serilog;

namespace SEEK.Automation.Phantom
{
    [ExcludeFromCodeCoverage]
    public static class OwinStartup
    {
        public static void Configuration(IAppBuilder application, ILifetimeScope container)
        {
            var config = new HttpConfiguration();

            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            config.Services.Add(typeof(IExceptionLogger), new SerilogExceptionLogger(container.Resolve<ILogger>().ForContext<SerilogExceptionLogger>()));
            
            application.UseAutofacMiddleware(container);
            application.UseAutofacWebApi(config);

            RouteConfig.RegisterRoutes(config);

            FormatterConfig.RegisterFormatters(config);

            application.UseWebApi(config);
        }
    }
}
