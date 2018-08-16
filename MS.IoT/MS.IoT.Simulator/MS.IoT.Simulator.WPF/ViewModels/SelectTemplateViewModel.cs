using MS.IoT.Common;
using MS.IoT.Domain.Interface;
using MS.IoT.Domain.Model;
using MS.IoT.Simulator.WPF.Helpers;
using MS.IoT.Simulator.WPF.Models;
using MS.IoT.Simulator.WPF.Services.Interfaces;
using MS.IoT.Simulator.WPF.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MS.IoT.Simulator.WPF.ViewModels
{
    /// <summary>
    /// SelectTemplate ModelView
    /// </summary>
    public class SelectTemplateViewModel : ObservableViewModel, ISelectTemplateViewModel
    {
        //Services Members
        private ICosmosDBRepository<Template> _TemplateRepo;
        private ICosmosDBRepository<Category> _CategoryRepo;
        private IUserService _UserService;

        //Properties Members
        private List<GroupedTemplate> _CacheGroupedTemplate;

        //Commands Members
        private AsyncCommand<IEnumerable<GroupedTemplate>> _LoadTemplatesCommand;



        /// <summary>
        /// Accessor to LoadTemplates that returns a list of templates along with category information
        /// </summary>
        public AsyncCommand<IEnumerable<GroupedTemplate>> LoadTemplates
        {
            get
            {
                return _LoadTemplatesCommand;
            }
        }

        /// <summary>
        /// Main Constructor
        /// </summary>
        /// <param name="templateRepo">Repository Template Service</param>
        /// <param name="categoryRepo">Repository Category Service</param>
        /// <param name="userService">User Service</param>
        public SelectTemplateViewModel(ICosmosDBRepository<Template> templateRepo, ICosmosDBRepository<Category> categoryRepo, IUserService userService)
        {
            Log.Debug("SelectTemplate ViewModel called");
            _TemplateRepo = templateRepo;
            _CategoryRepo = categoryRepo;
            _UserService = userService;
            _LoadTemplatesCommand = new AsyncCommand<IEnumerable<GroupedTemplate>>(() =>
                GetUserTemplatesGroupedByCategories()
            );
        }


        /// <summary>
        /// Retrieve templates based on the current user and associate a category with it.
        /// </summary>
        /// <returns>List of user templates associated with their category</returns>
        private async Task<IEnumerable<GroupedTemplate>> GetUserTemplatesGroupedByCategories()
        {
            Log.Information("Get User templates Called");

            try
            {
                if (_CacheGroupedTemplate != null && (_LoadTemplatesCommand.Parameter == null || _LoadTemplatesCommand.Parameter.ToString() != "refresh"))
                    return _CacheGroupedTemplate;

                //Get current user
                User currentUser = _UserService.GetCurrentUser();

                //Get all categories
                List<GroupedTemplate> groupTemplates = new List<GroupedTemplate>();
                IEnumerable<Category> enumCategories = await _CategoryRepo.GetItemsAsync(p => p.DocType == TemplateDocumentType.Category);
                if (enumCategories == null)
                    throw new Exception("The list of categories could not be found. Make sure that Cosmos DB is properly configured.");
                Category[] categories = enumCategories.ToArray();

                //Get user templates based on current user ID
                IEnumerable<Template> enumTemplates = await _TemplateRepo.GetItemsAsync(p => p.DocType == TemplateDocumentType.User && p.UserId == currentUser.Id, p => new Template()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    SubcategoryId = p.SubcategoryId,
                    CategoryId = p.CategoryId
                });
                if (enumTemplates == null)
                    throw new Exception("The list of templates could not be found. Make sure that Cosmos DB is properly configured.");

                //Associate user template and category. It is needed to get the proper icon and color in the UI.
                IEnumerator<Template> template = enumTemplates.GetEnumerator();
                while (template.MoveNext())
                {
                    Template currentTemplate = template.Current;
                    GroupedTemplate group = group = new GroupedTemplate()
                    {
                        Category = categories.First(p => p.Id == currentTemplate.CategoryId),
                        Template = currentTemplate
                    };
                    groupTemplates.Add(group);
                }

                //Cache the result as it won't change, unless the user forces a refresh or signs out.
                _CacheGroupedTemplate = groupTemplates;

                return groupTemplates;
            }
            catch(Exception e)
            {
                Log.Error(e.Message);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Reset the cache of user templates in order to force a new template fetch for a new user.
        /// </summary>
        public void ResetControl()
        {
            _CacheGroupedTemplate = null;
        }
    }
}
