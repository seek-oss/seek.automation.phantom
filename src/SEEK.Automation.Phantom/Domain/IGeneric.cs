using System;
using System.IO;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using PactNet;
using PactNet.Mocks.MockHttpService.Models;
using SEEK.Automation.Phantom.Support;
using Serilog;

namespace SEEK.Automation.Phantom.Domain
{
    public interface IGeneric
    {
        HttpResponseMessage DefaultRegistration(string payload);
        HttpResponseMessage PactRegistration(string payload, HttpListenerContext listenerContext);
        HttpResponseMessage RedirectRegistration(string payload, HttpListenerContext listenerContext);
        HttpResponseMessage LogRegistration(string payload, HttpListenerContext listenerContext);
    }

    public class Generic : IGeneric
    {
        public ILogger Logger;

        public Generic(ILogger logger)
        {
            Logger = logger;
        }

        public HttpResponseMessage DefaultRegistration(string payload)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(Helper.ApplyStaticRules(payload))
            };

            return response;
        }

        public HttpResponseMessage PactRegistration(string payload, HttpListenerContext listenerContext)
        {
            if (payload.StartsWith("http://pact:9292/") || payload.StartsWith("pact:9292"))
            {
                payload = Helper.GetPactViaBroker(payload);
            }

            payload = Helper.ApplyStaticRules(payload);
            var pactFile = JsonConvert.DeserializeObject<ProviderServicePactFile>(payload);
            var pb = new PactBuilder();
            pb.ServiceConsumer(pactFile.Consumer.Name);

            var responseToCurrentRequest = Helper.GetResponseForRequestFromPact(pactFile, listenerContext.Request);

            return responseToCurrentRequest;
        }

        public HttpResponseMessage RedirectRegistration(string payload, HttpListenerContext listenerContext)
        {
            if (!Uri.IsWellFormedUriString(payload, UriKind.RelativeOrAbsolute))
            {
                throw new Exception("Please specify a well formed Uri for redirection.");
            }

            payload = Helper.ApplyStaticRules(payload);

            listenerContext.Response.Redirect(payload);

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Redirect,
                Content = new StringContent("Redirected")
            };

            return response;
        }

        public HttpResponseMessage LogRegistration(string payload, HttpListenerContext listenerContext)
        {
            var body = new StreamReader(listenerContext.Request.InputStream).ReadToEnd();

            payload = Helper.ApplyStaticRules(payload);

            File.WriteAllText(payload, body);

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(Helper.ApplyStaticRules(payload))
            };

            return response;
        }
    }
}
