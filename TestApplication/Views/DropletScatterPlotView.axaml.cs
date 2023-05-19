using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Mixins;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using ScottPlot.Avalonia;
using Splat;
using TestLiveCharts.ViewModels;
using TestLiveCharts.Models;


namespace TestLiveCharts.Views;

public partial class DropletScatterPlotView : Avalonia.ReactiveUI.ReactiveUserControl<DropletScatterPlotViewModel>
{
    private AvaPlot _avaPlot => this.Find<AvaPlot>("DropletPlot");
    private ComboBox _axisXComboBox => this.Find<ComboBox>("AxisXComboBox");
    private ComboBox _axisYComboBox => this.Find<ComboBox>("AxisYComboBox");
    private ComboBox _peakMeasureComboBox => this.Find<ComboBox>("PeakMeasureComboBox");
    
    public DropletScatterPlotView()
    {
        InitializeComponent();
        DataContext = Locator.Current.GetService<DropletScatterPlotViewModel>();
        ViewModel.AvaPlot = _avaPlot;
    }

    public static readonly StyledProperty<Polygon> PolygonItemProperty =
        AvaloniaProperty.Register<DropletScatterPlotView, Polygon>(nameof(PolygonItem),
            defaultValue:null,
            defaultBindingMode: BindingMode.TwoWay);

    public Polygon PolygonItem
    {
        get
        {
            var p = GetValue(PolygonItemProperty);
            return p;
        }
        set { SetValue(PolygonItemProperty, value); }
    }

    private void InitializeComponent()
    {
        this.WhenActivated(disposables =>
        {
            // // Bind the Polygon property to the ViewModel's Polygon property
            //this.Bind(ViewModel, vm => vm.AvaPlot, view => view._avaPlot).DisposeWith(disposables);
            //this.Bind(ViewModel, vm => vm.Polygon, view => view.PolygonItem).DisposeWith(disposables);
            // this.BindCommand(ViewModel, vm => vm.LoadDataCommand, view => view._connectButton.Command).DisposeWith(disposables);
            // this.OneWayBind(ViewModel, vm => vm.AxisTypeSupported, view => view._axisXComboBox.Items).DisposeWith(disposables);
            // //this.Bind(ViewModel, vm => vm.AxisX, view => view._axisXComboBox.SelectedItem).DisposeWith(disposables);
            // this.OneWayBind(ViewModel, vm => vm.AxisTypeSupported, view => view._axisYComboBox.Items).DisposeWith(disposables);
            // //this.Bind(ViewModel, vm => vm.AxisY, view => view._axisYComboBox.SelectedItem).DisposeWith(disposables);
            // this.OneWayBind(ViewModel, vm => vm.PeakMeasureSupported, view => view._peakMeasureComboBox.Items).DisposeWith(disposables);
            // //this.Bind(ViewModel, vm => vm.PeakMeasure, view => view._peakMeasureComboBox.SelectedItem).DisposeWith(disposables);
            this.WhenAnyValue(view => view.PolygonItem)
                .BindTo(this, view => view.ViewModel.Polygon).DisposeWith(disposables);
            //this.Bind(ViewModel, vm => vm.Polygon, view => view.PolygonItem).DisposeWith(disposables);
            var a = GetValue(PolygonItemProperty);
            Console.WriteLine($"{a}");
            // var na = GetValue(PolygonNameProperty);
            // Console.WriteLine($"{na}");
        });
        AvaloniaXamlLoader.Load(this);
    }
}

