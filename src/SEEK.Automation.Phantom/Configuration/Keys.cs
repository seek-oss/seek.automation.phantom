using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace SEEK.Automation.Phantom.Configuration
{
    [ExcludeFromCodeCoverage]
    public class Keys
    {
        public static string Host = "SEEK.Automation.Phantom.Host";
        public static string Port = "SEEK.Automation.Phantom.Port";
        public static string CacheDir = "SEEK.Automation.Phantom.Cache";
        public static string FirewallCreateRules = "SEEK.Automation.Phantom.Firewall.Create.Rules";
        public static string FirewallPortFrom = "SEEK.Automation.Phantom.Firewall.Port.Range.From";
        public static string FirewallPortTo = "SEEK.Automation.Phantom.Firewall.Port.Range.To";


        public static string TryGetConfigValue(string key, string @default)
        {
            try
            {
                return ConfigurationManager.AppSettings[key];
            }
            catch (Exception)
            {
                return @default;
            }
        }
    }
}
