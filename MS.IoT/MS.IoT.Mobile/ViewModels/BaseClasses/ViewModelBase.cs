using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;


namespace QXFUtilities
{
    public class ViewModelBase : BaseViewModel
    {

        public INavigation Navigation { get; set; }

        public ViewModelBase(INavigation navigation = null)
        {
            Navigation = navigation;
        }






    }
}
