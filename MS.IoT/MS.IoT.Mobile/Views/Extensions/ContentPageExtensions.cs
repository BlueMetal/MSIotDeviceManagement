using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace QXFUtilities
{
    public static class ContentPageExtensions
    {

        public static void ForceAbsoluteLayoutContentContainer(this ContentPage page)
        {
            Type ContentPageContentType = page.Content.GetType();
            if (ContentPageContentType != typeof(AbsoluteLayout))
            {
                AbsoluteLayout al = new AbsoluteLayout
                {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand
                };

                View _content = page.Content;

                AbsoluteLayout.SetLayoutBounds(_content, _content.Bounds);
                AbsoluteLayout.SetLayoutFlags(_content, AbsoluteLayoutFlags.None);
                al.Children.Add(_content);
                page.Content = al;
            }
        }


    }
}
