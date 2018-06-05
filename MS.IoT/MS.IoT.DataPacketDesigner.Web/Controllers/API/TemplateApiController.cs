using MS.IoT.Common;
using MS.IoT.DataPacketDesigner.Web.Helpers;
using MS.IoT.Domain.Interface;
using MS.IoT.Domain.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace MS.IoT.DataPacketDesigner.Web.Controllers.API
{
    /// <summary>
    /// Template Controller API
    /// </summary>
    [Authorize]
    [RoutePrefix("api/templates")]
    public class TemplateApiController : BaseApiController
    {
        //Service members
        protected readonly ICosmosDBRepository<Template> _templateRepo;
        protected readonly ICosmosDBRepository<Category> _categoryRepo;

        /// <summary>
        /// Main Constructor
        /// </summary>
        /// <param name="templateRepo">Repository Template Service</param>
        /// <param name="categoryRepo">Repository Category Service</param>
        /// <param name="userProfile">User Profile Service</param>
        public TemplateApiController(ICosmosDBRepository<Template> templateRepo, 
            ICosmosDBRepository<Category> categoryRepo, IUserProfileService userProfile)
            : base(userProfile)
        {
            _templateRepo = templateRepo;
            _categoryRepo = categoryRepo;
        }

        /// <summary>
        /// Endpoint to retrieve the list of current templates
        /// </summary>
        /// <returns>List of Templates</returns>
        [Route("common")]
        [ResponseType(typeof(IEnumerable<Template>))]
        [HttpGet]
        public async Task<IHttpActionResult> GetCommonTemplates()
        {
            Domain.Model.User currentUser = await EnsureCurrentUser();
            string userId = currentUser.Id.ToLower();

            IEnumerable<Template> commonTemplates = await _templateRepo.GetItemsAsync(
                p => p.DocType == TemplateDocumentType.CommonTemplate ||
                (p.IsReusableTemplate && p.UserId.ToLower() == userId), p => new Template()
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CategoryId = p.CategoryId,
                SubcategoryId = p.SubcategoryId,
                IsReusableTemplate = p.IsReusableTemplate
                }); //Force documentdb to query only these fields
            if (commonTemplates == null)
            {
                Log.Error("API GetCommonTemplates error: CommonTemplates not found.");
                return StatusCode(HttpStatusCode.InternalServerError);
            }
            return Ok(commonTemplates);
        }

        /// <summary>
        /// Endpoint to retrieve the list of user templates by on the current user
        /// </summary>
        /// <returns>List of Templates</returns>
        [Route("usertemplates")]
        [ResponseType(typeof(IEnumerable<Template>))]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserTemplates()
        {
            Domain.Model.User currentUser = await EnsureCurrentUser();
            string userId = currentUser.Id.ToLower();

            IEnumerable<Template> userTemplates = await _templateRepo.GetItemsAsync(
                p => p.DocType == TemplateDocumentType.User &&
                p.UserId.ToLower() == userId, p => new Template()
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CategoryId = p.CategoryId,
                SubcategoryId = p.SubcategoryId,
                IsReusableTemplate = p.IsReusableTemplate
                }); //Force documentdb to query only these fields
            if (userTemplates == null)
            {
                Log.Error("API GetUserTemplates error: User Templates not found. User {UserId}", userId);
                return StatusCode(HttpStatusCode.InternalServerError);
            }
            return Ok(userTemplates);
        }

        /// <summary>
        /// Endpoint to retrieve the list of categories
        /// </summary>
        /// <returns>List of Categories</returns>
        [Route("categories")]
        [ResponseType(typeof(IEnumerable<Category>))]
        [HttpGet]
        public async Task<IHttpActionResult> GetCategories()
        {
            IEnumerable<Category> categories = await _categoryRepo.GetItemsAsync(p => p.DocType == TemplateDocumentType.Category);
            if (categories == null)
            {
                Log.Error("API GetCategories error: Categories not found.");
                return StatusCode(HttpStatusCode.InternalServerError);
            }
            return Ok(categories);
        }

        /// <summary>
        /// Endpoint to retrieve a template by ID
        /// If the template is a user template, a test will be done to ensure that the current user is allowed to see it
        /// </summary>
        /// <param name="templateId">Template ID</param>
        /// <returns>Template</returns>
        [Route("templates/{templateId}")]
        [ResponseType(typeof(Template))]
        [HttpGet]
        public async Task<IHttpActionResult> GetTemplateById(string templateId)
        {
            Template template = await _templateRepo.GetItemAsync(templateId);

            //Not found
            if (template == null)
            {
                Log.Error("API GetTemplateById error: Template with id {TemplateId} not found.", templateId);
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            //If not a template, return error
            if(template.DocType != TemplateDocumentType.User && template.DocType != TemplateDocumentType.CommonTemplate)
            {
                Log.Error("API GetTemplateById error: {TemplateId} is not a template.", templateId);
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            //If user template, check user
            if (template.DocType == TemplateDocumentType.User)
            {
                Domain.Model.User currentUser = await EnsureCurrentUser();
                string userId = currentUser.Id.ToLower();

                //If user doesn't match, return error
                if (template.UserId.ToLower() != userId)
                {
                    Log.Error("API GetTemplateById error: Template with id {TemplateId} was found but the user {UserId} does not match.", templateId, userId);
                    return StatusCode(HttpStatusCode.InternalServerError);
                }
            }

            return Ok(template);
        }

        /// <summary>
        /// Endpoint to create a user template
        /// </summary>
        /// <param name="template">New Template</param>
        /// <returns>Template ID</returns>
        [Route("usertemplates")]
        [ResponseType(typeof(string))]
        [HttpPost]
        public async Task<IHttpActionResult> CreateUserTemplate([FromBody] Template template)
        {
            Domain.Model.User currentUser = await EnsureCurrentUser();
            string userId = currentUser.Id.ToLower();

            //Ensure user values and unique ID are set server side.
            template.BaseTemplateId = template.Id;
            template.Id = Guid.NewGuid().ToString();
            template.DocType = TemplateDocumentType.User;
            template.UserId = userId;
            template.CreationDate = DateTime.Now;
            template.ModifiedDate = DateTime.Now;

            //Create new user template
            string newId = await _templateRepo.CreateItemAsync(template);
            if (string.IsNullOrEmpty(newId))
            {
                Log.Error("API CreateUserTemplate error: There was an error while creating the template {TemplateName}", template.Name);
                return StatusCode(HttpStatusCode.InternalServerError);
            }
            return Ok(newId);
        }

        /// <summary>
        /// Endpoint to edit a user template
        /// The method will ensure that the user is allowed to edit his own template
        /// The method will ensure that the template is not a common template
        /// </summary>
        /// <param name="template">Template object</param>
        /// <returns>True if success</returns>
        [Route("usertemplates")]
        [ResponseType(typeof(bool))]
        [HttpPut]
        public async Task<IHttpActionResult> EditUserTemplate([FromBody] Template template)
        {
            Domain.Model.User currentUser = await EnsureCurrentUser();
            string userId = currentUser.Id.ToLower();

            //Ensure user values and unique ID are set server side.
            template.DocType = TemplateDocumentType.User;
            template.UserId = userId;
            template.ModifiedDate = DateTime.Now;

            //Ensure the current user is allowed to edit this template
            Template userTemplate = await _templateRepo.GetItemAsync(template.Id);
            if (userTemplate == null || userTemplate.UserId == null || userTemplate.UserId.ToLower() != userId)
            {
                Log.Error("API EditUserTemplate error: Template with id {TemplateId} was found but the user {UserId} does not match.", template.Id, userId);
                return StatusCode(HttpStatusCode.InternalServerError);
            }
            if (userTemplate.DocType != TemplateDocumentType.User)
            {
                Log.Error("API EditUserTemplate error: Template with id {TemplateId} is not a user template.", template.Id, userId);
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            //Create new user template
            bool status = await _templateRepo.UpdateItemAsync(template.Id, template);
            if (!status)
            {
                Log.Error("API EditUserTemplate error: There was an error while creating the template {TemplateId}", template.Id);
                return StatusCode(HttpStatusCode.InternalServerError);
            }
            return Ok(status);
        }

        /// <summary>
        /// Endpoint to delete a user template
        /// The method will ensure that the user is allowed to delete his own template
        /// The method will ensure that the template is not a common template
        /// </summary>
        /// <param name="templateId">Template ID</param>
        /// <returns>True if success</returns>
        [Route("usertemplates/{templateId}")]
        [ResponseType(typeof(bool))]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteUserTemplate(string templateId)
        {
            Domain.Model.User currentUser = await EnsureCurrentUser();
            string userId = currentUser.Id.ToLower();

            //Ensure the current user is allowed to edit this template
            Template userTemplate = await _templateRepo.GetItemAsync(templateId);
            if (userTemplate == null || userTemplate.UserId == null || userTemplate.UserId.ToLower() != userId)
            {
                Log.Error("API DeleteUserTemplate error: Template with id {TemplateId} was found but the user {UserId} does not match.", templateId, userId);
                return StatusCode(HttpStatusCode.InternalServerError);
            }
            if (userTemplate.DocType != TemplateDocumentType.User)
            {
                Log.Error("API DeleteUserTemplate error: Template with id {TemplateId} is not a user template.", templateId, userId);
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            //Delete user template
            bool status = await _templateRepo.DeleteItemAsync(templateId);
            if (!status)
            {
                Log.Error("API DeleteUserTemplate error: There was an error while deleting the template {TemplateId}", templateId);
                return StatusCode(HttpStatusCode.InternalServerError);
            }
            return Ok(status);
        }

        /*[Route("generate")]
        [ResponseType(typeof(bool))]
        [HttpGet]
        public async Task<IHttpActionResult> GenerateCommonTemplates()
        {
            try
            {
                string dbEndpoint = "https://thibaultbuquet.documents.azure.com:443/";
                string dbAuthKey = "HgMWnDxCUN96CNdjLvhsN3PfMmirrB85ns2VlNgNBkEY8TtMDCFYHDXH6JYyEN8Rwwn3POoucwQ1eetAcr6gXQ==";
                string dbDatabase = "MSIoT";
                string dbCollection = "Templates";
                IRepository<Template> templateRepository = new Repository<Template>(dbEndpoint, dbAuthKey, dbDatabase, dbCollection);
                IRepository<Category> categoryRepository = new Repository<Category>(dbEndpoint, dbAuthKey, dbDatabase, dbCollection);

                await templateRepository.Initialize();

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority);

                    //Parsing
                    string jsonCategories = await httpClient.GetStringAsync("Content/templates/categories.json");
                    Category[] categories = JsonConvert.DeserializeObject<Category[]>(jsonCategories);

                    //Adding templates
                    foreach (Category category in categories)
                        await CreateCategoryDocument(categoryRepository, category);

                    //Parsing
                    string jsonTemplates = await httpClient.GetStringAsync("Content/templates/templates.json");
                    Template[] templates = JsonConvert.DeserializeObject<Template[]>(jsonTemplates);

                    //Adding templates
                    foreach (Template template in templates)
                        await CreateTemplateDocument(templateRepository, template);
                }

                return Ok(true);
            }
            catch (Exception e)
            {
                //TODO Log error
                return Ok(false);
            }
        }

        private async Task CreateTemplateDocument(IRepository<Template> templateRepository, Template template)
        {
            Template templateObj = await templateRepository.GetItemAsync(template.Id);
            if (templateObj == null)
                await templateRepository.CreateItemAsync(template);
        }

        private async Task CreateCategoryDocument(IRepository<Category> categoryRepository, Category category)
        {
            Category categoryObj = await categoryRepository.GetItemAsync(category.Id);
            if (categoryObj == null)
                await categoryRepository.CreateItemAsync(category);
        }*/
    }
}