using System;
using System.Threading.Tasks;
using MS.IoT.MarketingPortal.Web.Models;
using MS.IoT.Domain.Interface;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web.Http;
using MS.IoT.Domain.Model;
using System.Web.Http.Description;
using System.Net.Http;
using MS.IoT.Repositories;
using MS.IoT.Common;
using System.IO;
using System.Net;
using MS.IoT.MarketingPortal.Web.Helpers;

namespace MS.IoT.MarketingPortal.Web.Controllers
{
    [RoutePrefix("api/commonTemplates")]
    [Authorize]
    public class TemplateApiController : BaseApiController
    {
        public TemplateApiController(IServicePrincipalRepository servicePrincipalRepo, IResourceManagerRepository resourceManagerRepo) : base(servicePrincipalRepo, resourceManagerRepo)
        {
        }

        [Route("generate")]
        [ResponseType(typeof(bool))]
        [HttpPost]
        public async Task<IHttpActionResult> GenerateCommonTemplates(CosmosDBInitModel cosmosInit)
        {
            try
            {
                Log.Information("Generate Cosmos DB Template: collection {endpoint},{key}",
                    cosmosInit.CosmosDBAccountEndPoint,cosmosInit.CosmosDBAccountKey);
             
                // create template collection
                ICosmosDBRepository<Template> templateRepository = new CosmosDBRepositorySetup<Template>(cosmosInit.CosmosDBAccountEndPoint, 
                    cosmosInit.CosmosDBAccountKey, AppConfig.ConfigurationItems.CosmosDbDatabaseName,
                    AppConfig.ConfigurationItems.CosmosDbCollectionNameTemplates);
                await templateRepository.Initialize();

                ICosmosDBRepository<Category> categoryRepository = new CosmosDBRepositorySetup<Category>(cosmosInit.CosmosDBAccountEndPoint, 
                    cosmosInit.CosmosDBAccountKey, AppConfig.ConfigurationItems.CosmosDbDatabaseName,
                    AppConfig.ConfigurationItems.CosmosDbCollectionNameTemplates);

                // create message collection
                ICosmosDBRepository<Template> templateMessagesRepository = new CosmosDBRepositorySetup<Template>
                    (cosmosInit.CosmosDBAccountEndPoint, cosmosInit.CosmosDBAccountKey,
                    AppConfig.ConfigurationItems.CosmosDbDatabaseName,
                    AppConfig.ConfigurationItems.CosmosDbCollectionNameMessages);
                await templateMessagesRepository.Initialize();

                // create settings collection
                ICosmosDBRepository<CustomGroupModel> devicesRepository = new CosmosDBRepositorySetup<CustomGroupModel>
                   (cosmosInit.CosmosDBAccountEndPoint, cosmosInit.CosmosDBAccountKey,
                   AppConfig.ConfigurationItems.CosmosDbDatabaseName,
                    AppConfig.ConfigurationItems.CosmosDbCollectionNameSettings);
                await devicesRepository.Initialize();

                var basePath = AppConfig.ConfigurationItems.CosmosDbTemplatesBaseUrl;

                using (WebClient client = new WebClient())
                {                 
                    Stream stream = client.OpenRead(basePath + "categories.json");
                    using (StreamReader r = new StreamReader(stream))
                    {
                        string jsonCategories = r.ReadToEnd();
                        Category[] categories = JsonConvert.DeserializeObject<Category[]>(jsonCategories);
                        //Adding templates
                        foreach (Category category in categories)
                            await CreateCategoryDocument(categoryRepository, category);
                    }
                    stream.Close();

                    stream = client.OpenRead(basePath + "templates.json");
                    using (StreamReader r = new StreamReader(stream))
                    {
                        string jsonTemplates = r.ReadToEnd();
                        Template[] templates = JsonConvert.DeserializeObject<Template[]>(jsonTemplates);

                        //Adding templates
                        foreach (Template template in templates)
                            await CreateTemplateDocument(templateRepository, template);
                    }
                    stream.Close();
                    Log.Information("Generate Cosmos DB Template completed");
                }
                return Ok(true);
            }
            catch (Exception e)
            {
                Log.Error("Generate Cosmos DB Template: exception {error}{exc}:", e.Message,e.InnerException);
                return Ok(false);
            }
        }

        private async Task CreateTemplateDocument(ICosmosDBRepository<Template> templateRepository, Template template)
        {
            Template templateObj = await templateRepository.GetItemAsync(template.Id);
            if (templateObj == null)
                await templateRepository.CreateItemAsync(template);
        }

        private async Task CreateCategoryDocument(ICosmosDBRepository<Category> categoryRepository, Category category)
        {
            Category categoryObj = await categoryRepository.GetItemAsync(category.Id);
            if (categoryObj == null)
                await categoryRepository.CreateItemAsync(category);
        }
    }
}