using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FFImageLoading.Svg;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MS.IoT.Mobile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BrewStrengthViewCell : ViewCell
	{

        private ImageSource Button0Source = FFImageLoading.Svg.Forms.SvgImageSource.FromResource("MS.IoT.Mobile.Assets.Icons.btn-mild-brew.svg");
        private ImageSource Button0SelectedSource = FFImageLoading.Svg.Forms.SvgImageSource.FromResource("MS.IoT.Mobile.Assets.Icons.btn-mild-brew-selected.svg");
        private ImageSource Button1Source = FFImageLoading.Svg.Forms.SvgImageSource.FromResource("MS.IoT.Mobile.Assets.Icons.btn-medium-brew.svg");
        private ImageSource Button1SelectedSource = FFImageLoading.Svg.Forms.SvgImageSource.FromResource("MS.IoT.Mobile.Assets.Icons.btn-medium-brew-selected.svg");
        private ImageSource Button2Source = FFImageLoading.Svg.Forms.SvgImageSource.FromResource("MS.IoT.Mobile.Assets.Icons.btn-strong-brew.svg");
        private ImageSource Button2SelectedSource = FFImageLoading.Svg.Forms.SvgImageSource.FromResource("MS.IoT.Mobile.Assets.Icons.btn-strong-brew-selected.svg");

        private static int selectedButton = 1;

        public BrewStrengthViewCell ()
		{
			InitializeComponent ();

            ChangeButton(selectedButton);
        }



        public void OnButton0Tapped()
        {           
            if (selectedButton != 0)
            {
                ChangeButton(0);
            }
        }
        public void OnButton1Tapped()
        {           
            if (selectedButton != 1)
            {
                ChangeButton(1);
            }
        }
        public void OnButton2Tapped()
        {           
            if (selectedButton != 2)
            {
                ChangeButton(2);
            }
        }

        private void ChangeButton(int button)
        {
            switch (selectedButton)
            {
                case 0:
                    Button0.Source = Button0Source;
                    Button0Label.TextColor = GlobalResources.Colors.DisabledTextColor;
                    break;
                case 1:
                    Button1.Source = Button1Source;
                    Button1Label.TextColor = GlobalResources.Colors.DisabledTextColor;
                    break;
                case 2:
                    Button2.Source = Button2Source;
                    Button2Label.TextColor = GlobalResources.Colors.DisabledTextColor;
                    break;
            }
            switch (button)
            {
                case 0:
                    Button0.Source = Button0SelectedSource;
                    Button0Label.TextColor = GlobalResources.Colors.TextColor;
                    break;
                case 1:
                    Button1.Source = Button0SelectedSource;
                    Button1Label.TextColor = GlobalResources.Colors.TextColor;
                    break;
                case 2:
                    Button2.Source = Button0SelectedSource;
                    Button2Label.TextColor = GlobalResources.Colors.TextColor;
                    break;
            }
            selectedButton = button;
        }





    }
}