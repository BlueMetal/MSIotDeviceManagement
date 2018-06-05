using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace QXFUtilities
{
    public static class ViewExtensions
    {

        public static Rectangle AbsoluteLocation(this View view, AbsoluteLocationRoot locationRoot = AbsoluteLocationRoot.Page, string viewGUID = null)
        {
            double x = view.X;
            double y = view.Y;
            double h = view.Height;
            double w = view.Width;

            try  // try block in case cant cast first parent
            {
                var parent = (VisualElement)view.Parent;

				while (parent != null) // && parent.Parent.GetType() == typeof(VisualElement))
                {
					if (locationRoot == AbsoluteLocationRoot.Page)
					{
						if (parent.GetType() == typeof(Page))
						{
							break;
						}
					}
					else if (locationRoot == AbsoluteLocationRoot.ViewWithGUID)
					{
						string parentID = parent.Id.ToString();
						if (parentID == viewGUID)
						{
							break;
						}
					}

                    x += parent.X;
                    y += parent.Y;
                    parent = (VisualElement)parent.Parent;
                }
            }
            catch { }

            return new Rectangle(x, y, w, h);
        }


    }

	public enum AbsoluteLocationRoot
	{
		Screen,
		Page,
		ViewWithGUID
	}
}
