using MS.IoT.Simulator.WPF.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MS.IoT.Simulator.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for JsonDataGrid.xaml
    /// </summary>
    public partial class JsonDataGrid : UserControl
    {
        // Dependency Property
        public static readonly DependencyProperty TemplatePacketProperty =
             DependencyProperty.Register("DataSource", typeof(object),
             typeof(JsonDataGrid), new FrameworkPropertyMetadata(null, OnTemplatePacketPropertyChanged));

        private static void OnTemplatePacketPropertyChanged(DependencyObject source,
        DependencyPropertyChangedEventArgs e)
        {
            JsonDataGrid control = source as JsonDataGrid;
            if (control != null && e.NewValue != null && (e.NewValue is TemplatePacket || e.NewValue is CosmosDBMessage)) {
                string serialized = JsonConvert.SerializeObject(e.NewValue);
                control.textBlock.Text = JValue.Parse(serialized).ToString(Formatting.Indented);
            } else if(control != null)
            {
                control.textBlock.Text = string.Empty;
            }
        }

        public object DataSource
        {
            get {
                return GetValue(TemplatePacketProperty);
            }
            set {
                SetValue(TemplatePacketProperty, value);
            }
        }


        public JsonDataGrid()
        {
            InitializeComponent();
        }
    }
}
