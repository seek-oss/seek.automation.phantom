using SEEK.Automation.Phantom.Support;
using Serilog;

namespace SEEK.Automation.Phantom.Domain
{
    public interface IWebServerFactory
    {
        IWebServer Create();
    }

    public class WebServerFactory : IWebServerFactory
    {
        public ILogger Logger;

        public WebServerFactory(ILogger logger)
        {
            Logger = logger;
        }

        public IWebServer Create()
        {
            // TODO I know this is not the way to go...but please if you figure out the "Parameterized Instantiation (Func<X, Y, B>)" in autofac I am keen to know how:
            // http://autofac.readthedocs.org/en/latest/resolve/relationships.html?highlight=instantiation#parameterized-instantiation-func-x-y-b
            return new WebServer(Logger);
        }
    }
}
