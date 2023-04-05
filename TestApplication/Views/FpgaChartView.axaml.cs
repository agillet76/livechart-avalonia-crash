using Avalonia.Markup.Xaml;
using Splat;
using TestLiveCharts.ViewModels;

namespace TestLiveCharts.Views;

public partial class FpgaChartView : Avalonia.ReactiveUI.ReactiveUserControl<FpgaChartViewModel>
{


    public FpgaChartView()
    {
        InitializeComponent();
        DataContext = Locator.Current.GetService<FpgaChartViewModel>();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

