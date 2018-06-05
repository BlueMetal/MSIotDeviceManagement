using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace MS.IoT.UWP.CoffeeMaker.UserControls
{
    public sealed partial class ActionButton : UserControl
    {
        // Dependency Property
        public static readonly DependencyProperty ActionNameProperty = DependencyProperty.Register(
         "ActionName", typeof(string), typeof(ActionButton), new PropertyMetadata(default(string), (s, e) =>
         {
             ActionButton control = (ActionButton)s;
             string n = (string)e.NewValue;

             control.LblActionName.Text = n;
         }));

        // Dependency Property
        public static readonly DependencyProperty ActionDescriptionProperty = DependencyProperty.Register(
         "ActionDescription", typeof(string), typeof(ActionButton), new PropertyMetadata(default(string), (s, e) =>
         {
             ActionButton control = (ActionButton)s;
             string n = (string)e.NewValue;

             control.LblActionDescription.Text = n;
         }));

        // Dependency Property
        public static readonly DependencyProperty ActionImageProperty = DependencyProperty.Register(
         "ActionImage", typeof(ImageSource), typeof(ActionButton), new PropertyMetadata(null, (s, e) =>
         {
             ActionButton control = (ActionButton)s;
             ImageSource n = (ImageSource)e.NewValue;

             control.ImgAction.Source = n;
         }));

        // Dependency Property
        public static readonly DependencyProperty LoadingImageProperty = DependencyProperty.Register(
         "LoadingImage", typeof(bool), typeof(ActionButton), new PropertyMetadata(null, (s, e) =>
         {
             ActionButton control = (ActionButton)s;
             bool n = (bool)e.NewValue;

             control.ImgLoading.Visibility = n ? Visibility.Visible : Visibility.Collapsed;
         }));

        public string ActionName
        {
            get {
                return (string)GetValue(ActionNameProperty);
            }
            set {
                SetValue(ActionNameProperty, value);
            }
        }

        public bool LoadingImage
        {
            get
            {
                return (bool)GetValue(ActionNameProperty);
            }
            set
            {
                SetValue(ActionNameProperty, value);
            }
        }

        public string ActionDescription
        {
            get
            {
                return (string)GetValue(ActionDescriptionProperty);
            }
            set
            {
                SetValue(ActionDescriptionProperty, value);
            }
        }

        public ImageSource ActionImage
        {
            get
            {
                return (ImageSource)GetValue(ActionImageProperty);
            }
            set
            {
                SetValue(ActionImageProperty, value);
            }
        }

        public ActionButton()
        {
            this.InitializeComponent();
            ImgLoading.Visibility = Visibility.Collapsed;
        }
    }
}
