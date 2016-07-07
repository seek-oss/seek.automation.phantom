using System.Net;
using System.Net.Http;
using System.Web.Http;
using SEEK.Automation.Phantom.Support;

namespace SEEK.Automation.Phantom.Controllers
{
    [Route("v1/list")]
    public class AllocationController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Overall()
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(PortHelper.ListOccupiedPorts())
            };
        }
    }
}
