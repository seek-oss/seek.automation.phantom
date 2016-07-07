using System;
using Xunit;
using FluentAssertions;

namespace SEEK.Automation.Phantom.Tests.UsageTests
{
    public class VersionControllerTests : TestBase
    {
        [Fact]
        public async void Health_Controller_Returns_Ok()
        {
            var response = await Phantom.HttpClient.GetAsync("/v1/version");

            Version version;
            Version.TryParse(response.Content.ReadAsStringAsync().Result.Trim().Replace("\"", ""), out version).Should().BeTrue();
        }
    }
}
