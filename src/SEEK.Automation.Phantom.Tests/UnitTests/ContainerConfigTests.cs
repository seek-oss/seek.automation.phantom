using Autofac;
using SEEK.Automation.Phantom.Controllers;
using Xunit;
using Serilog;
using SEEK.Automation.Phantom.Domain;
using SEEK.Automation.Phantom.Support;

namespace SEEK.Automation.Phantom.Tests.UnitTests
{
    public class ContainerConfigTests
    {
        [Fact]
        public void BuildAutofacContainer_Should_Register_HealthController()
        {
            var container = ContainerConfig.BuildContainer();

            Assert.True(container.IsRegistered<HealthController>());
        }

        [Fact]
        public void BuildAutofacContainer_Should_Register_VersionController()
        {
            var container = ContainerConfig.BuildContainer();
            
            Assert.True(container.IsRegistered<VersionController>());
        }

        [Fact]
        public void BuildAutofacContainer_Should_Register_LoggingOptions()
        {
            var container = ContainerConfig.BuildContainer();

            Assert.True(container.IsRegistered<ILogger>());
        }

        [Fact]
        public void BuildAutofacContainer_Should_Register_WebServer()
        {
            var container = ContainerConfig.BuildContainer();

            Assert.True(container.IsRegistered<IWebServer>());
        }
        
        [Fact]
        public void BuildAutofacContainer_Should_Register_WebServerFactory()
        {
            var container = ContainerConfig.BuildContainer();

            Assert.True(container.IsRegistered<IWebServerFactory>());
        }

        [Fact]
        public void BuildAutofacContainer_Should_Register_Broker()
        {
            var container = ContainerConfig.BuildContainer();

            Assert.True(container.IsRegistered<IBroker>());
        }

        [Fact]
        public void BuildAutofacContainer_Should_Register_Simulation()
        {
            var container = ContainerConfig.BuildContainer();

            Assert.True(container.IsRegistered<ISimulation>());
        }

        [Fact]
        public void BuildAutofacContainer_Should_Register_SimulationFactory()
        {
            var container = ContainerConfig.BuildContainer();

            Assert.True(container.IsRegistered<ISimulationFactory>());
        }

        [Fact]
        public void BuildAutofacContainer_Should_Register_Generic()
        {
            var container = ContainerConfig.BuildContainer();

            Assert.True(container.IsRegistered<IGeneric>());
        }

        [Fact]
        public void BuildAutofacContainer_Should_Register_CachingFactory()
        {
            var container = ContainerConfig.BuildContainer();

            Assert.True(container.IsRegistered<ICachingProvider>());
        }

        [Fact]
        public void BuildAutofacContainer_Should_Register_WindowsFirewall()
        {
            var container = ContainerConfig.BuildContainer();

            Assert.True(container.IsRegistered<IWindowsFirewallHelper>());
        }

        [Fact]
        public void BuildAutofacContainer_Should_Register_Schedulers()
        {
            var container = ContainerConfig.BuildContainer();

            Assert.True(container.IsRegistered<IStartable>());
        }
    }
}
