using MS.IoT.Domain.Model;

namespace MS.IoT.Simulator.WPF.Models
{
    public class GroupedTemplate
    {
        public Category Category { get; set; }
        //public List<Template> Templates { get; set; }
        public Template Template { get; set; }

        public string PictureUrl
        {
            get
            {
                return string.Format("pack://application:,,,/Resources/categories/{0}.png", Template.SubcategoryId);
            }
        }
    }
}
