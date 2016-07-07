using System;
using System.Net;
using System.Net.Http;
using SEEK.Automation.Phantom.Support;
using Serilog;

namespace SEEK.Automation.Phantom.Domain
{
    public interface ISimulation
    {
        ISimulation Configure(int port, string type, string payload, Func<int, HttpListenerContext, HttpResponseMessage> callbackMethod);
        void UpdateCallbackMethod(Func<int, HttpListenerContext, HttpResponseMessage> callbackMethod);

        string Payload { get; set; }
        Func<int, HttpListenerContext, HttpResponseMessage> CallbackMethod { get; set; }
    }

    public class Simulation : ISimulation
    {
        public int Port;
        public string Type;
        public ILogger Logger;
        public IWebServer WebServer;

        public string Payload { get; set; }

        public Func<int, HttpListenerContext, HttpResponseMessage> CallbackMethod { get; set; }

        public Simulation(ILogger logger, IWebServer webServer)
        {
            Logger = logger;
            WebServer = webServer;
        }

        public ISimulation Configure(int port, string type, string payload, Func<int, HttpListenerContext, HttpResponseMessage> callbackMethod)
        {
            Port = port;
            Type = type;
            Payload = payload;
            CallbackMethod = callbackMethod;

            WebServer.Simulate(CallbackMethod, Port);

            return this;
        }

        public void UpdateCallbackMethod(Func<int, HttpListenerContext, HttpResponseMessage> callbackMethod)
        {
            CallbackMethod = callbackMethod;
            WebServer.CallbackMethod = CallbackMethod;
        }
    }
}
