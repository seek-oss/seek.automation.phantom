using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Serilog;

namespace SEEK.Automation.Phantom.Domain
{
    public interface IBroker
    {
        void Initialise(int port, string type, string payload);
    }

    public class Broker : IBroker
    {
        public string Payload = string.Empty;
        public IGeneric Generic;
        public ILogger Logger;
        private readonly ISimulationFactory _simulationFactory;
        private static readonly Dictionary<int, ISimulation> Simulations = new Dictionary<int, ISimulation>();
        private readonly Dictionary<string, Func<int, HttpListenerContext, HttpResponseMessage>> _callbackMap;

        public int Port { get; set; }

        public Broker(ILogger logger, ISimulationFactory simulationFactory, IGeneric generic)
        {
            Logger = logger;
            _simulationFactory = simulationFactory;
            Generic = generic;
            Port = -1;

            _callbackMap = new Dictionary<string, Func<int, HttpListenerContext, HttpResponseMessage>>
            {
                {"register", RegisterCallback},
                {"pact", PactCallback},
                {"redirect", RedirectCallback},
                {"log", LogCallback}
            };
        }

        public void Initialise(int port, string type, string payload)
        {
            var callbackMethod = _callbackMap[type];
            if (UpdateSimulation(port, payload, callbackMethod)) return;

            var simulation = _simulationFactory.Create();
            simulation.Configure(port, type, payload, callbackMethod);

            Simulations.Add(port, simulation);
        }

        public HttpResponseMessage RegisterCallback(int port, HttpListenerContext listenerContext)
        {
            var regRes = Generic.DefaultRegistration(Simulations[port].Payload);

            return regRes;
        }

        public HttpResponseMessage PactCallback(int port, HttpListenerContext listenerContext)
        {
            var regRes = Generic.PactRegistration(Simulations[port].Payload, listenerContext);

            return regRes;
        }

        public HttpResponseMessage RedirectCallback(int port, HttpListenerContext listenerContext)
        {
            var regRes = Generic.RedirectRegistration(Simulations[port].Payload, listenerContext);

            return regRes;
        }

        public HttpResponseMessage LogCallback(int port, HttpListenerContext listenerContext)
        {
            var regRes = Generic.LogRegistration(Simulations[port].Payload, listenerContext);

            return regRes;
        }

        private bool UpdateSimulation(int port, string payload, Func<int, HttpListenerContext, HttpResponseMessage> callbackMethod)
        {
            Logger.Information("Update simulation for port {0}...", port);

            if (!Simulations.ContainsKey(port)) return false;

            Simulations[port].UpdateCallbackMethod(callbackMethod);
            Simulations[port].Payload = payload;

            return true;
        }
    }
}
