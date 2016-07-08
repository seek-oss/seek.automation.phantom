using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SEEK.Automation.Phantom.Support
{
    [ExcludeFromCodeCoverage]
    public static class HttpResponseExtensions
    {
        public static bool IsSuccessful(this HttpStatusCode code)
        {
            return (int)code < 300 && (int)code >= 200;
        }
    }
}
