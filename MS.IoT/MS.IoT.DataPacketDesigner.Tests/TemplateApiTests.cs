using MS.IoT.DataPacketDesigner.Web.Controllers.API;
using MS.IoT.Domain.Interface;
using MS.IoT.Domain.Model;
using MS.IoT.Repositories;
using MS.IoT.DataPacketDesigner.Web;
using MS.IoT.DataPacketDesigner.Web.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Collections;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MS.IoT.DataPacketDesigner.Tests
{
    public class TemplateApiTests : IAsyncLifetime
    {
        public static readonly string tenantId = "72f43d57-b980-4152-b703-e2d8666a3ea9";
        public static readonly string clientId = "17073c60-a87e-42b7-a896-22123d500b1d";
        public static readonly string clientSecret = "oSdGtr786XS7Sdv3211U/uC7co7CUdGJmHu8BPIWytQ=";
        public static string managementAuthToken;
        public static readonly string managementUrl = "https://management.azure.com/";
        public static readonly string armTemplateUrl = "https://msiotsolutiondev.blob.core.windows.net/template/azuredeploy.json";

        public static readonly string endpoint = "https://cosmosdbnhebubkulhjwe.documents.azure.com:443/";
        public static readonly string authkey = "AN87gpLR9MZ2YlX5JqUYBAikwMj1vc6kCcEMNFtRfNI2RHPVNZ0gYfX5KhE4PX5p9Qwu9fdWMDlKlMI1WrVgXQ==";
        public static readonly string database = "MSIoT";
        public static readonly string colTemplate = "Templates";

        public static readonly string clientId2 = "17073c60-a87e-42b7-a896-22123d500b1d";
        public static readonly string appKey = "oSdGtr786XS7Sdv3211U/uC7co7CUdGJmHu8BPIWytQ=";
        public static readonly string AADInstance = "https://login.microsoftonline.com/";
        public static readonly string graphUri = "https://graph.windows.net";

        
        public async Task InitializeAsync()
        {
            string authContextURL = "https://login.microsoftonline.com/" + tenantId;
            // string authContextURL = "https://login.microsoftonline.com/common";
            var authenticationContext = new AuthenticationContext(authContextURL);
            var credential = new ClientCredential(clientId, clientSecret);
            var result = await authenticationContext.AcquireTokenAsync("https://management.azure.com/", credential);
            managementAuthToken = result.AccessToken;
        }

        [Fact(Skip = "skipped")]
        public async Task TestGetCommonTemplates()
        {
            ICosmosDBRepository<Template> _RepoTemplate = new CosmosDBRepository<Template>(endpoint, authkey, database, colTemplate);
            ICosmosDBRepository<Category> _RepoCategory = new CosmosDBRepository<Category>(endpoint, authkey, database, colTemplate);
            IUserProfileService _UserProfileProvider = new MockUserProfileService(graphUri, clientId2, appKey, AADInstance);
            ILogger<TemplateApiController> _logger = new LoggerFactory().CreateLogger<TemplateApiController>();

            TemplateApiController templateApiController = new TemplateApiController(_RepoTemplate, _RepoCategory, _UserProfileProvider, _logger);

            IActionResult actionResult = await templateApiController.GetCommonTemplates();
            
            actionResult.AssertOkValueType<IEnumerable<Template>>();
        }

        [Fact(Skip = "skipped")]
        public async Task TestGetUserTemplates()
        {
            ICosmosDBRepository<Template> _RepoTemplate = new CosmosDBRepository<Template>(endpoint, authkey, database, colTemplate);
            ICosmosDBRepository<Category> _RepoCategory = new CosmosDBRepository<Category>(endpoint, authkey, database, colTemplate);
            IUserProfileService _UserProfileProvider = new MockUserProfileService(graphUri, clientId2, appKey, AADInstance);
            ILogger<TemplateApiController> _logger = new LoggerFactory().CreateLogger<TemplateApiController>();

            TemplateApiController templateApiController = new TemplateApiController(_RepoTemplate, _RepoCategory, _UserProfileProvider, _logger);

            var actionResult = await templateApiController.GetUserTemplates();
            actionResult.AssertOkValueType<IEnumerable<Template>>();
        }

        [Fact(Skip = "skipped")]
        public async Task TestGetCategories()
        {
            ICosmosDBRepository<Template> _RepoTemplate = new CosmosDBRepository<Template>(endpoint, authkey, database, colTemplate);
            ICosmosDBRepository<Category> _RepoCategory = new CosmosDBRepository<Category>(endpoint, authkey, database, colTemplate);
            IUserProfileService _UserProfileProvider = new MockUserProfileService(graphUri, clientId2, appKey, AADInstance);
            ILogger<TemplateApiController> _logger = new LoggerFactory().CreateLogger<TemplateApiController>();

            TemplateApiController templateApiController = new TemplateApiController(_RepoTemplate, _RepoCategory, _UserProfileProvider, _logger);

            var actionResult = await templateApiController.GetCategories();
            actionResult.AssertOkValueType<IEnumerable<Category>>();
        }

        [Fact(Skip = "skipped")]
        public async Task TestGetTemplateById()
        {
            ICosmosDBRepository<Template> _RepoTemplate = new CosmosDBRepository<Template>(endpoint, authkey, database, colTemplate);
            ICosmosDBRepository<Category> _RepoCategory = new CosmosDBRepository<Category>(endpoint, authkey, database, colTemplate);
            IUserProfileService _UserProfileProvider = new MockUserProfileService(graphUri, clientId2, appKey, AADInstance);
            ILogger<TemplateApiController> _logger = new LoggerFactory().CreateLogger<TemplateApiController>();

            TemplateApiController templateApiController = new TemplateApiController(_RepoTemplate, _RepoCategory, _UserProfileProvider, _logger);

            var actionResult = await templateApiController.GetTemplateById("refrigerator_smart");
            actionResult.AssertOkValueType<Template>();

            var result = actionResult.OkayContent<Template>(); ;

            Assert.Equal("refrigerator_smart", result.Id);
            Assert.Equal(TemplateDocumentType.CommonTemplate, result.DocType);
        }

        [Fact(Skip = "skipped")]
        public async Task TestGetCurrentUser()
        {
            IUserProfileService _UserProfileProvider = new MockUserProfileService(graphUri, clientId2, appKey, AADInstance);
            ILogger<UserApiController> _logger1 = new LoggerFactory().CreateLogger<UserApiController>();

            UserApiController userApiController = new UserApiController(_UserProfileProvider, _logger1);

            var actionResult = await userApiController.GetCurrentUser();
            actionResult.AssertOkValueType<User>();

            var result = actionResult.OkayContent<User>(); ;

            Assert.Equal("JohnDoe@test.com", result.Id);
        }

        [Fact(Skip = "skipped")]
        public async Task TestAddTemplate()
        {
            ICosmosDBRepository<Template> _RepoTemplate = new CosmosDBRepository<Template>(endpoint, authkey, database, colTemplate);
            ICosmosDBRepository<Category> _RepoCategory = new CosmosDBRepository<Category>(endpoint, authkey, database, colTemplate);
            IUserProfileService _UserProfileProvider = new MockUserProfileService(graphUri, clientId2, appKey, AADInstance);
            ILogger<TemplateApiController> _logger = new LoggerFactory().CreateLogger<TemplateApiController>();
            ILogger<UserApiController> _logger1 = new LoggerFactory().CreateLogger<UserApiController>();

            TemplateApiController templateApiController = new TemplateApiController(_RepoTemplate, _RepoCategory, _UserProfileProvider, _logger);
            UserApiController userApiController = new UserApiController(_UserProfileProvider, _logger1);

            var actionUserResult = await userApiController.GetCurrentUser();
            var userResult = actionUserResult.OkayContent<User>();

            var actionResult = await templateApiController.CreateUserTemplate(new Template(){ Name = "TemplateTest" });
            actionResult.AssertOkValueType<string>();

            string templateId = actionResult.OkayContent<string>();

            Assert.NotNull(templateId);

            var actionResult2 = await templateApiController.GetTemplateById(templateId);
            actionResult2.AssertOkValueType<Template>();

            var result2 = actionResult2.OkayContent<Template>(); ;
            Assert.Equal(templateId, result2.Id);
            Assert.Equal(TemplateDocumentType.User, result2.DocType);
            Assert.Equal(userResult.Id.ToLower(), result2.UserId);
        }

        [Fact(Skip = "skipped")]
        public async Task TestEditCommonTemplate()
        {
            ICosmosDBRepository<Template> _RepoTemplate = new CosmosDBRepository<Template>(endpoint, authkey, database, colTemplate);
            ICosmosDBRepository<Category> _RepoCategory = new CosmosDBRepository<Category>(endpoint, authkey, database, colTemplate);
            IUserProfileService _UserProfileProvider = new MockUserProfileService(graphUri, clientId2, appKey, AADInstance);
            ILogger<TemplateApiController> _logger = new LoggerFactory().CreateLogger<TemplateApiController>();

            TemplateApiController templateApiController = new TemplateApiController(_RepoTemplate, _RepoCategory, _UserProfileProvider, _logger);

            var actionResult = await templateApiController.GetTemplateById("refrigerator_smart");
            actionResult.AssertOkValueType<Template>();
            
            var toEdit = actionResult.OkayContent<Template>(); 
            toEdit.Name = "New Name";

            var actionResult2 = await templateApiController.EditUserTemplate(toEdit); //Should fail because you can't edit a common template
            Assert.IsType<StatusCodeResult>(actionResult2);
        }

        [Fact(Skip = "skipped")]
        public async Task TestEditUserTemplate()
        {
            await TestAddTemplate();

            ICosmosDBRepository<Template> _RepoTemplate = new CosmosDBRepository<Template>(endpoint, authkey, database, colTemplate);
            ICosmosDBRepository<Category> _RepoCategory = new CosmosDBRepository<Category>(endpoint, authkey, database, colTemplate);
            IUserProfileService _UserProfileProvider = new MockUserProfileService(graphUri, clientId2, appKey, AADInstance);
            ILogger<TemplateApiController> _logger = new LoggerFactory().CreateLogger<TemplateApiController>();

            TemplateApiController templateApiController = new TemplateApiController(_RepoTemplate, _RepoCategory, _UserProfileProvider, _logger);

            var actionGetTemplatesResult = await templateApiController.GetUserTemplates();
            actionGetTemplatesResult.AssertOkValueType<IEnumerable<Template>>();
            
            var userTemplates = actionGetTemplatesResult.OkayContent<IEnumerable<Template>>(); 
            IEnumerator enumerator = userTemplates.GetEnumerator();
            enumerator.MoveNext();
            Template toEdit = (Template)enumerator.Current;

            toEdit.Name = "New Name";

            var actionResult = await templateApiController.EditUserTemplate(toEdit);
            actionResult.AssertOkValueType<bool>();

            var result = actionResult.OkayContent<bool>();
            Assert.True(result);
        }

        [Fact(Skip = "skipped")]
        public async Task TestDeleteUserTemplate()
        {
            ICosmosDBRepository<Template> _RepoTemplate = new CosmosDBRepository<Template>(endpoint, authkey, database, colTemplate);
            ICosmosDBRepository<Category> _RepoCategory = new CosmosDBRepository<Category>(endpoint, authkey, database, colTemplate);
            IUserProfileService _UserProfileProvider = new MockUserProfileService(graphUri, clientId2, appKey, AADInstance);
            ILogger<TemplateApiController> _logger = new LoggerFactory().CreateLogger<TemplateApiController>();

            TemplateApiController templateApiController = new TemplateApiController(_RepoTemplate, _RepoCategory, _UserProfileProvider, _logger);

            var actionGetTemplatesResult = await templateApiController.GetUserTemplates();
            actionGetTemplatesResult.AssertOkValueType<IEnumerable<Template>>();

            IEnumerable<Template> userTemplates = actionGetTemplatesResult.OkayContent<IEnumerable<Template>>();
            IEnumerator enumerator = userTemplates.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Template toDelete = (Template)enumerator.Current;

                var actionResult = await templateApiController.DeleteUserTemplate(toDelete.Id);
                actionResult.AssertOkValueType<bool>();

                var result = actionResult.OkayContent<bool>();
                Assert.True(result);
            }

            var actionGetTemplatesResult2 = await templateApiController.GetUserTemplates();
            actionGetTemplatesResult.AssertOkValueType<IEnumerable<Template>>();
            
            IEnumerable<Template> userTemplates2 = actionGetTemplatesResult2.OkayContent<IEnumerable<Template>>();
            IEnumerator enumerator2 = userTemplates2.GetEnumerator();
            Assert.False(enumerator2.MoveNext());
        }

        [Fact(Skip = "skipped")]
        public async Task TestDeleteCommonTemplate()
        {
            ICosmosDBRepository<Template> _RepoTemplate = new CosmosDBRepository<Template>(endpoint, authkey, database, colTemplate);
            ICosmosDBRepository<Category> _RepoCategory = new CosmosDBRepository<Category>(endpoint, authkey, database, colTemplate);
            IUserProfileService _UserProfileProvider = new MockUserProfileService(graphUri, clientId2, appKey, AADInstance);
            ILogger<TemplateApiController> _logger = new LoggerFactory().CreateLogger<TemplateApiController>();

            TemplateApiController templateApiController = new TemplateApiController(_RepoTemplate, _RepoCategory, _UserProfileProvider, _logger);

            var actionResult = await templateApiController.GetTemplateById("refrigerator_smart");
            actionResult.AssertOkValueType<Template>();
            
            var toDelete = actionResult.OkayContent<Template>();

            var actionResult2 = await templateApiController.DeleteUserTemplate(toDelete.Id); //Should fail because you can't delete a common template
            Assert.IsAssignableFrom<StatusCodeResult>(actionResult2);
        }

        
        public async Task DisposeAsync()
        {
            await TestDeleteUserTemplate();
        }
    }

    static class TestExtensions
    {
        public static T OkayContent<T>(this IActionResult actionResult)
        {
            return (T)((actionResult as OkObjectResult)?.Value);
        }

        public static void AssertOkValueType<T>(this IActionResult actionResult)
        {
            Assert.IsAssignableFrom<T>((actionResult as OkObjectResult)?.Value);
        }
    }
}
