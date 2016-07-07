using System.Reflection;
using Autofac;
using Autofac.Integration.WebApi;
using AutofacSerilogIntegration;
using Serilog;
using SEEK.Automation.Phantom.Domain;
using SEEK.Automation.Phantom.Support;

namespace SEEK.Automation.Phantom
{
    public class ContainerConfig
    {
        public static ILifetimeScope BuildContainer()
        {
            return CreateContainer().Build();
        }

        public static ContainerBuilder CreateContainerWithoutDependency()
        {
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyModules(Assembly.GetExecutingAssembly());
            builder.RegisterApiControllers(typeof(OwinStartup).Assembly);

            builder.RegisterType<Service>().AsSelf();

            builder.RegisterType<WebServer>().As<IWebServer>().InstancePerRequest();
            builder.RegisterType<WebServerFactory>().As<IWebServerFactory>().InstancePerRequest();
            builder.RegisterType<Broker>().As<IBroker>().InstancePerRequest();
            builder.RegisterType<Simulation>().As<ISimulation>();
            builder.RegisterType<SimulationFactory>().As<ISimulationFactory>();
            builder.RegisterType<Generic>().As<IGeneric>().InstancePerRequest();
            builder.RegisterType<CachingProvider>().As<ICachingProvider>().InstancePerRequest();

            builder.RegisterType<WindowsFirewallHelper>().As<IWindowsFirewallHelper>();
            builder.RegisterType<Launcher>().As<IStartable>().SingleInstance();

            return builder;
        }

        public static ContainerBuilder CreateContainer()
        {
            var builder = CreateContainerWithoutDependency();

            RegisterLogging(builder);

            return builder;
        }

        public static void RegisterLogging(ContainerBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.WithProperty("SourceContext", null)
                .WriteTo.LiterateConsole(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message} ({SourceContext:l}){NewLine}{Exception}")
                .WriteTo.RollingFile("Log-{Date}.txt")
                .CreateLogger();

            builder.RegisterLogger(autowireProperties: true);
        }
    }
}
