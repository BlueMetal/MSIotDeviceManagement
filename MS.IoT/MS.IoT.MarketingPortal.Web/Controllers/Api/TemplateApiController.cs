using System;
using System.Threading.Tasks;
using MS.IoT.MarketingPortal.Web.Models;
using MS.IoT.Domain.Interface;
using Newtonsoft.Json;
using MS.IoT.Domain.Model;
using MS.IoT.Repositories;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MS.IoT.MarketingPortal.Web.Controllers
{
    [Route("api/commonTemplates")]
    [Authorize]
    public class TemplateApiController : BaseApiController
    {
        private readonly CosmosDbOptions cosmosDbOptions;
        private readonly ILogger<TemplateApiController> logger;

        public TemplateApiController(IServicePrincipalRepository servicePrincipalRepo,
                                     IResourceManagerRepository resourceManagerRepo,
                                     IOptions<CosmosDbOptions> cosmosDbOptions,
                                     ILogger<TemplateApiController> logger) 
            : base(servicePrincipalRepo, resourceManagerRepo)
        {
            this.cosmosDbOptions = cosmosDbOptions.Value;
            this.logger = logger;
        }

        [Route("generate")]
        [Produces(typeof(bool))]
        [HttpPost]
        public async Task<IActionResult> GenerateCommonTemplates([FromBody]CosmosDBInitModel cosmosInit)
        {
            try
            {
                logger.LogInformation("Generate Cosmos DB Template: collection {endpoint},{key}",
                    cosmosInit.CosmosDBAccountEndPoint,cosmosInit.CosmosDBAccountKey);
             
                // create template collection
                ICosmosDBRepository<Template> templateRepository =
                    new CosmosDBRepositorySetup<Template>(cosmosInit.CosmosDBAccountEndPoint, 
                                                          cosmosInit.CosmosDBAccountKey, 
                                                          cosmosDbOptions.Database,
                                                          cosmosDbOptions.Collections.Templates);
                await templateRepository.Initialize();

                ICosmosDBRepository<Category> categoryRepository = 
                    new CosmosDBRepositorySetup<Category>(cosmosInit.CosmosDBAccountEndPoint, 
                                                          cosmosInit.CosmosDBAccountKey, 
                                                          cosmosDbOptions.Database,
                                                          cosmosDbOptions.Collections.Templates);

                // create message collection
                ICosmosDBRepository<Template> templateMessagesRepository = 
                    new CosmosDBRepositorySetup<Template>(cosmosInit.CosmosDBAccountEndPoint, 
                                                          cosmosInit.CosmosDBAccountKey,
                                                          cosmosDbOptions.Database,
                                                          cosmosDbOptions.Collections.Messages);
                await templateMessagesRepository.Initialize();

                // create settings collection
                ICosmosDBRepository<CustomGroupModel> devicesRepository = 
                    new CosmosDBRepositorySetup<CustomGroupModel>(cosmosInit.CosmosDBAccountEndPoint, 
                                                                  cosmosInit.CosmosDBAccountKey,
                                                                  cosmosDbOptions.Database,
                                                                  cosmosDbOptions.Collections.Groups);
                await devicesRepository.Initialize();

                var basePath = cosmosDbOptions.TemplatesBase;

                using (var client = new HttpClient())
                {                 
                    Stream stream = await client.GetStreamAsync(basePath + "categories.json");
                    using (StreamReader r = new StreamReader(stream))
                    {
                        string jsonCategories = r.ReadToEnd();
                        Category[] categories = JsonConvert.DeserializeObject<Category[]>(jsonCategories);
                        //Adding templates
                        foreach (Category category in categories)
                            await CreateCategoryDocument(categoryRepository, category);
                    }
                    stream.Close();

                    stream = await client.GetStreamAsync(basePath + "templates.json");
                    using (StreamReader r = new StreamReader(stream))
                    {
                        string jsonTemplates = r.ReadToEnd();
                        Template[] templates = JsonConvert.DeserializeObject<Template[]>(jsonTemplates);

                        //Adding templates
                        foreach (Template template in templates)
                            await CreateTemplateDocument(templateRepository, template);
                    }
                    stream.Close();
                    logger.LogInformation("Generate Cosmos DB Template completed");
                }
                return Ok(true);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Generate Cosmos DB Template: exception {error}{exc}:", e.Message,e.InnerException);
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