using System.Net.Http;
using Xamarin.Forms;
using QXFUtilities.Communication;

[assembly: Dependency(typeof(QXFUtilities.UWP.Communication.HttpClientProvider))]
namespace QXFUtilities.UWP.Communication
{
    public class HttpClientProvider : IHttpClientProvider
    {
        public HttpClient GetHttpClient(HttpClientImplementation implementation = HttpClientImplementation.Auto)
        {
            // UWP currently does not support System.Net.ServicePointManager
            //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            return new HttpClient();
        }
    }
}