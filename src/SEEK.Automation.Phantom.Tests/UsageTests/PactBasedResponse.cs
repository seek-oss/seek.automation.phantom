using System.IO;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using Xunit;

namespace SEEK.Automation.Phantom.Tests.UsageTests
{
    public class PactBasedResponse : TestBase
    {
        [Fact]
        public async void Can_Simulate_A_Single_Response()
        {
            var phantomResponse = await Phantom.HttpClient.PostAsync("/v1/simulate?type=pact&port=9000", new StringContent(File.ReadAllText("Data/Pact.json")));
        
            phantomResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var response = DoHttpPost("http://localhost:9000", "/please/give/me/some/money");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Should().Be("{\r\n  \"name\": \"phantom\"\r\n}");
        }
    }
}
