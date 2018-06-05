using System.Net.Http;
using Android.OS;


using Xamarin.Forms;
using QXFUtilities.Communication;


[assembly: Dependency(typeof(QXFUtilities.Android.Communication.HttpClientProvider))]
namespace QXFUtilities.Android.Communication
{
    public class HttpClientProvider : IHttpClientProvider
    {
        public HttpClient GetHttpClient(HttpClientImplementation implementation = HttpClientImplementation.Auto)
        {
            if (implementation == HttpClientImplementation.Managed)
            {
                return new HttpClient();
            }
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                return new HttpClient(new Xamarin.Android.Net.AndroidClientHandler());
            }
            return new HttpClient();
        }
    }
}