using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class Template : BaseTemplateDocument
    {
        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "categoryId")]
        [DisplayName("Category")]
        public string CategoryId { get; set; }

        [JsonProperty(PropertyName = "subcategoryId")]
        [DisplayName("Subcategory")]
        public string SubcategoryId { get; set; }

        [JsonProperty(PropertyName = "baseTemplateId")]
        public string BaseTemplateId { get; set; }

        [JsonProperty(PropertyName = "creationDate")]
        public DateTime CreationDate { get; set; }

        [JsonProperty(PropertyName = "modifiedDate")]
        public DateTime ModifiedDate { get; set; }

        [JsonProperty(PropertyName = "isReusableTemplate")]
        public bool IsReusableTemplate { get; set; }

        [JsonProperty(PropertyName = "properties")]
        public List<CustomProperty> Properties { get; set; }
    }
}
