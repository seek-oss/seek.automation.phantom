using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using Serilog;

namespace SEEK.Automation.Phantom
{
    [ExcludeFromCodeCoverage]
    public class SerilogExceptionLogger : IExceptionLogger
    {
        private readonly ILogger _logger;
        private readonly Task _done = Task.FromResult(1);

        public SerilogExceptionLogger(ILogger logger)
        {
            _logger = logger;
        }

        public Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
        {
            _logger.Error(context.Exception, "WebApi error for {Request}", context.Request);
            return _done;
        }
    }
}
