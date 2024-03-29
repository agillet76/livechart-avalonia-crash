using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Mixins;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ScottPlot.Avalonia;
using Splat;
using TestLiveCharts.ViewModels;
using TestLiveCharts.Models;


namespace TestLiveCharts.Views;

public partial class DropletScatterPlotView : ReactiveUserControl<DropletScatterPlotViewModel>
{
    private AvaPlot _avaPlot => this.Find<AvaPlot>("DropletPlot");
    private ComboBox _axisXComboBox => this.Find<ComboBox>("AxisXComboBox");
    private ComboBox _axisYComboBox => this.Find<ComboBox>("AxisYComboBox");
    private ComboBox _peakMeasureComboBox => this.Find<ComboBox>("PeakMeasureComboBox");
    
    public DropletScatterPlotView()
    {
        
        
        InitializeComponent();
        DataContext = Locator.Current.GetService<DropletScatterPlotViewModel>();
        //ViewModel.AvaPlot = _avaPlot;
    }

    public static readonly StyledProperty<Polygon> PolygonItemProperty =
        AvaloniaProperty.Register<DropletScatterPlotView, Polygon>(nameof(PolygonItem),
            defaultValue:null,
            defaultBindingMode: BindingMode.TwoWay);

    [Reactive] public Polygon PolygonItem
    {
        get
        {
            var p = GetValue(PolygonItemProperty);
            return p;
        }
        set { SetValue(PolygonItemProperty, value); }
    }

    public static readonly StyledProperty<string> PolygonNameProperty =
        AvaloniaProperty.Register<DropletScatterPlotView, string>(nameof(PolygonName),
            defaultValue: null,
            defaultBindingMode: BindingMode.OneWay);

    [Reactive]
    public string PolygonName
    {
        get
        {
            var p = GetValue(PolygonNameProperty);
            return p;
        }
        set { SetValue(PolygonNameProperty, value); }
    }


    public static readonly DirectProperty<DropletScatterPlotView, string> MyPropProperty =
        AvaloniaProperty.RegisterDirect<DropletScatterPlotView, string>(
            nameof(MyProp),
            o => o.MyProp,
            (o, v) => o.MyProp = v);

    private string _myprop = "";

    public string MyProp
    {
        get { return _myprop; }
        set { SetAndRaise(MyPropProperty, ref _myprop, value); }
    }

    private void InitializeComponent()
    {
        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(view => view.PolygonItem)
                .BindTo(this, view => view.ViewModel.Polygon).DisposeWith(disposables);
            //this.Bind(ViewModel, vm => vm.Polygon, view => view.PolygonItem).DisposeWith(disposables);
            var a = PolygonItem;
            //Console.WriteLine($"{a.Name}");
        });
        AvaloniaXamlLoader.Load(this);
    }
}

