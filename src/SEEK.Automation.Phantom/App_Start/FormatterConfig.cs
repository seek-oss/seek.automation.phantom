using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace SEEK.Automation.Phantom
{
    [ExcludeFromCodeCoverage]
    public class FormatterConfig
    {
        public static void RegisterFormatters(HttpConfiguration config)
        {
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            var jsonSettings = config.Formatters.JsonFormatter.SerializerSettings;
            jsonSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonSettings.Converters = new List<JsonConverter> { new StringEnumConverter { CamelCaseText = true } };
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
        }
    }
}
