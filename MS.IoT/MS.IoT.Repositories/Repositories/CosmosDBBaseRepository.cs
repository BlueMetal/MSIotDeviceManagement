using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using MS.IoT.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Repositories
{
    public abstract class CosmosDBBaseRepository
    {
        protected static DocumentClient _client;
        protected string _databaseId;
        protected string _collectionId;

        protected CosmosDBBaseRepository(string endPoint, string authKey, string databaseId, string collectionId)
        {
            _databaseId = databaseId;
            _collectionId = collectionId;
            InitializeClient(endPoint, authKey);
        }

        private static DocumentClient InitializeClient(string endPoint, string authKey)
        {
            if (_client == null)
            {
                Uri endpointUri = new Uri(endPoint);
                _client = new DocumentClient(endpointUri, authKey);
            }
            return _client;
        }

        public async Task Initialize()
        {
            await CreateDatabaseIfNotExists();
            await CreateCollectionIfNotExists();
        }

        private async Task CreateDatabaseIfNotExists()
        {
            try
            {
                await _client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(_databaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await _client.CreateDatabaseAsync(new Database { Id = _databaseId });
                }
                else
                {
                    Log.Error(e.Message);
                    throw;
                }
            }
        }

        private async Task CreateCollectionIfNotExists()
        {
            try
            {
                await _client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await _client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(_databaseId),
                        new DocumentCollection { Id = _collectionId },
                        new RequestOptions { OfferThroughput = 1000 });
                }
                else
                {
                    Log.Error(e.Message);
                    throw;
                }
            }
        }
    }
}
