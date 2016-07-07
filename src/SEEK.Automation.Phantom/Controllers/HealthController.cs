using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SEEK.Automation.Phantom.Controllers
{
    [Route("v1/health")]
    public class HealthController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Overall()
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("OK")
            };
        }
    }
}
