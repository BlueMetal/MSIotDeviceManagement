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
using Microsoft.Extensions.Options;
using MS.IoT.Common;

namespace MS.IoT.Repositories
{
    public class CosmosDBRepository<T> : CosmosDBBaseRepository, ICosmosDBRepository<T> where T : class
    {
        public CosmosDBRepository(string endpoint, string authkey, string databaseId, string collectionId) :
            base(endpoint, authkey, databaseId, collectionId)
        {
        }

        public CosmosDBRepository(IOptionsSnapshot<CosmosDbOptions> config) :
            this(config.Get(typeof(T).FullName).Endpoint,
                 config.Get(typeof(T).FullName).AuthKey,
                 config.Get(typeof(T).FullName).Database,
                 config.Get(typeof(T).FullName).Collection) 
        {
        }

        public async Task<T> GetItemAsync(string id)
        {
            try
            {
                Document document = await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id));
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
        }

        public async Task<bool> IsItemExistsByIdAsync(string Id)
        {
            try
            {
                Document document = await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, Id));
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
            try
            {
                IDocumentQuery<T> query = _client.CreateDocumentQuery<T>(
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
            try
            {
                IDocumentQuery<T> query = _client.CreateDocumentQuery<T>(
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
        }

        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> where)
        {
            try
            {
                IDocumentQuery<T> query = _client.CreateDocumentQuery<T>(
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
        }

        public async Task<IEnumerable<T>> GetItemsAsync()
        {
            try
            {
                IDocumentQuery<T> query = _client.CreateDocumentQuery<T>(
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
        }

        public async Task<string> CreateItemAsync(T item)
        {
            try
            {
                Document doc = await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId), item);
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
        }

        public async Task<bool> UpdateItemAsync(string id, T item)
        {
            try
            {
                Document doc = await _client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id), item);
                return true;
            }
            catch (DocumentClientException e)
            {
                Log.Error("UpdateItemAsync error: DocumentClientException: " + e.Error.Message);
                throw e;
            }
            catch (Exception e)
            {
                Log.Error("UpdateItemAsync error: " + e.Message);
                throw e;
            }
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            try
            {
                await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id));
                return true;
            }
            catch (DocumentClientException e)
            {
                Log.Error("DeleteItemAsync error: DocumentClientException: " + e.Error.Message);
                throw e;
            }
            catch (Exception e)
            {
                Log.Error("DeleteItemAsync error: " + e.Message);
                throw e;
            }
        }      
    }
}
