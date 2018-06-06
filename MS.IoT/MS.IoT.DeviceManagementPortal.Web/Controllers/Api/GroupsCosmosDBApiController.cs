using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MS.IoT.Domain.Interface;
using MS.IoT.DeviceManagementPortal.Web.Helpers;
using MS.IoT.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MS.IoT.DeviceManagementPortal.Web.Controllers.API
{

    [Route("api/groups")]
    [Authorize]
    public class GroupsCosmosDBApiController : BaseApiController
    {
        private ICosmosDBRepository<CustomGroupModel> _GroupsRepo;
        private readonly IDeviceDBService _DeviceServiceDB;
        private readonly ILogger<GroupsCosmosDBApiController> logger;

        public GroupsCosmosDBApiController(IUserProfileService userProfile, 
                                           ICosmosDBRepository<CustomGroupModel> groupsRepo, 
                                           IDeviceDBService deviceServiceDB,
                                           ILogger<GroupsCosmosDBApiController> logger) 
            : base(userProfile)
        {
            _GroupsRepo = groupsRepo;
            _DeviceServiceDB = deviceServiceDB;
            this.logger = logger;
        }

        [Route("groups")]
        [HttpGet]
        [Produces(typeof(IEnumerable<CustomGroupModel>))]
        public async Task<IActionResult> GetCustomGroups()
        {
            try
            {
                logger.LogInformation("Get custom groups called");

                var customGroups = await _GroupsRepo.GetItemsAsync();

                return Ok(customGroups);
            }
            catch (Exception e)
            {
                logger.LogError("Get custom groups - Exception: {message}", e.Message);
                throw;
            }
        }

        [Route("group/{customGroupId}")]
        [HttpGet]
        [Produces(typeof(CustomGroupModel))]
        public async Task<IActionResult> GetCustomGroupById(string customGroupId)
        {
            try
            {
                logger.LogInformation("Get custom group called");

                var device = await _GroupsRepo.GetItemAsync(customGroupId);
                return Ok(device);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Get custom group - Exception: {message}", e.Message);
                throw;
            }
        }

        /// <summary>
        /// Endpoint to create a custom group
        /// </summary>
        /// <param name="customGroup">CustomGroup object</param>
        /// <returns>CustomGroup ID</returns>
        [Route("groups")]
        [Produces(typeof(string))]
        [HttpPost]
        public async Task<IActionResult> CreateCustomGroup([FromBody] CustomGroupModel customGroup)
        {
            logger.LogInformation("Create custom group called");

            //New Guid
            customGroup.Id = Guid.NewGuid().ToString();
            customGroup.Count = 0;

            //Get number of custom groups
            IEnumerable<CustomGroupModel> customGroupsEnum = await _GroupsRepo.GetItemsAsync();
            if (customGroupsEnum != null)
            {
                List<CustomGroupModel> customGroups = new List<CustomGroupModel>();
                IEnumerator<CustomGroupModel> enumator = customGroupsEnum.GetEnumerator();
                while (enumator.MoveNext())
                    customGroups.Add(enumator.Current);

                customGroup.Order = customGroups.Count;
            }
            {
                customGroup.Order = 0;
            }

            //Create new user template
            string newId = await _GroupsRepo.CreateItemAsync(customGroup);
            if (string.IsNullOrEmpty(newId))
            {
                logger.LogError("Create custom group - Exception: {0}", customGroup.Name);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(newId);
        }

        /// <summary>
        /// Endpoint to edit a custom group
        /// </summary>
        /// <param name="customGroup">CustomGroup object</param>
        /// <returns>True if success</returns>
        [Route("groups")]
        [Produces(typeof(bool))]
        [HttpPut]
        public async Task<IActionResult> UpdateCustomGroup([FromBody] CustomGroupModel customGroup)
        {
            logger.LogInformation("Update custom group called");

            //Ensure the current user is allowed to edit this template
            var customGroupItem = await _GroupsRepo.GetItemAsync(customGroup.Id);
            if (customGroupItem == null)
            {
                logger.LogError("Update custom group - Exception: Custom group with id {0} was not found.", customGroup.Id);
                return StatusCode(StatusCodes.Status500InternalServerError);
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
                logger.LogError("Update custom group - Exception: There was an error while updating the custom group {0}", customGroup.Id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(status);
        }

        /// <summary>
        /// Endpoint to reorder custom groups
        /// </summary>
        /// <param name="listCustomGroupsIds">List of CustomGroup object</param>
        /// <returns>True if success</returns>
        [Route("groups/reorder")]
        [Produces(typeof(bool))]
        [HttpPost]
        public async Task<IActionResult> ReorderCustomGroups([FromBody] string[] listCustomGroupsIds)
        {
            logger.LogInformation("Reorder custom group called");

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
                    logger.LogError("Reorder custom group - Exception: Custom group with id {0} was not found.", customGroupId);
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                //Reorder user template
                customGroupCheck.Order = i;
                status = await _GroupsRepo.UpdateItemAsync(customGroupId, customGroupCheck);
                if (!status)
                {
                    logger.LogError("Reorder custom group - Exception: There was an error while reordering the custom group {0}", customGroupId);
                    return StatusCode(StatusCodes.Status500InternalServerError);
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
        [Produces(typeof(bool))]
        [HttpPost]
        public async Task<IActionResult> DeleteCustomGroups([FromBody] string[] listCustomGroupsIds)
        {
            bool status = false;
            foreach (string customGroupId in listCustomGroupsIds)
            {
                //Ensure the current user is allowed to edit this template
                var customGroupCheck = await _GroupsRepo.GetItemAsync(customGroupId);
                if (customGroupCheck == null)
                {
                    logger.LogError("Delete custom group - Exception: Custom group with id {0} was not found.", customGroupId);
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                //Delete user template
                status = await _GroupsRepo.DeleteItemAsync(customGroupId);
                if (!status)
                {
                    logger.LogError("Delete custom group - Exception: There was an error while deleting the custom group {0}", customGroupId);
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            return Ok(status);
        }
    }
}