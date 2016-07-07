using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Autofac;
using Microsoft.Owin.Testing;
using RestSharp;
using Serilog;

namespace SEEK.Automation.Phantom.Tests.UsageTests
{
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public class TestBase : IDisposable
    {
        private readonly ContainerBuilder _containerBuilder;
        protected TestServer Phantom;

        public TestBase()
        {
            _containerBuilder = ContainerConfig.CreateContainerWithoutDependency();
            _containerBuilder.RegisterInstance(CreateTestLogger()).As<ILogger>();

            Phantom = TestServer.Create(app => OwinStartup.Configuration(app, BuildPhantom()));
        }

        public IContainer BuildPhantom()
        {
            return _containerBuilder.Build();
        }

        public static ILogger CreateTestLogger()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var logDirectory = Path.Combine(currentDirectory, "logs");

            return new LoggerConfiguration().WriteTo.Console().WriteTo.RollingFile(
                    Path.Combine(logDirectory, "seek.automation.phantom-{Date}.txt"),
                    retainedFileCountLimit: 5,
                    fileSizeLimitBytes: 10000000
                ).CreateLogger();
        }

        public IRestResponse DoHttpPost(string baseUrl, string path, string jsonBody = null)
        {
            var client = new RestClient(baseUrl);
            var request = new RestRequest(path, Method.POST);

            if (!string.IsNullOrEmpty(jsonBody))
            {
                request.RequestFormat = DataFormat.Json;
                request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);
            }

            return client.Execute(request);
        }

        public IRestResponse DoHttpGet(string path)
        {
            var client = new RestClient(path);
            var request = new RestRequest(path, Method.GET);
            
            return client.Execute(request);
        }

        public void Dispose()
        {
            Phantom.Dispose();
        }
    }
}
