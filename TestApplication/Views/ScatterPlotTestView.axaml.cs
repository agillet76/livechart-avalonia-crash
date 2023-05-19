using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Splat;
using TestLiveCharts.DependencyInjection;
using TestLiveCharts.ViewModels;

namespace TestLiveCharts.Views
{
    public partial class ScatterPlotTestView : ReactiveUserControl<ScatterPlotTestViewModel>
    {
        private DropletScatterPlotView ScatterPlot => this.Find<DropletScatterPlotView>("ScatterPlot");
        public ScatterPlotTestView()
        { 
            InitializeComponent();
            DataContext = Locator.Current.GetRequiredService<ScatterPlotTestViewModel>();
        }

        private void InitializeComponent()
        {
            // ViewModel's WhenActivated block will also get called.
            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.PolygonA, view => view.ScatterPlot.PolygonItem).DisposeWith(disposables);
            });
            AvaloniaXamlLoader.Load(this);
        }
    }
}
