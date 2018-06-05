
using System.Net.Http;

namespace QXFUtilities.Communication
{
    public interface IHttpClientProvider
    {
        HttpClient GetHttpClient(HttpClientImplementation implementation = HttpClientImplementation.Auto);
    }

    public enum HttpClientImplementation
    {
        Auto,
        Managed,
        Native
    }
}
