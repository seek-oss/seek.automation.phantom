using System;
using System.IO;
using System.Net;
using System.Net.Http;
using Autofac;
using FluentAssertions;
using Microsoft.Owin.Testing;
using NSubstitute;
using Serilog;
using SEEK.Automation.Phantom.Domain;
using Xunit;

namespace SEEK.Automation.Phantom.Tests.UnitTests
{
    public class RequestControllerTests
    {
        public TestServer Phantom;
        private readonly ContainerBuilder _containerBuilder;

        public RequestControllerTests()
        {
            var logger = Substitute.For<ILogger>();

            _containerBuilder = ContainerConfig.CreateContainerWithoutDependency();
            _containerBuilder.RegisterInstance(logger);
        }

        [Fact]
        public async void Validate_Clear_Cache()
        {
            var cachingProvider = Substitute.For<ICachingProvider>();
            _containerBuilder.RegisterInstance(cachingProvider);

            using (Phantom = TestServer.Create(app => OwinStartup.Configuration(app, _containerBuilder.Build())))
            {
                var response = await Phantom.HttpClient.PostAsync("v1/cache?port=-1", new StringContent(string.Empty));

                response.StatusCode.Should().Be(HttpStatusCode.OK);
                response.Content.ReadAsStringAsync().Result.Should().BeEquivalentTo("\"Port -1 is configured to respond by the specified payload(0 means all ports, -1 means clear cache).\"");
            }
        }

        [Fact]
        public async void Validate_Clear_Cache_One_Port()
        {
            var cachingProvider = Substitute.For<ICachingProvider>();
            _containerBuilder.RegisterInstance(cachingProvider);

            using (Phantom = TestServer.Create(app => OwinStartup.Configuration(app, _containerBuilder.Build())))
            {
                var response = await Phantom.HttpClient.PostAsync("v1/cache?port=1", new StringContent(string.Empty));

                response.StatusCode.Should().Be(HttpStatusCode.OK);
                response.Content.ReadAsStringAsync().Result.Should().BeEquivalentTo("\"Port 1 is configured to respond by the specified payload(0 means all ports, -1 means clear cache).\"");
            }
        }

        [Fact]
        public async void Validate_Clear_Cache_Throws_Exception()
        {
            var cachingProvider = Substitute.For<ICachingProvider>();
            cachingProvider
                .When(x => x.ClearCache())
                .Do(x => { throw new FileNotFoundException("File not found!");});

            _containerBuilder.RegisterInstance(cachingProvider);

            using (Phantom = TestServer.Create(app => OwinStartup.Configuration(app, _containerBuilder.Build())))
            {
                var response = await Phantom.HttpClient.PostAsync("v1/cache?port=-1", new StringContent(string.Empty));

                response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

                response.Content.ReadAsStringAsync().Result.Should().Contain("Could not start the server.");
            }
        }

        [Fact]
        public async void Validate_Simulate_Throws_Exception()
        {
            var cachingProvider = Substitute.For<ICachingProvider>();
            cachingProvider
                .When(x => x.Save(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>()))
                .Do(x => { throw new Exception("Phantom the menace"); });

            _containerBuilder.RegisterInstance(cachingProvider);

            using (Phantom = TestServer.Create(app => OwinStartup.Configuration(app, _containerBuilder.Build())))
            {
                var response = await Phantom.HttpClient.PostAsync("v1/simulate?type=register&port=9000", new StringContent(string.Empty));

                response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

                response.Content.ReadAsStringAsync().Result.Should().Contain("Phantom the menace");
            }
        }
    }
}
