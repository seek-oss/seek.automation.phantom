using System;
using System.Diagnostics.CodeAnalysis;
using Autofac;
using Microsoft.Owin.Hosting;
using Serilog;
using SEEK.Automation.Phantom.Configuration;

namespace SEEK.Automation.Phantom
{
    [ExcludeFromCodeCoverage]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public class Service
    {
        private IDisposable _webApplication;
        private readonly ILogger _logger;
        private readonly ILifetimeScope _container;

        public Service(ILogger logger, ILifetimeScope container)
        {
            _logger = logger;
            _container = container;
        }

        public void Start()
        {
            var host = Keys.TryGetConfigValue(Keys.Host, "localhost");
            var port = Keys.TryGetConfigValue(Keys.Port, "8080");

            var endpoint = string.Format("http://{0}:{1}", host, port);
            _webApplication = WebApp.Start(endpoint, app => OwinStartup.Configuration(app, _container));
            
            _logger.Information("{ServiceName} started listening on {Endpoint}", typeof(Service).FullName, endpoint);
        }

        public void Stop()
        {
            _logger.Information("{ServiceName} stopping.", typeof(Service).FullName);

            _webApplication.Dispose();

            _container.Dispose();
        }
    }
}
