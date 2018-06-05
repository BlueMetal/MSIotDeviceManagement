using MS.IoT.DataPacketDesigner.Web.Controllers.API;
using MS.IoT.Domain.Interface;
using MS.IoT.Domain.Model;
using MS.IoT.Repositories;
using MS.IoT.DataPacketDesigner.Web;
using MS.IoT.DataPacketDesigner.Web.Helpers;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Collections;

namespace MS.IoT.DataPacketDesigner.Tests
{
    [TestClass]
    public class TemplateApiTests
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

        [TestInitialize]
        public async Task SetupAsync()
        {
            string authContextURL = "https://login.microsoftonline.com/" + tenantId;
            // string authContextURL = "https://login.microsoftonline.com/common";
            var authenticationContext = new AuthenticationContext(authContextURL);
            var credential = new ClientCredential(clientId, clientSecret);
            var result = await authenticationContext.AcquireTokenAsync("https://management.azure.com/", credential);
            managementAuthToken = result.AccessToken;
        }

        [TestMethod]
        [Ignore]
        public async Task TestGetCommonTemplates()
        {
            ICosmosDBRepository<Template> _RepoTemplate = new CosmosDBRepository<Template>(endpoint, authkey, database, colTemplate);
            ICosmosDBRepository<Category> _RepoCategory = new CosmosDBRepository<Category>(endpoint, authkey, database, colTemplate);
            IUserProfileService _UserProfileProvider = new MockUserProfileService(graphUri, clientId2, appKey, AADInstance);

            TemplateApiController templateApiController = new TemplateApiController(_RepoTemplate, _RepoCategory, _UserProfileProvider);

            IHttpActionResult actionResult = await templateApiController.GetCommonTemplates();
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<IEnumerable<Template>>));
        }

        [TestMethod]
        [Ignore]
        public async Task TestGetUserTemplates()
        {
            ICosmosDBRepository<Template> _RepoTemplate = new CosmosDBRepository<Template>(endpoint, authkey, database, colTemplate);
            ICosmosDBRepository<Category> _RepoCategory = new CosmosDBRepository<Category>(endpoint, authkey, database, colTemplate);
            IUserProfileService _UserProfileProvider = new MockUserProfileService(graphUri, clientId2, appKey, AADInstance);

            TemplateApiController templateApiController = new TemplateApiController(_RepoTemplate, _RepoCategory, _UserProfileProvider);

            IHttpActionResult actionResult = await templateApiController.GetUserTemplates();
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<IEnumerable<Template>>));
        }

        [TestMethod]
        [Ignore]
        public async Task TestGetCategories()
        {
            ICosmosDBRepository<Template> _RepoTemplate = new CosmosDBRepository<Template>(endpoint, authkey, database, colTemplate);
            ICosmosDBRepository<Category> _RepoCategory = new CosmosDBRepository<Category>(endpoint, authkey, database, colTemplate);
            IUserProfileService _UserProfileProvider = new MockUserProfileService(graphUri, clientId2, appKey, AADInstance);

            TemplateApiController templateApiController = new TemplateApiController(_RepoTemplate, _RepoCategory, _UserProfileProvider);

            IHttpActionResult actionResult = await templateApiController.GetCategories();
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<IEnumerable<Category>>));
        }

        [TestMethod]
        [Ignore]
        public async Task TestGetTemplateById()
        {
            ICosmosDBRepository<Template> _RepoTemplate = new CosmosDBRepository<Template>(endpoint, authkey, database, colTemplate);
            ICosmosDBRepository<Category> _RepoCategory = new CosmosDBRepository<Category>(endpoint, authkey, database, colTemplate);
            IUserProfileService _UserProfileProvider = new MockUserProfileService(graphUri, clientId2, appKey, AADInstance);

            TemplateApiController templateApiController = new TemplateApiController(_RepoTemplate, _RepoCategory, _UserProfileProvider);

            IHttpActionResult actionResult = await templateApiController.GetTemplateById("refrigerator_smart");
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<Template>));

            OkNegotiatedContentResult<Template> result = (OkNegotiatedContentResult<Template>)actionResult;

            Assert.AreEqual("refrigerator_smart", result.Content.Id);
            Assert.AreEqual(TemplateDocumentType.CommonTemplate, result.Content.DocType);
        }

        [TestMethod]
        [Ignore]
        public async Task TestGetCurrentUser()
        {
            IUserProfileService _UserProfileProvider = new MockUserProfileService(graphUri, clientId2, appKey, AADInstance);

            UserApiController userApiController = new UserApiController(_UserProfileProvider);

            IHttpActionResult actionResult = await userApiController.GetCurrentUser();
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<User>));

            OkNegotiatedContentResult<User> result = (OkNegotiatedContentResult<User>)actionResult;

            Assert.AreEqual("JohnDoe@test.com", result.Content.Id);
        }

        [TestMethod]
        [Ignore]
        public async Task TestAddTemplate()
        {
            ICosmosDBRepository<Template> _RepoTemplate = new CosmosDBRepository<Template>(endpoint, authkey, database, colTemplate);
            ICosmosDBRepository<Category> _RepoCategory = new CosmosDBRepository<Category>(endpoint, authkey, database, colTemplate);
            IUserProfileService _UserProfileProvider = new MockUserProfileService(graphUri, clientId2, appKey, AADInstance);

            TemplateApiController templateApiController = new TemplateApiController(_RepoTemplate, _RepoCategory, _UserProfileProvider);
            UserApiController userApiController = new UserApiController(_UserProfileProvider);

            IHttpActionResult actionUserResult = await userApiController.GetCurrentUser();
            OkNegotiatedContentResult<User> userResult = (OkNegotiatedContentResult<User>)actionUserResult;

            IHttpActionResult actionResult = await templateApiController.CreateUserTemplate(new Template(){ Name = "TemplateTest" });
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<string>));

            OkNegotiatedContentResult<string> result = (OkNegotiatedContentResult<string>)actionResult;
            string templateId = result.Content;

            Assert.IsNotNull(templateId);

            IHttpActionResult actionResult2 = await templateApiController.GetTemplateById(templateId);
            Assert.IsInstanceOfType(actionResult2, typeof(OkNegotiatedContentResult<Template>));

            OkNegotiatedContentResult<Template> result2 = (OkNegotiatedContentResult<Template>)actionResult2;
            Assert.AreEqual(templateId, result2.Content.Id);
            Assert.AreEqual(TemplateDocumentType.User, result2.Content.DocType);
            Assert.AreEqual(userResult.Content.Id.ToLower(), result2.Content.UserId);
        }

        [TestMethod]
        [Ignore]
        public async Task TestEditCommonTemplate()
        {
            ICosmosDBRepository<Template> _RepoTemplate = new CosmosDBRepository<Template>(endpoint, authkey, database, colTemplate);
            ICosmosDBRepository<Category> _RepoCategory = new CosmosDBRepository<Category>(endpoint, authkey, database, colTemplate);
            IUserProfileService _UserProfileProvider = new MockUserProfileService(graphUri, clientId2, appKey, AADInstance);

            TemplateApiController templateApiController = new TemplateApiController(_RepoTemplate, _RepoCategory, _UserProfileProvider);

            IHttpActionResult actionResult = await templateApiController.GetTemplateById("refrigerator_smart");
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<Template>));

            OkNegotiatedContentResult<Template> result = (OkNegotiatedContentResult<Template>)actionResult;

            Template toEdit = result.Content;
            toEdit.Name = "New Name";

            IHttpActionResult actionResult2 = await templateApiController.EditUserTemplate(toEdit); //Should fail because you can't edit a common template
            Assert.IsInstanceOfType(actionResult2, typeof(StatusCodeResult));
        }

        [TestMethod]
        [Ignore]
        public async Task TestEditUserTemplate()
        {
            await TestAddTemplate();

            ICosmosDBRepository<Template> _RepoTemplate = new CosmosDBRepository<Template>(endpoint, authkey, database, colTemplate);
            ICosmosDBRepository<Category> _RepoCategory = new CosmosDBRepository<Category>(endpoint, authkey, database, colTemplate);
            IUserProfileService _UserProfileProvider = new MockUserProfileService(graphUri, clientId2, appKey, AADInstance);

            TemplateApiController templateApiController = new TemplateApiController(_RepoTemplate, _RepoCategory, _UserProfileProvider);

            IHttpActionResult actionGetTemplatesResult = await templateApiController.GetUserTemplates();
            Assert.IsInstanceOfType(actionGetTemplatesResult, typeof(OkNegotiatedContentResult<IEnumerable<Template>>));

            OkNegotiatedContentResult<IEnumerable<Template>> userTemplatesResult = (OkNegotiatedContentResult<IEnumerable<Template>>)actionGetTemplatesResult;
            IEnumerable<Template> userTemplates = userTemplatesResult.Content;
            IEnumerator enumerator = userTemplates.GetEnumerator();
            enumerator.MoveNext();
            Template toEdit = (Template)enumerator.Current;

            toEdit.Name = "New Name";

            IHttpActionResult actionResult = await templateApiController.EditUserTemplate(toEdit);
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<bool>));

            OkNegotiatedContentResult<bool> result = (OkNegotiatedContentResult<bool>)actionResult;
            Assert.AreEqual(true, result.Content);
        }

        [TestMethod]
        [Ignore]
        public async Task TestDeleteUserTemplate()
        {
            ICosmosDBRepository<Template> _RepoTemplate = new CosmosDBRepository<Template>(endpoint, authkey, database, colTemplate);
            ICosmosDBRepository<Category> _RepoCategory = new CosmosDBRepository<Category>(endpoint, authkey, database, colTemplate);
            IUserProfileService _UserProfileProvider = new MockUserProfileService(graphUri, clientId2, appKey, AADInstance);

            TemplateApiController templateApiController = new TemplateApiController(_RepoTemplate, _RepoCategory, _UserProfileProvider);

            IHttpActionResult actionGetTemplatesResult = await templateApiController.GetUserTemplates();
            Assert.IsInstanceOfType(actionGetTemplatesResult, typeof(OkNegotiatedContentResult<IEnumerable<Template>>));

            OkNegotiatedContentResult<IEnumerable<Template>> userTemplatesResult = (OkNegotiatedContentResult<IEnumerable<Template>>)actionGetTemplatesResult;
            IEnumerable<Template> userTemplates = userTemplatesResult.Content;
            IEnumerator enumerator = userTemplates.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Template toDelete = (Template)enumerator.Current;

                IHttpActionResult actionResult = await templateApiController.DeleteUserTemplate(toDelete.Id);
                Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<bool>));

                OkNegotiatedContentResult<bool> result = (OkNegotiatedContentResult<bool>)actionResult;
                Assert.AreEqual(true, result.Content);
            }

            IHttpActionResult actionGetTemplatesResult2 = await templateApiController.GetUserTemplates();
            Assert.IsInstanceOfType(actionGetTemplatesResult2, typeof(OkNegotiatedContentResult<IEnumerable<Template>>));
            OkNegotiatedContentResult<IEnumerable<Template>> userTemplatesResult2 = (OkNegotiatedContentResult<IEnumerable<Template>>)actionGetTemplatesResult2;
            IEnumerable<Template> userTemplates2 = userTemplatesResult2.Content;
            IEnumerator enumerator2 = userTemplates2.GetEnumerator();
            Assert.AreEqual(false, enumerator2.MoveNext());
        }

        [TestMethod]
        [Ignore]
        public async Task TestDeleteCommonTemplate()
        {
            ICosmosDBRepository<Template> _RepoTemplate = new CosmosDBRepository<Template>(endpoint, authkey, database, colTemplate);
            ICosmosDBRepository<Category> _RepoCategory = new CosmosDBRepository<Category>(endpoint, authkey, database, colTemplate);
            IUserProfileService _UserProfileProvider = new MockUserProfileService(graphUri, clientId2, appKey, AADInstance);

            TemplateApiController templateApiController = new TemplateApiController(_RepoTemplate, _RepoCategory, _UserProfileProvider);

            IHttpActionResult actionResult = await templateApiController.GetTemplateById("refrigerator_smart");
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<Template>));

            OkNegotiatedContentResult<Template> result = (OkNegotiatedContentResult<Template>)actionResult;

            Template toDelete = result.Content;

            IHttpActionResult actionResult2 = await templateApiController.DeleteUserTemplate(toDelete.Id); //Should fail because you can't delete a common template
            Assert.IsInstanceOfType(actionResult2, typeof(StatusCodeResult));
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await TestDeleteUserTemplate();
        }
    }
}
