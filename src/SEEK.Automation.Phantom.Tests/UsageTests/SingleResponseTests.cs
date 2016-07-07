using System.Net;
using System.Net.Http;
using FluentAssertions;
using Xunit;

namespace SEEK.Automation.Phantom.Tests.UsageTests
{
    public class SingleResponseTests : TestBase
    {
        [Fact]
        public async void Can_Simulate_A_Single_Response()
        {
            var simulatedResponse = "{\r\n  \"status\": 200,\r\n   \"body\": {\r\n        \"name\": \"phantom\"\r\n   }        \r\n }";

            var phantomResponse = await Phantom.HttpClient.PostAsync("/v1/simulate?type=register&port=9000", new StringContent(simulatedResponse));

            phantomResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var response = DoHttpGet("http://localhost:9000");
            
            response.Content.Should().Be(simulatedResponse);
        }
    }
}
