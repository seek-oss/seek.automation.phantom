using System.Diagnostics.CodeAnalysis;
using System.Net;
using Autofac;
using Serilog;
using Topshelf;
using Topshelf.Autofac;

namespace SEEK.Automation.Phantom
{
    [ExcludeFromCodeCoverage]
    class TopSelfServiceHost
    {
        static int Main()
        {
            // Remove the limit on concurrent network connections
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;

            // Set up the IoC container
            var container = ContainerConfig.BuildContainer();

            var logger = container.Resolve<ILogger>().ForContext<TopSelfServiceHost>();
            Log.Logger = logger;

            var exitCode = HostFactory.Run(host =>
            {
                host.Service<Service>(service =>
                {
                    service.ConstructUsingAutofacContainer();
                    service.WhenStarted(a => a.Start());
                    service.WhenStopped(a => a.Stop());
                });

                host.UseAutofacContainer(container);
                host.RunAsNetworkService();
            });

            return (int)exitCode;
        }
    }
}
