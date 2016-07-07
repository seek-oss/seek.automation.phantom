using Serilog;

namespace SEEK.Automation.Phantom.Domain
{

    public interface ISimulationFactory
    {
        ISimulation Create();
    }

    public class SimulationFactory : ISimulationFactory
    {
        public ILogger Logger;
        public IWebServerFactory WebServerFactory;

        public SimulationFactory(ILogger logger, IWebServerFactory webServerFactory)
        {
            Logger = logger;
            WebServerFactory = webServerFactory;
        }

        public ISimulation Create()
        {
            // TODO I know this is not the way to go...but please if you figure out the "Parameterized Instantiation (Func<X, Y, B>)" in autofac I am keen to know how:
            // http://autofac.readthedocs.org/en/latest/resolve/relationships.html?highlight=instantiation#parameterized-instantiation-func-x-y-b
            return new Simulation(Logger, WebServerFactory.Create());
        }
    }
}
