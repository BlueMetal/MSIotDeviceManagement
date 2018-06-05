using System;
using System.Collections.Generic;
using System.Web;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using MS.IoT.Common;
using MS.IoT.Domain.Interface;
using MS.IoT.DeviceManagementPortal.Web.Helpers;
using System.Web.Http.Description;
using MS.IoT.Domain.Model;

namespace MS.IoT.DeviceManagementPortal.Web.Controllers.Api
{

    [RoutePrefix("api/groups")]
    [Authorize]
    public class GroupsCosmosDBApiController : BaseApiController
    {
        private ICosmosDBRepository<CustomGroupModel> _GroupsRepo;
        private readonly IDeviceDBService _DeviceServiceDB;
        public HttpContext HttpContext { get; set; }

        public GroupsCosmosDBApiController(IUserProfileService userProfile, ICosmosDBRepository<CustomGroupModel> groupsRepo, IDeviceDBService deviceServiceDB) 
            : base(userProfile)
        {
            _GroupsRepo = groupsRepo;
            _DeviceServiceDB = deviceServiceDB;
        }

        [Route("groups")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<CustomGroupModel>))]
        public async Task<IHttpActionResult> GetCustomGroups()
        {
            try
            {
                Log.Information("Get custom groups called");

                var customGroups = await _GroupsRepo.GetItemsAsync();

                return Ok(customGroups);
            }
            catch (Exception e)
            {
                Log.Error("Get custom groups - Exception: {message}", e.Message);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        [Route("group/{customGroupId}")]
        [HttpGet]
        [ResponseType(typeof(CustomGroupModel))]
        public async Task<IHttpActionResult> GetCustomGroupById(string customGroupId)
        {
            try
            {
                Log.Information("Get custom group called");

                var device = await _GroupsRepo.GetItemAsync(customGroupId);
                return Ok(device);
            }
            catch (Exception e)
            {
                Log.Error("Get custom group - Exception: {message}", e.Message);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        /// <summary>
        /// Endpoint to create a custom group
        /// </summary>
        /// <param name="customGroup">CustomGroup object</param>
        /// <returns>CustomGroup ID</returns>
        [Route("groups")]
        [ResponseType(typeof(string))]
        [HttpPost]
        public async Task<IHttpActionResult> CreateCustomGroup([FromBody] CustomGroupModel customGroup)
        {
            Log.Information("Create custom group called");

            //New Guid
            customGroup.Id = Guid.NewGuid().ToString();
            customGroup.Count = 0;

            //Get number of custom groups
            IEnumerable<CustomGroupModel> customGroupsEnum = await _GroupsRepo.GetItemsAsync();
            List<CustomGroupModel> customGroups = new List<CustomGroupModel>();
            IEnumerator<CustomGroupModel> enumator = customGroupsEnum.GetEnumerator();
            while (enumator.MoveNext())
                customGroups.Add(enumator.Current);

            customGroup.Order = customGroups.Count;

            //Create new user template
            string newId = await _GroupsRepo.CreateItemAsync(customGroup);
            if (string.IsNullOrEmpty(newId))
            {
                Log.Error("Create custom group - Exception: {0}", customGroup.Name);
                return StatusCode(HttpStatusCode.InternalServerError);
            }
            return Ok(newId);
        }

        /// <summary>
        /// Endpoint to edit a custom group
        /// </summary>
        /// <param name="customGroup">CustomGroup object</param>
        /// <returns>True if success</returns>
        [Route("groups")]
        [ResponseType(typeof(bool))]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateCustomGroup([FromBody] CustomGroupModel customGroup)
        {
            Log.Information("Update custom group called");

            //Ensure the current user is allowed to edit this template
            var customGroupItem = await _GroupsRepo.GetItemAsync(customGroup.Id);
            if (customGroupItem == null)
            {
                Log.Error("Update custom group - Exception: Custom group with id {0} was not found.", customGroup.Id);
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            var groupDetails = _DeviceServiceDB.GetDevicesTwinInfoAsync(new DeviceQueryConfiguration()
            {
                ItemsPerPage = 1,
                Where = customGroup.Where
            });
            customGroup.Count = groupDetails.ItemsCount;

            //Update Custom Group
            bool status = await _GroupsRepo.UpdateItemAsync(customGroup.Id, customGroup);
            if (!status)
            {
                Log.Error("Update custom group - Exception: There was an error while updating the custom group {0}", customGroup.Id);
                return StatusCode(HttpStatusCode.InternalServerError);
            }
            return Ok(status);
        }

        /// <summary>
        /// Endpoint to reorder custom groups
        /// </summary>
        /// <param name="listCustomGroupsIds">List of CustomGroup object</param>
        /// <returns>True if success</returns>
        [Route("groups/reorder")]
        [ResponseType(typeof(bool))]
        [HttpPost]
        public async Task<IHttpActionResult> ReorderCustomGroups([FromBody] string[] listCustomGroupsIds)
        {
            Log.Information("Reorder custom group called");

            bool status = false;
            int i = 0;

            IEnumerable<CustomGroupModel> customGroupsEnum = await _GroupsRepo.GetItemsAsync();
            List<CustomGroupModel> customGroups = new List<CustomGroupModel>();
            IEnumerator<CustomGroupModel> enumator = customGroupsEnum.GetEnumerator();
            while (enumator.MoveNext())
                customGroups.Add(enumator.Current);

            foreach (string customGroupId in listCustomGroupsIds)
            {
                //Ensure the current user is allowed to edit this template
                var customGroupCheck = customGroups.Find(p => p.Id == customGroupId);
                if (customGroupCheck == null)
                {
                    Log.Error("Reorder custom group - Exception: Custom group with id {0} was not found.", customGroupId);
                    return StatusCode(HttpStatusCode.InternalServerError);
                }

                //Reorder user template
                customGroupCheck.Order = i;
                status = await _GroupsRepo.UpdateItemAsync(customGroupId, customGroupCheck);
                if (!status)
                {
                    Log.Error("Reorder custom group - Exception: There was an error while reordering the custom group {0}", customGroupId);
                    return StatusCode(HttpStatusCode.InternalServerError);
                }
                i++;
            }
            return Ok(status);
        }

        /// <summary>
        /// Endpoint to delete custom groups
        /// </summary>
        /// <param name="listCustomGroupsIds">List of CustomGroup object</param>
        /// <returns>True if success</returns>
        [Route("groups/delete")]
        [ResponseType(typeof(bool))]
        [HttpPost]
        public async Task<IHttpActionResult> DeleteCustomGroups([FromBody] string[] listCustomGroupsIds)
        {
            bool status = false;
            foreach (string customGroupId in listCustomGroupsIds)
            {
                //Ensure the current user is allowed to edit this template
                var customGroupCheck = await _GroupsRepo.GetItemAsync(customGroupId);
                if (customGroupCheck == null)
                {
                    Log.Error("Delete custom group - Exception: Custom group with id {0} was not found.", customGroupId);
                    return StatusCode(HttpStatusCode.InternalServerError);
                }

                //Delete user template
                status = await _GroupsRepo.DeleteItemAsync(customGroupId);
                if (!status)
                {
                    Log.Error("Delete custom group - Exception: There was an error while deleting the custom group {0}", customGroupId);
                    return StatusCode(HttpStatusCode.InternalServerError);
                }
            }
            return Ok(status);
        }
    }
}