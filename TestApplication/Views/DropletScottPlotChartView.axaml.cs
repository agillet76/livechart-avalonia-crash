using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using ScottPlot.Avalonia;
using Splat;
using TestLiveCharts.ViewModels;
using TestLiveCharts.Models;
using ScottPlot;


namespace TestLiveCharts.Views;

public partial class DropletScottPlotChartView : Avalonia.ReactiveUI.ReactiveUserControl<DropletScottPlotChartViewModel>
{
    public DropletScottPlotChartView()
    {
        InitializeComponent();
        DataContext = Locator.Current.GetService<DropletScottPlotChartViewModel>();
        (((DropletScottPlotChartViewModel) DataContext)!).AvaPlot = this.Find<AvaPlot>("AvaDropletScottPlot");
    }

    private void InitializeComponent()
    {
        this.WhenActivated(disposables =>
        {
            //this.OneWayBind(ViewModel, vm => vm.LastImage, view => view.ImageControl.Source)
            //    .DisposeWith(disposables);
        });
        AvaloniaXamlLoader.Load(this);
    }
}

