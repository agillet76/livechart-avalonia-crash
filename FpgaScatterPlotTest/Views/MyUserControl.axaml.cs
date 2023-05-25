using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Markup.Xaml;

namespace FpgaScatterPlotTest.Views
{
    
    public partial class MyUserControl: UserControl
    {

        public static readonly DirectProperty<MyUserControl, string> MyPropProperty =
            AvaloniaProperty.RegisterDirect<MyUserControl, string>(
                nameof(MyProp),
                o => o.MyProp,
                (o, v) => o.MyProp = v,
                defaultBindingMode: BindingMode.TwoWay);

        private string _myprop = "";

        public string MyProp
        {
            get { return _myprop; }
            set { SetAndRaise(MyPropProperty, ref _myprop, value); }
        }

        public MyUserControl()
        {
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}