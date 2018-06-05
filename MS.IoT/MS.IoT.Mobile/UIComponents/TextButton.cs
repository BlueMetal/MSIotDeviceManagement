using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using System.Threading.Tasks;
using System.ComponentModel;

using Xamarin.Forms;

namespace QXFUtilities.Controls
{
    public class TextButton : ContentView
    {



        public event EventHandler Clicked;



        // Custom view properties and fields

        private Button _button = new Button { BackgroundColor = Color.Transparent, BorderWidth = 0, BorderColor = Color.Transparent }; // Actual button control - will retain to support visual cues 
        private ExtendedBoxView _visualButton = new ExtendedBoxView();  // dummy button visual required due to bug in Android button. will also be used to support asymmetric buttons
        private ExtendedBoxView _filter = new ExtendedBoxView();  // filter
        private RelativeLayout _rL = new RelativeLayout();


        private bool cachedFilterVisibility = false;


        private bool BaseViewIsVisible
        {
            get { return base.IsVisible; }
            set { base.IsVisible = value; }
        }
        private double BaseViewOpacity
        {
            get { return base.Opacity; }
            set { base.Opacity = value; }
        }




        public static readonly BindableProperty TagProperty = BindableProperty.Create("Tag", typeof(object), typeof(TextButton), null);
        public object Tag
        {
            get { return GetValue(TagProperty); }
            set {
                if (value != GetValue(TagProperty)) {
                    SetValue(TagProperty, value);
                }
            }
        }




        public new static readonly BindableProperty IsEnabledProperty = BindableProperty.Create
            (nameof(IsEnabled), typeof(bool), typeof(TextButton), true, BindingMode.OneWay, null,
                new BindableProperty.BindingPropertyChangedDelegate((sender, old, current) => {
                    TextButton button = (TextButton)sender;
                    SetEnabledVisibilityFilter(button, (bool)current);
                }));
        public new bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set
            {
                if (value != (bool)GetValue(IsEnabledProperty)) {
                    SetValue(IsEnabledProperty, value);
                    SetEnabledVisibilityFilter(this, value);
                }
            }
        }



        public static readonly BindableProperty DisabledFilterColorProperty = BindableProperty.Create
            (nameof(DisabledFilterColor), typeof(Color), typeof(TextButton), Color.Transparent, BindingMode.OneWay, null,
                new BindableProperty.BindingPropertyChangedDelegate((sender, old, current) => {
                    TextButton button = (TextButton)sender;
                    button._filter.BackgroundColor = (Color)current;
                }));
        public Color DisabledFilterColor
        {
            get { return (Color)GetValue(DisabledFilterColorProperty); }
            set
            {
                if (value != (Color)GetValue(DisabledFilterColorProperty)) {
                    SetValue(DisabledFilterColorProperty, value);
                    _filter.BackgroundColor = value;
                }
            }
        }



        public new static readonly BindableProperty IsVisibleProperty = BindableProperty.Create
            (nameof(IsVisible), typeof(bool), typeof(TextButton), true, BindingMode.OneWay, null,
                new BindableProperty.BindingPropertyChangedDelegate((sender, old, current) => {
                    TextButton button = (TextButton)sender;
                    SetVisibility(button, (bool)current);
                }));
        public new bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set
            {
                if (value != (bool)GetValue(IsVisibleProperty))
                {
                    SetValue(IsVisibleProperty, value);
                    SetVisibility(this, value);
                }
            }
        }
        private static void SetVisibility(TextButton button, bool value)
        {
            if (value) {
                button._filter.IsVisible = button.cachedFilterVisibility;
            } else {
                button.cachedFilterVisibility = button._filter.IsVisible;
                button._filter.IsVisible = value;
            }
            button._button.IsVisible = value;
            button._rL.IsVisible = value;
            button.BaseViewIsVisible = value;
        }




        public new static readonly BindableProperty OpacityProperty = BindableProperty.Create
            (nameof(Opacity), typeof(double), typeof(TextButton), 1.0, BindingMode.OneWay, null,
                new BindableProperty.BindingPropertyChangedDelegate((sender, old, current) => {
                    TextButton button = (TextButton)sender;
                    SetOpacity(button, (double)current);
                }));
        public new double Opacity
        {
            get { return (double)GetValue(OpacityProperty); }
            set
            {
                if (value != (double)GetValue(OpacityProperty))
                {
                    SetValue(OpacityProperty, value);
                    SetOpacity(this, value);
                }
            }
        }
        private static void SetOpacity(TextButton button, double value)
        {
            button._filter.Opacity = value;
            button._button.Opacity = value;
            button._rL.Opacity = value;
            button.BaseViewOpacity = value;
        }







        // Exposed Button Properties and fields

        public new static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create
            (nameof(BackgroundColor), typeof(Color), typeof(TextButton), Color.Transparent, BindingMode.OneWay, null,
                new BindableProperty.BindingPropertyChangedDelegate((sender, old, current) => {
                    TextButton button = (TextButton)sender;
                    button._button.BackgroundColor = (Color)current;
                }));
        public new Color BackgroundColor
        {
            get { return (Color)GetValue(BackgroundColorProperty); }
            set {
                if (value != (Color)GetValue(BackgroundColorProperty)) {
                    SetValue(BackgroundColorProperty, value);
                    //_button.BackgroundColor = value;
                    _visualButton.BackgroundColor = value;
                }
            }
        }




        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create
            (nameof(BorderColor), typeof(Color), typeof(TextButton), Color.Transparent, BindingMode.OneWay, null,
                new BindableProperty.BindingPropertyChangedDelegate((sender, old, current) => {
                    TextButton button = (TextButton)sender;
                    button._visualButton.BorderColor = (Color)current;
                    //button._button.BorderColor = (Color)current;
                }));
        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set {
                if (value != (Color)GetValue(BorderColorProperty)) {
                    SetValue(BorderColorProperty, value);
                    _visualButton.BorderColor = value;
                    //_button.BorderColor = value;
                }
            }
        }





        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create
            (nameof(CornerRadius), typeof(CornerRadius), typeof(TextButton), new CornerRadius(0, 0, 0, 0), BindingMode.OneWay, null,
                new BindableProperty.BindingPropertyChangedDelegate((sender, old, current) => {
                    TextButton button = (TextButton)sender;
                    //button._button.BorderRadius = (int)current;
                    button._visualButton.CornerRadius = (CornerRadius)current;
                    button._filter.CornerRadius = (CornerRadius)current;
                }));
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set {
                if (value != (CornerRadius)GetValue(CornerRadiusProperty))
                {
                    SetValue(CornerRadiusProperty, value);
                    //_button.BorderRadius = value;
                    _visualButton.CornerRadius = value;
                    _filter.CornerRadius = value;
                }
            }
        }




        public static readonly BindableProperty BorderWidthProperty = BindableProperty.Create
            (nameof(BorderWidth), typeof(double), typeof(TextButton), 0.0, BindingMode.OneWay, null,
                new BindableProperty.BindingPropertyChangedDelegate((sender, old, current) => {
                    TextButton button = (TextButton)sender;
                    //button._button.BorderWidth = (double)current;
                    button._visualButton.BorderThickness = (double)current;
                    button._filter.BorderThickness = (double)current;
                }));
        public double BorderWidth
        {
            get { return (double)GetValue(BorderWidthProperty); }
            set {
                if (value != (double)GetValue(BorderWidthProperty)) {
                    SetValue(BorderWidthProperty, value);
                    //_button.BorderWidth = value; 
                    _button.Margin = new Thickness(value);
                    _visualButton.BorderThickness = value;
                    _filter.BorderThickness = value;
                }
            }
        }






        public static readonly BindableProperty CommandProperty = BindableProperty.Create
            (nameof(Command), typeof(System.Windows.Input.ICommand), typeof(TextButton), null, BindingMode.OneWay, null,

                new BindableProperty.BindingPropertyChangedDelegate((sender, old, current) => {
                    TextButton button = (TextButton)sender;
                    button._button.Command = current == null ? null : (System.Windows.Input.ICommand)current;
                }));
        public System.Windows.Input.ICommand Command
        {
            get { return (System.Windows.Input.ICommand)GetValue(CommandProperty); }
            set {
                if (value != (System.Windows.Input.ICommand)GetValue(CommandProperty)) {
                    SetValue(CommandProperty, value);
                    _button.Command = value;
                }
            }
        }


        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create
            (nameof(CommandParameter), typeof(object), typeof(TextButton), null, BindingMode.OneWay, null,
                new BindableProperty.BindingPropertyChangedDelegate((sender, old, current) => {
                    TextButton button = (TextButton)sender;
                    button._button.CommandParameter = current;
                }));
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set {
                if (value != GetValue(CommandParameterProperty)) {
                    SetValue(CommandParameterProperty, value);
                    _button.CommandParameter = value;
                }
            }
        }





        // Button Text Properties


        public static readonly BindableProperty ButtonHorizontalOptionsProperty = BindableProperty.Create
            (nameof(ButtonHorizontalOptions), typeof(LayoutOptions), typeof(TextButton), LayoutOptions.CenterAndExpand, BindingMode.OneWay, null,
                new BindableProperty.BindingPropertyChangedDelegate((sender, old, current) => {
                    if ((current != null) && (old != null))
                    {
                        TextButton button = (TextButton)sender;
                        LayoutOptions newOptions = (LayoutOptions)current;
                        LayoutOptions oldOptions = (LayoutOptions)old;
                        if ((newOptions.Alignment != oldOptions.Alignment) || (newOptions.Expands != oldOptions.Expands)) {
                            button._button.HorizontalOptions = newOptions;
                        }
                    }
                }));
        public LayoutOptions ButtonHorizontalOptions
        {
            get { return (LayoutOptions)GetValue(ButtonHorizontalOptionsProperty); }
            set {
                if (!value.Equals((LayoutOptions)GetValue(ButtonVerticalOptionsProperty))) {
                    SetValue(ButtonHorizontalOptionsProperty, value);
                    _button.HorizontalOptions = value;
                }
            }
        }



        public static readonly BindableProperty ButtonVerticalOptionsProperty = BindableProperty.Create
            (nameof(ButtonVerticalOptions), typeof(LayoutOptions), typeof(TextButton), LayoutOptions.CenterAndExpand, BindingMode.OneWay, null,
                new BindableProperty.BindingPropertyChangedDelegate((sender, old, current) => {
                    if ((current != null) && (old != null))
                    {
                        TextButton button = (TextButton)sender;
                        LayoutOptions newOptions = (LayoutOptions)current;
                        LayoutOptions oldOptions = (LayoutOptions)old;
                        if ((newOptions.Alignment != oldOptions.Alignment) || (newOptions.Expands != oldOptions.Expands)) {
                            button._button.VerticalOptions = newOptions;
                        }
                    }
                }));
        public LayoutOptions ButtonVerticalOptions
        {
            get { return (LayoutOptions)GetValue(ButtonVerticalOptionsProperty); }
            set {
                if (!value.Equals((LayoutOptions)GetValue(ButtonVerticalOptionsProperty))) {
                    SetValue(ButtonVerticalOptionsProperty, value);
                    _button.VerticalOptions = value;
                }
            }
        }





        public static readonly BindableProperty FontProperty = BindableProperty.Create
            (nameof(Font), typeof(Font), typeof(TextButton), Font.Default, BindingMode.OneWay, null,
                new BindableProperty.BindingPropertyChangedDelegate((sender, old, current) => {
                    TextButton button = (TextButton)sender;
                    button._button.Font = current == null ? Font.Default : (Font)current;
                }));
        public Font Font
        {
            get {
                return (Font.OfSize(_button.FontFamily, _button.FontSize)).WithAttributes(_button.FontAttributes);
            }
            set {
                if (value != (Font)GetValue(FontProperty)) {
                    SetValue(FontProperty, value);
                    _button.Font = value;
                }
            }
        }




        public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create
            (nameof(FontAttributes), typeof(FontAttributes), typeof(TextButton), Font.Default.FontAttributes, BindingMode.OneWay, null,
                new BindableProperty.BindingPropertyChangedDelegate((sender, old, current) => {
                    TextButton button = (TextButton)sender;
                    button._button.FontAttributes = (FontAttributes)current;
                }));
        public FontAttributes FontAttributes
        {
            get { return (FontAttributes)GetValue(FontAttributesProperty); }
            set {
                if (value != (FontAttributes)GetValue(FontAttributesProperty)) {
                    SetValue(FontAttributesProperty, value);
                    _button.FontAttributes = value;
                }
            }
        }




        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create
            (nameof(FontFamily), typeof(string), typeof(TextButton), Font.Default.FontFamily, BindingMode.OneWay, null,
                new BindableProperty.BindingPropertyChangedDelegate((sender, old, current) => {
                    TextButton button = (TextButton)sender;
                    button._button.FontFamily = current == null ? null : (string)current;
                }));
        public string FontFamily
        {
            get { return (string)GetValue(FontFamilyProperty); }
            set {
                if (value != (string)GetValue(FontFamilyProperty)) {
                    SetValue(FontFamilyProperty, value);
                    _button.FontFamily = value;
                }
            }
        }




        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create
            (nameof(FontSize), typeof(double), typeof(TextButton), Font.Default.FontSize, BindingMode.OneWay, null,
                new BindableProperty.BindingPropertyChangedDelegate((sender, old, current) => {
                    TextButton button = (TextButton)sender;
                    button._button.FontSize = (double)current;
                }));
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set {
                if (value != (double)GetValue(FontSizeProperty)) {
                    SetValue(FontSizeProperty, value);
                    _button.FontSize = value;
                }
            }
        }



        // Note: will always reset the Text, even if the same value, due to potential issues in Xamarin.Forms that may require the Text to be re-assigned
        public static readonly BindableProperty TextProperty = BindableProperty.Create
            (nameof(Text), typeof(string), typeof(TextButton), string.Empty, BindingMode.OneWay, null,
                new BindableProperty.BindingPropertyChangedDelegate((sender, old, current) => {
                    TextButton button = (TextButton)sender;
                    button._button.Text = current == null ? null : (string)current;
                }));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set {
                SetValue(TextProperty, value);
                _button.Text = value;
            }
        }




        public static readonly BindableProperty TextColorProperty = BindableProperty.Create
            (nameof(TextColor), typeof(Color), typeof(TextButton), Color.Default, BindingMode.OneWay, null,
                new BindableProperty.BindingPropertyChangedDelegate((sender, old, current) => {
                    TextButton button = (TextButton)sender;
                    button._button.TextColor = (Color)current;
                }));
        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set {
                if (value != (Color)GetValue(TextColorProperty)) {
                    SetValue(TextColorProperty, value);
                    _button.TextColor = value;
                }
            }
        }




        public static readonly BindableProperty TextIsVisibleProperty = BindableProperty.Create
            (nameof(TextIsVisible), typeof(bool), typeof(TextButton), true, BindingMode.OneWay, null,
                new BindableProperty.BindingPropertyChangedDelegate((sender, old, current) => {
                    TextButton button = (TextButton)sender;
                    if ((bool)current) {
                        button._button.TextColor = (Color)button.GetValue(TextColorProperty);
                    } else {
                        button._button.TextColor = Color.Transparent;
                    }
                }));
        public bool TextIsVisible
        {
            get { return (bool)GetValue(TextIsVisibleProperty); }
            set
            {
                if (value != (bool)GetValue(TextIsVisibleProperty))
                {
                    SetValue(TextIsVisibleProperty, value);
                    if (value) {
                        _button.TextColor = (Color)GetValue(TextColorProperty);
                    } else {
                        _button.TextColor = Color.Transparent;
                    }
                }
            }
        }







        public TextButton() : base()
        {

            base.BackgroundColor = Color.Transparent;
            base.Padding = 0;
            base.VerticalOptions = LayoutOptions.FillAndExpand;


            _button = new Button
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent,
                BorderColor = Color.Transparent,
				BorderRadius = 0,
				BorderWidth = 0,
				Text = this.Text,
				FontFamily = this.FontFamily,
				FontSize = this.FontSize,
				TextColor = this.TextColor
            };
            _button.Clicked += SendClicked;


            _visualButton = new ExtendedBoxView
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent,
                BorderColor = Color.Transparent,
				CornerRadius = new CornerRadius(0),
                BorderThickness = 0
            };


            _filter = new ExtendedBoxView
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent,
                BorderColor = Color.Transparent,
				CornerRadius = new CornerRadius(0),
                BorderThickness = 0,
                IsVisible = false,
                IsEnabled = false       
            };


			_rL = new RelativeLayout
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions= LayoutOptions.FillAndExpand,
                Padding=0,
                IsClippedToBounds=true   // Need to check this is ok for iOS and android
			};


            // An adjustment is required on Windows Phone due to a bug in the WP implementation of Button,
            // which is placed in a Frame that has padding set to 15. As we cant access this to change it
            // we must adjust the positioning
            double wpAdj = 15;

            if (Device.RuntimePlatform == Device.UWP || Device.RuntimePlatform == Device.WinRT || Device.RuntimePlatform == Device.WinPhone)
            {
                _rL.Children.Add(_visualButton, Constraint.Constant (-1*wpAdj), Constraint.Constant (-1*wpAdj), Constraint.RelativeToParent(p => p.Width+(2*wpAdj)), Constraint.RelativeToParent(p=>p.Height+(2*wpAdj)));
                _rL.Children.Add(_button, Constraint.Constant (-1*wpAdj), Constraint.Constant (-1*wpAdj), Constraint.RelativeToParent(p => p.Width+(2*wpAdj)), Constraint.RelativeToParent(p=>p.Height+(2*wpAdj)));
                _rL.Children.Add(_filter, Constraint.Constant (-1*wpAdj), Constraint.Constant (-1*wpAdj), Constraint.RelativeToParent(p => p.Width+(2*wpAdj)), Constraint.RelativeToParent(p=>p.Height+(2*wpAdj)));
            }
            else { 
                _rL.Children.Add(_visualButton, Constraint.Constant (0), Constraint.Constant (0), Constraint.RelativeToParent(p => p.Width), Constraint.RelativeToParent(p=>p.Height));
                _rL.Children.Add(_button, Constraint.Constant (0), Constraint.Constant (0), Constraint.RelativeToParent(p => p.Width), Constraint.RelativeToParent(p=>p.Height));
                _rL.Children.Add(_filter, Constraint.Constant (0), Constraint.Constant (0), Constraint.RelativeToParent(p => p.Width), Constraint.RelativeToParent(p=>p.Height));
            }

			//			_filter.GestureRecognizers.Add (new TapGestureRecognizer (sender => {SendClicked();} ));



			Content = _rL;


		}



		private static void SetEnabledVisibilityFilter(TextButton thisButton, bool isEnabled)
		{
			if (isEnabled) {
				thisButton._button.IsEnabled = true;
                thisButton._filter.IsVisible = false;
                thisButton._filter.IsEnabled = false;
			} else {
				if (Device.RuntimePlatform == Device.UWP || Device.RuntimePlatform == Device.WinRT || Device.RuntimePlatform == Device.WinPhone)
                {	// Don't want to disable underlying button on iOS or winPhone so we preserve the look of text - as disabled look handled by filter layer
					thisButton._button.IsEnabled = false;  
				}
                thisButton._filter.IsVisible = true;
                thisButton._filter.IsEnabled = true;
			}
		}



		private void SendClicked(object sender, EventArgs args)
		{           
			if (this.IsEnabled) {
                Clicked?.Invoke(this, EventArgs.Empty);
            }
		}



	}

/*
    public class TestRL : RelativeLayout
    {

        public TestRL() : base(){}

        protected override SizeRequest OnSizeRequest(double widthConstraint, double heightConstraint)
        {
            SizeRequest req = base.OnSizeRequest(widthConstraint, heightConstraint);
            return req;
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            SizeRequest req = base.OnMeasure(widthConstraint, heightConstraint);
            return req;
        }

        protected override void LayoutChildren(double x, double y, double w, double h)
        {
            base.LayoutChildren(x, y, w, h);
        }


        protected override void OnAdded(View view)
        {
            base.OnAdded(view);
        }




    }
*/

}

