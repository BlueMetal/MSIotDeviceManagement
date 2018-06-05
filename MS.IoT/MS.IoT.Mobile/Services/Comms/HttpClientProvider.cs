using System.Net.Http;
using Xamarin.Forms;


namespace QXFUtilities.Communication
{
    public class HttpClientProvider : IHttpClientProvider
    {
        public HttpClient GetHttpClient(HttpClientImplementation implementation = HttpClientImplementation.Auto)
        {
            var provider = DependencyService.Get<IHttpClientProvider>();
            return provider.GetHttpClient(implementation);
        }

        public static HttpClient HttpClient(HttpClientImplementation implementation = HttpClientImplementation.Auto)
        {
            var provider = DependencyService.Get<IHttpClientProvider>();
            return provider.GetHttpClient(implementation);
        }
    }
}