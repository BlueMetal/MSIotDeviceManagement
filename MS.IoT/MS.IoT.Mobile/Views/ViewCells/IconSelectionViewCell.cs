using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

namespace MS.IoT.Mobile
{
    public class IconSelectionViewCell : ViewCell
    {
        // Have to do in code to dynamically support multiple icons/buttons

        public List<SelectionItem> SelectionItems { get; set; } = new List<SelectionItem>();

  


        public IconSelectionViewCell()
        {

        }

        public IconSelectionViewCell(List<SelectionItem> selectionItems)
        {
            SelectionItems = selectionItems;
        }


        public class SelectionItem
        {
            public ImageSource SelectedIcon;
            public ImageSource UnselectedIcon;
            public string Label;
        }

    }


}
