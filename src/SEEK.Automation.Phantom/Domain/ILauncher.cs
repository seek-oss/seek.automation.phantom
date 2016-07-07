using Autofac;
using Serilog;
using NetFwTypeLib;
using SEEK.Automation.Phantom.Configuration;

namespace SEEK.Automation.Phantom.Domain
{
    public class Launcher : IStartable
    {
        private readonly ILogger _logger;
        private readonly IWindowsFirewallHelper _firewallHelper;

        public Launcher(ILogger logger, IWindowsFirewallHelper fwHelper)
        {
            _logger = logger;

            _firewallHelper = fwHelper;
        }

        public void Start()
        {
            if (bool.Parse(Keys.TryGetConfigValue(Keys.FirewallCreateRules, "false")))
            {
                _logger.Information("Open inbound port...");
                _firewallHelper.CreateGenericFwRule(true, "Simulator", "Open ports for Simulator", NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN, NET_FW_ACTION_.NET_FW_ACTION_ALLOW, "All");

                _logger.Information("Open outbound port...");
                _firewallHelper.CreateGenericFwRule(true, "Simulator", "Open ports for simulator", NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT, NET_FW_ACTION_.NET_FW_ACTION_ALLOW, "All");

                return;
            }

            _logger.Warning("Service is configured to not open firewall ports.");
            _logger.Warning("The registrations may not work unless the ports are already opened.");
        }
    }
}
