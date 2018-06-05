using System.Net.Http;
using UIKit;
using Xamarin.Forms;
using QXFUtilities.Communication;

[assembly: Dependency(typeof(QXFUtilities.iOS.Communication.HttpClientProvider))]
namespace QXFUtilities.iOS.Communication
{
    public class HttpClientProvider : IHttpClientProvider
    {
        public HttpClient GetHttpClient(HttpClientImplementation implementation = HttpClientImplementation.Auto)
        {
            if (implementation == HttpClientImplementation.Managed)
            {
                return new HttpClient();
            }
            if (UIDevice.CurrentDevice.CheckSystemVersion(7,0))
            {
                return new HttpClient(new NSUrlSessionHandler());
            }
            else if (UIDevice.CurrentDevice.CheckSystemVersion(6,0))
            {
                return new HttpClient(new CFNetworkHandler());
            }
            return new HttpClient();
        }
    }
}