using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Autofac;
using Autofac.Core;
using Serilog;

namespace SEEK.Automation.Phantom
{
    [ExcludeFromCodeCoverage]
    public class SerilogLoggingModule : Module
    {
        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            registration.Preparing += AddILoggerToParameters;
        }

        private static void AddILoggerToParameters(object sender, PreparingEventArgs e)
        {
            var t = e.Component.Activator.LimitType;
            e.Parameters = e.Parameters.Union(
                new[]
                {
                    new ResolvedParameter((p, i) => p.ParameterType == typeof(ILogger), (p, i) => i.Resolve<ILogger>().ForContext(t))
                });
        }
    }
}
