using System;
using System.Net;
using FluentAssertions;
using NSubstitute;
using RestSharp;
using Serilog;
using SEEK.Automation.Phantom.Support;
using Xunit;

namespace SEEK.Automation.Phantom.Tests.UnitTests
{
    public class InteractionNotFoundException : Exception
    {
        public InteractionNotFoundException(string message) : base(message) { }
    }

    public class WebServerTests
    {
        [Fact]
        public void Validate_When_Callback_Method_Is_Not_Specified()
        {
            var logger = Substitute.For<ILogger>();

            var webServer = new WebServer(logger);

            var ex = Assert.Throws<ArgumentException>(() => webServer.Simulate(null, 2000));

            ex.Message.Should().BeEquivalentTo("Callback method was not specified");
        }

        [Fact]
        public void Validate_When_Callback_Method_Throws()
        {
            var logger = Substitute.For<ILogger>();

            var webServer = new WebServer(logger);
            webServer.Simulate((port, listenerContext) => { throw new Exception("Because I can"); }, 12345);

            var client = new RestClient("http://localhost:12345");
            var request = new RestRequest("/please/give/me/some/food", Method.POST);
            var response = client.Execute(request);

            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

            webServer.Stop();
        }
    }
}
