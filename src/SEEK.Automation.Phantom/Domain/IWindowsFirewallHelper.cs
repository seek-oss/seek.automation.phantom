using System;
using System.Linq;
using NetFwTypeLib;
using Serilog;
using SEEK.Automation.Phantom.Configuration;

namespace SEEK.Automation.Phantom.Domain
{
    public interface IWindowsFirewallHelper
    {
        void CreateGenericFwRule(bool enabled, string ruleName, string ruleDescription, NET_FW_RULE_DIRECTION_ direction, NET_FW_ACTION_ permissionType, string interfaceType);
    }

    public class WindowsFirewallHelper : IWindowsFirewallHelper
    {
        private readonly ILogger _logger;

        public WindowsFirewallHelper(ILogger logger)
        {
            _logger = logger;
        }

        public void CreateGenericFwRule(bool enabled, string ruleName, string ruleDescription, NET_FW_RULE_DIRECTION_ direction, NET_FW_ACTION_ permissionType, string interfaceType)
        {
            var firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            if (firewallPolicy.Rules.Cast<INetFwRule2>().Any(rule => rule.Name.Equals(ruleName)))
            {
                _logger.Warning("The rule with the name {RuleName} already exists. Will not create another one!", ruleName);
                return;
            }

            var portsToOpen = string.Empty;
            var from = Keys.TryGetConfigValue(Keys.FirewallPortFrom, "9000");
            var to = Keys.TryGetConfigValue(Keys.FirewallPortTo, "9025");
            for(var i = int.Parse(from); i <= int.Parse(to); i++)
            {
                portsToOpen = portsToOpen + "," + i;
            }

            portsToOpen = portsToOpen.Remove(0, 1);

            var firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
            firewallRule.Action = permissionType;
            firewallRule.Description = ruleDescription;
            firewallRule.Direction = direction;
            firewallRule.Enabled = enabled;
            firewallRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
            firewallRule.InterfaceTypes = interfaceType;
            firewallRule.Name = ruleName;
            firewallRule.LocalPorts = portsToOpen;

            firewallPolicy.Rules.Add(firewallRule);

            _logger.Information("Created a rule for the specified port for Windows firewall.");
        }
    }
}
