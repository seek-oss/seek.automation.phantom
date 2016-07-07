using System.Web.Http;

namespace SEEK.Automation.Phantom.Controllers
{
    public class VersionController : ApiController
    {
        public string Get()
        {
            return typeof(Service).Assembly.GetName().Version.ToString();
        }
    }
}
