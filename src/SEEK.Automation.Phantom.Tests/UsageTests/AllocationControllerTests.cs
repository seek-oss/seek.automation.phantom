using FluentAssertions;
using Xunit;

namespace SEEK.Automation.Phantom.Tests.UsageTests
{
    public class AllocationControllerTests : TestBase
    {
        [Fact]
        public async void Health_Controller_Returns_Ok()
        {
            var response = await Phantom.HttpClient.GetAsync("/v1/list");
            
            response.Content.ReadAsStringAsync().Result.Should().StartWith("Port#     Process Name                                                Port Name");
        }
    }
}
