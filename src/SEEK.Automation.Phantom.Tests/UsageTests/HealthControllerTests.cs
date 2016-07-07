using System.Net;
using Xunit;
using FluentAssertions;

namespace SEEK.Automation.Phantom.Tests.UsageTests
{
    public class HealthControllerTests : TestBase
    {
        [Fact]
        public async void Health_Controller_Returns_Ok()
        {
            var response = await Phantom.HttpClient.GetAsync("/v1/health");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
