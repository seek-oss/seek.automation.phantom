using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace SEEK.Automation.Phantom.Support
{
    public interface IWebServer
    {
        void Simulate(Func<int, HttpListenerContext, HttpResponseMessage> callbackMethod, int port);

        Func<int, HttpListenerContext, HttpResponseMessage> CallbackMethod { get; set; }
    }

    public class WebServer : IWebServer
    {
        private readonly HttpListener _httpListener;
        private readonly ILogger _logger;

        public Func<int, HttpListenerContext, HttpResponseMessage> CallbackMethod { get; set; }
        
        public WebServer(ILogger logger)
        {
            _httpListener = new HttpListener();
            _logger = logger;
        }

        public void Simulate(Func<int, HttpListenerContext, HttpResponseMessage> callbackMethod, int port)
        {
            _logger.Information("Starting the web server for port {0}", port);

            var prefix = string.Format("http://*:{0}/", port);

            CallbackMethod = callbackMethod;

            if (CallbackMethod == null)
            {
                throw new ArgumentException("Callback method was not specified");
            }

            if (!HttpListener.IsSupported)
            {
                throw new NotSupportedException("HttpListener is only supported on Windows XP SP2, Server 2003 or later");
            }

            ServicePointManager.DefaultConnectionLimit = 500;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.MaxServicePoints = 500;
            ServicePointManager.UseNagleAlgorithm = true;
            ServicePointManager.MaxServicePointIdleTime = 20000;

            _httpListener.Prefixes.Add(prefix);

            _httpListener.Start();

            Task.Factory.StartNew(() =>
            {
                while (_httpListener.IsListening)
                {
                    var httpListenerContext = _httpListener.GetContext();
                    try
                    {
                        var response = CallbackMethod(port, httpListenerContext);

                        httpListenerContext.Response.StatusCode = (int)response.StatusCode;
                        var content = response.Content != null ? response.Content.ReadAsStringAsync().Result : null;

                        if (!string.IsNullOrEmpty(content))
                        {
                            var buf = Encoding.UTF8.GetBytes(content);
                            httpListenerContext.Response.ContentLength64 = buf.Length;
                            httpListenerContext.Response.OutputStream.Write(buf, 0, buf.Length);   
                        }
                    }
                    catch (Exception ex)
                    {
                        httpListenerContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        _logger.Error(ex, "Failed to start the web server.");
                    }
                    finally
                    {
                        httpListenerContext.Response.OutputStream.Close();
                        httpListenerContext.Response.OutputStream.Dispose();
                    }
                }
            });
        }

        public void Stop()
        {
            _httpListener.Stop();
            _httpListener.Close();
        }
    }
}

