using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using MS.IoT.Domain.Interface;
using MS.IoT.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using MS.IoT.Common;

namespace MS.IoT.Repositories
{
    public class CosmosDBRepositorySetup<T> : ICosmosDBRepository<T> where T : class
    {
        private string _databaseId;
        private string _collectionId;
        private Uri _endPoint;
        private string _authKey;

        public CosmosDBRepositorySetup(string endPoint, string authKey, string databaseId, string collectionId)
        {
            _databaseId = databaseId;
            _collectionId = collectionId;
            _endPoint = new Uri(endPoint);
            _authKey = authKey;
        }

        public async Task Initialize()
        {
            await CreateDatabaseIfNotExists();
            await CreateCollectionIfNotExists();
        }

        public async Task<T> GetItemAsync(string id)
        {
            DocumentClient client = null;
            try
            {
                client = new DocumentClient(_endPoint, _authKey);
                Document document = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id));
                return (T)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    Log.Error("GetItemAsync error: DocumentClientException: " + e.Error.Message);
                    throw;
                }
            }
            catch (Exception e)
            {
                Log.Error("GetItemAsync error: " + e.Message);
                return null;
            }
            finally
            {
                if (client != null)
                    client.Dispose();
            }
        }

        public async Task<bool> IsItemExistsByIdAsync(string Id)
        {
            DocumentClient client = null;
            try
            {
                client = new DocumentClient(_endPoint, _authKey);
                Document document = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, Id));
                return true;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return false;
                }
                else
                {
                    Log.Error("GetItemAsync error: DocumentClientException: " + e.Error.Message);
                    throw e;
                }
            }
            catch (Exception e)
            {
                Log.Error("GetItemAsync error: " + e.Message);
                throw e;
            }
        }

        public async Task<bool> IsItemExistsByNonIdAsync(Expression<Func<T, bool>> where)
        {
            DocumentClient client = null;
            try
            {
                client = new DocumentClient(_endPoint, _authKey);
                IDocumentQuery<T> query = client.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId),
                    new FeedOptions { MaxItemCount = -1 })
                    .Where(where)
                    .AsDocumentQuery();

                List<T> results = new List<T>();
                while (query.HasMoreResults)
                {
                    results.AddRange(await query.ExecuteNextAsync<T>());
                }
                if (results.Count == 0)
                    return false;
                else
                    return true;
            }
            catch (DocumentClientException e)
            {
                Log.Error("GetItemsAsync error: DocumentClientException: " + e.Error.Message);
                throw e;
            }
            catch (Exception e)
            {
                Log.Error("GetItemsAsync error: " + e.Message);
                throw e;
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> where, Expression<Func<T, T>> select)
        {
            DocumentClient client = null;
            try
            {
                client = new DocumentClient(_endPoint, _authKey);
                IDocumentQuery<T> query = client.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId),
                    new FeedOptions { MaxItemCount = -1 })
                    .Where(where)
                    .Select(select)
                    .AsDocumentQuery();

                List<T> results = new List<T>();
                while (query.HasMoreResults)
                {
                    results.AddRange(await query.ExecuteNextAsync<T>());
                }
                return results;
            }
            catch (DocumentClientException e)
            {
                Log.Error("GetItemsAsync error: DocumentClientException: " + e.Error.Message);
                return null;
            }
            catch (Exception e)
            {
                Log.Error("GetItemsAsync error: " + e.Message);
                return null;
            }
            finally
            {
                if (client != null)
                    client.Dispose();
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> where)
        {
            DocumentClient client = null;
            try
            {
                client = new DocumentClient(_endPoint, _authKey);
                IDocumentQuery<T> query = client.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId),
                    new FeedOptions { MaxItemCount = -1 })
                    .Where(where)
                    .AsDocumentQuery();

                List<T> results = new List<T>();
                while (query.HasMoreResults)
                {
                    results.AddRange(await query.ExecuteNextAsync<T>());
                }
                return results;
            }
            catch (DocumentClientException e)
            {
                Log.Error("GetItemsAsync error: DocumentClientException: " + e.Error.Message);
                return null;
            }
            catch (Exception e)
            {
                Log.Error("GetItemsAsync error: " + e.Message);
                return null;
            }
            finally
            {
                if (client != null)
                    client.Dispose();
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsync()
        {
            DocumentClient client = null;
            try
            {
                client = new DocumentClient(_endPoint, _authKey);
                IDocumentQuery<T> query = client.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId),
                    new FeedOptions { MaxItemCount = -1 })
                    .AsDocumentQuery();

                List<T> results = new List<T>();
                while (query.HasMoreResults)
                {
                    results.AddRange(await query.ExecuteNextAsync<T>());
                }
                return results;
            }
            catch (DocumentClientException e)
            {
                Log.Error("GetItemsAsync error: DocumentClientException: " + e.Error.Message);
                return null;
            }
            catch (Exception e)
            {
                Log.Error("GetItemsAsync error: " + e.Message);
                return null;
            }
            finally
            {
                if (client != null)
                    client.Dispose();
            }
        }

        public async Task<string> CreateItemAsync(T item)
        {
            DocumentClient client = null;
            try
            {
                client = new DocumentClient(_endPoint, _authKey);
                Document doc = await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId), item);
                return doc.Id;
            }
            catch (DocumentClientException e)
            {
                Log.Error("CreateItemAsync error: DocumentClientException: " + e.Error.Message);
                return string.Empty;
            }
            catch (Exception e)
            {
                Log.Error("CreateItemAsync error: " + e.Message);
                return string.Empty;
            }
            finally
            {
                if (client != null)
                    client.Dispose();
            }
        }

        public async Task<bool> UpdateItemAsync(string id, T item)
        {
            DocumentClient client = null;
            try
            {
                client = new DocumentClient(_endPoint, _authKey);
                Document doc = await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id), item);
                return true;
            }
            catch (DocumentClientException e)
            {
                Log.Error("UpdateItemAsync error: DocumentClientException: " + e.Error.Message);
                return false;
            }
            catch (Exception e)
            {
                Log.Error("UpdateItemAsync error: " + e.Message);
                return false;
            }
            finally
            {
                if (client != null)
                    client.Dispose();
            }
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            DocumentClient client = null;
            try
            {
                client = new DocumentClient(_endPoint, _authKey);
                await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id));
                return true;
            }
            catch (DocumentClientException e)
            {
                Log.Error("DeleteItemAsync error: DocumentClientException: " + e.Error.Message);
                return false;
            }
            catch (Exception e)
            {
                Log.Error("DeleteItemAsync error: " + e.Message);
                return false;
            }
            finally
            {
                if (client != null)
                    client.Dispose();
            }
        }

        private async Task CreateDatabaseIfNotExists()
        {
            DocumentClient client = null;
            try
            {
                client = new DocumentClient(_endPoint, _authKey);
                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(_databaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound && client != null)
                {
                    await client.CreateDatabaseAsync(new Database { Id = _databaseId });
                }
                else
                {
                    Log.Error(e.Message);
                    throw;
                }
            }
            finally
            {
                if (client != null)
                    client.Dispose();
            }
        }

        private async Task CreateCollectionIfNotExists()
        {
            DocumentClient client = null;
            try
            {
                client = new DocumentClient(_endPoint, _authKey);
                await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound && client != null)
                {
                    await client.CreateDocumentCollectionAsync(
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
            finally
            {
                if (client != null)
                    client.Dispose();
            }
        }
    }
}
