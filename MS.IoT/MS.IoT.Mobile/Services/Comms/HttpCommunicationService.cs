using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Polly;

namespace QXFUtilities.Communication
{
    public class HttpCommunicationService
    {
        private HttpClient _client = null;
        public HttpClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = HttpClientProvider.HttpClient();
                }
                return _client;
            }
        }

        public Task<T> DeleteAsync<T>(string baseAddress, string requestUri)
        {
            return SendWithRetryAsync<T>(HttpRequestType.Delete, baseAddress, requestUri);
        }

        public Task<T> PutAsync<T, K>(string baseAddress, string requestUri, K obj) // where object
        {
            var jsonRequest = !obj.Equals(default(K)) ? JsonConvert.SerializeObject(obj) : null;
            return SendWithRetryAsync<T>(HttpRequestType.Put, baseAddress, requestUri, jsonRequest);
        }
        public Task<T> PutAsync<T>(string baseAddress, string requestUri) // where object
        {
            return SendWithRetryAsync<T>(HttpRequestType.Put, baseAddress, requestUri);
        }
        public Task<T> PostAsync<T, K>(string baseAddress, string requestUri, K obj) // where object is the object that will be serialized into Json
        {
            var jsonRequest = !obj.Equals(default(K)) ? JsonConvert.SerializeObject(obj) : null;
            return SendWithRetryAsync<T>(HttpRequestType.Post, baseAddress, requestUri, jsonRequest);
        }


        public Task<T> GetAsync<T>(string baseAddress, string requestUri)
        {
            return SendWithRetryAsync<T>(HttpRequestType.Get, baseAddress, requestUri);
        }

        async Task<T> SendWithRetryAsync<T>(HttpRequestType requestType, string baseAddress, string requestUri, string jsonRequest = null)
        {
            return await Policy
                .Handle<WebException>()
                .WaitAndRetryAsync(5, retryAttempt =>
                                   TimeSpan.FromMilliseconds((200 * retryAttempt)),
                    (exception, timeSpan, context) =>
                    {
                        Debug.WriteLine(exception.ToString());
                    }
                )
                .ExecuteAsync(async () => { return await SendAsync<T>(requestType, baseAddress, requestUri, jsonRequest); });
        }
        async Task<string> SendWithRetryAsync(HttpRequestType requestType, string baseAddress, string requestUri, string jsonRequest = null)
        {
            return await Policy
                .Handle<WebException>()
                .WaitAndRetryAsync(5, retryAttempt =>
                                   TimeSpan.FromMilliseconds((200 * retryAttempt)),
                    (exception, timeSpan, context) =>
                    {
                        Debug.WriteLine(exception.ToString());
                    }
                )
                .ExecuteAsync(async () => { return await SendAsync(requestType, baseAddress, requestUri, jsonRequest); });
        }
        async Task<T> SendAsync<T>(HttpRequestType requestType, string baseAddress, string requestUri, string jsonRequest = null)
        {
            T result = default(T);

            HttpContent content = null;

            if (jsonRequest != null)
                content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            if (Client.BaseAddress == null)
                Client.BaseAddress = new Uri(baseAddress);

            Task<HttpResponseMessage> httpTask = null;

            switch (requestType)
            {
                case HttpRequestType.Delete:
                    httpTask = Client.DeleteAsync(requestUri);
                    break;
                case HttpRequestType.Put:
                    httpTask = Client.PutAsync(requestUri, content);
                    break;
                case HttpRequestType.Get:
                    httpTask = Client.GetAsync(requestUri);
                    break;
                 case HttpRequestType.Post:
                    httpTask = Client.PostAsync(requestUri, content);
                    break;
                default:
                    throw new Exception("Not a valid request type");
            }

            HttpResponseMessage response = new HttpResponseMessage();
            if (httpTask != null)
            {
                string json = string.Empty;
                try
                {
                    response = await httpTask.ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                        return result;

                    if (response != null)
                        json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                }
                catch(Exception e)
                {

                }
                if (!string.IsNullOrEmpty(json))
                {
                    try
                    {
                        result = JsonConvert.DeserializeObject<T>(json);
                    }
                    catch (Exception ex) { }
                }
            }
            return result;
        }
        async Task<String> SendAsync(HttpRequestType requestType, string baseAddress, string requestUri, string jsonRequest = null)
        {
            string result = string.Empty;

            HttpContent content = null;

            if (jsonRequest != null)
                content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            if (Client.BaseAddress == null)
                Client.BaseAddress = new Uri(baseAddress);

            Task<HttpResponseMessage> httpTask = null;

            switch (requestType)
            {
                case HttpRequestType.Delete:
                    httpTask = Client.DeleteAsync(requestUri);
                    break;
                case HttpRequestType.Put:
                    httpTask = Client.PutAsync(requestUri, content);
                    break;
                case HttpRequestType.Get:
                    httpTask = Client.GetAsync(requestUri);
                    break;
                case HttpRequestType.Post:
                    httpTask = Client.PostAsync(requestUri, content);
                    break;
                default:
                    throw new Exception("Not a valid request type");
            }

            HttpResponseMessage response = new HttpResponseMessage();
            if (httpTask != null)
            {
                try
                {
                    response = await httpTask.ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                        return result;

                    if (response != null)
                        result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                catch (Exception ex) { }
            }
            return result;
        }

    }
    public enum HttpRequestType
    {
        Delete,
        Get,
        Post,
        Put
    }

}
