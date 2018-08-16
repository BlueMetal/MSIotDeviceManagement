using Microsoft.Azure.Documents.Client;
using System;

namespace MS.IoT.Repositories
{
    public class DocumentDBConfig
    {
        private static DocumentClient _client;

        public static DocumentClient GetClient(string endPoint, string authKey)
        {
            if (_client == null)
            {
                Uri endpointUri = new Uri(endPoint);
                _client = new DocumentClient(endpointUri, authKey);
            }
            return _client;
        }
    }
}
