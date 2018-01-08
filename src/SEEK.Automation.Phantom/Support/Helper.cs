using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Owin;
using Nancy.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PactNet.Mocks.MockHttpService.Models;

namespace SEEK.Automation.Phantom.Support
{
    public class Helper
    {
        public static string Between(string str, string firstString, string lastString)
        {
            var pos1 = str.IndexOf(firstString, StringComparison.Ordinal) + firstString.Length;
            var pos2 = str.IndexOf(lastString, StringComparison.Ordinal);
            var finalString = str.Substring(pos1, pos2 - pos1);

            return finalString;
        }

        public static string GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMdd-HHmmssfff");
        }

        public static string GetPactViaBroker(string pactBrokerUrl)
        {

            var myRequest = (HttpWebRequest)WebRequest.Create(pactBrokerUrl);
            myRequest.Method = "GET";
            var myResponse = myRequest.GetResponse();

            // ReSharper disable once AssignNullToNotNullAttribute
            var sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
            var result = sr.ReadToEnd();
            sr.Close();
            myResponse.Close();

            return result;
        }

        public static HttpResponseMessage GetResponseForRequestFromPact(ProviderServicePactFile pactFile, HttpListenerRequest request)
        {
            foreach (var providerServiceInteraction in pactFile.Interactions)
            {
                if (providerServiceInteraction.Request.Path.Equals(request.Url.LocalPath))
                {
                    if (!string.IsNullOrEmpty(request.Url.Query))
                    {
                        if(string.IsNullOrEmpty(providerServiceInteraction.Request.Query)) continue;
                        
                        var currentPactQuery = HttpUtility.ParseQueryString(providerServiceInteraction.Request.Query);
                        var currentQuery = HttpUtility.ParseQueryString(request.Url.Query);

                        var isCurrentPactQueryTheSameAsCurrentRequestQuery = currentPactQuery.FilterCollection().CollectionEquals(currentQuery.FilterCollection());

                        if(!isCurrentPactQueryTheSameAsCurrentRequestQuery) continue;
                    }

                    var response = new HttpResponseMessage
                    {
                        StatusCode = (HttpStatusCode) providerServiceInteraction.Response.Status
                    };

                    var content = providerServiceInteraction.Response.Body == null ? string.Empty : providerServiceInteraction.Response.Body.ToString();
                    
                    Console.WriteLine(content);

                    response.Content = new StringContent(ApplyStaticRules(content));

                    return response;
                }
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        public static string GetClientIp(HttpRequestMessage request)
        {
            string clientIp;

            try
            {
                var owinContext = (OwinContext)request.Properties["MS_OwinContext"];

                clientIp = owinContext.Request.RemoteIpAddress;
            }
            catch (Exception)
            {
                clientIp = "UKNOWN";
            }
        
            return clientIp;
        }

        public static string ApplyStaticRules(string str)
        {
            var random = new Random();

            var updatedString = str.Replace("[GUID]", Guid.NewGuid().ToString());
            updatedString = updatedString.Replace("[INT]", random.Next().ToString(CultureInfo.InvariantCulture));
            return updatedString;
        }

        public static bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || (strInput.StartsWith("[") && strInput.EndsWith("]"))) 
            {
                try
                {
                    JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }

            return false;
        }
    }

    public static class NameValueCollection
    {
        public static bool CollectionEquals(this System.Collections.Specialized.NameValueCollection nameValueCollection1, System.Collections.Specialized.NameValueCollection nameValueCollection2)
        {
            return nameValueCollection1.ToKeyValue().SequenceEqual(nameValueCollection2.ToKeyValue());
        }

        private static IEnumerable<object> ToKeyValue(this System.Collections.Specialized.NameValueCollection nameValueCollection)
        {
            return nameValueCollection.AllKeys.OrderBy(x => x).Select(x => new { Key = x, Value = nameValueCollection[x] });
        }

        public static System.Collections.Specialized.NameValueCollection FilterCollection(this System.Collections.Specialized.NameValueCollection nameValueCollection)
        {
            var filteredCollection = new System.Collections.Specialized.NameValueCollection();

            var unwantedKeys = new List<string> { "oauth_consumer_key", "oauth_timestamp", "oauth_signature" };
            foreach (var key in nameValueCollection.AllKeys)
            {
                if(unwantedKeys.Contains(key)) continue;
                
                filteredCollection.Add(key, nameValueCollection[key]);
            }

            return filteredCollection;
        }
    }
}
