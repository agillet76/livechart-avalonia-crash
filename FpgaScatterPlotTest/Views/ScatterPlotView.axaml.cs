using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Mixins;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FpgaScatterPlotTest.Models;
using FpgaScatterPlotTest.ViewModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ScottPlot.Avalonia;

using Splat;


namespace FpgaScatterPlotTest.Views;

public partial class ScatterPlotView : ReactiveUserControl<ScatterPlotViewModel>
{
    private AvaPlot _avaPlot => this.Find<AvaPlot>("DropletPlot"); ComboBox _axisXComboBox => this.Find<ComboBox>("AxisXComboBox");
    private ComboBox _axisYComboBox => this.Find<ComboBox>("AxisYComboBox");
    private ComboBox _peakMeasureComboBox => this.Find<ComboBox>("PeakMeasureComboBox");
    
    public ScatterPlotView()
    {
        
        
        InitializeComponent();
        DataContext = Locator.Current.GetService<ScatterPlotViewModel>();
        //ViewModel.AvaPlot = _avaPlot;
    }

    public static readonly StyledProperty<Polygon> PolygonItemProperty =
        AvaloniaProperty.Register<ScatterPlotView, Polygon>(nameof(PolygonItem),
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
        AvaloniaProperty.Register<ScatterPlotView, string>(nameof(PolygonName),
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


    public static readonly DirectProperty<ScatterPlotView, string> MyPropProperty =
        AvaloniaProperty.RegisterDirect<ScatterPlotView, string>(
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

