using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using SEEK.Automation.Phantom.Domain;
using SEEK.Automation.Phantom.Support;
using Serilog;

// ReSharper disable All
namespace SEEK.Automation.Phantom.Controllers
{
    public class RequestController : ApiController
    {
        public IBroker Broker;
        public ILogger Logger;
        public ICachingProvider CachingProvider;
        
        public RequestController(ILogger logger, IBroker broker, ICachingProvider cachingProvider)
        {
            Broker = broker;
            Logger = logger;
            CachingProvider = cachingProvider;
        }

        [HttpPost]
        [Route("v1/simulate")]
        public async Task<HttpResponseMessage> Register(int port, string type)
        {
            Logger.Information("Client at IP '{0}' requested a simulation of type {1} on port {2}.", Helper.GetClientIp(Request), type, port);

            try
            {
                var payload = await Request.Content.ReadAsStringAsync();

                CachingProvider.Save(port, type, payload);
                Broker.Initialise(port, type, payload);

                return Request.CreateResponse(HttpStatusCode.OK, string.Format("Port {0} is configured to respond by the specified payload.", port));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Could not start the server. {0}", ex);
            }
        }

        [HttpPost]
        [Route("v1/cache")]
        public  HttpResponseMessage Cache(int port)
        {
            Logger.Information("Client at IP '{0}' requested a load from cache on port {1}.", Helper.GetClientIp(Request), port);

            try
            {
                if (port.Equals(-1))
                {
                    CachingProvider.ClearCache();
                }
                else
                {
                    CachingProvider.Load(port);
                }

                return Request.CreateResponse(HttpStatusCode.OK, string.Format("Port {0} is configured to respond by the specified payload(0 means all ports, -1 means clear cache).", port));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Could not start the server. {0}", ex);
            }
        }
    }
}
