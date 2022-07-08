using Avalonia.Markup.Xaml;
using Splat;
using TestLiveCharts.ViewModels;

namespace TestLiveCharts.Views
{
    public partial class DataControllerView : Avalonia.ReactiveUI.ReactiveUserControl<DataControllerViewModel>
    {
        public DataControllerView()
        {
            InitializeComponent();
            DataContext = Locator.Current.GetService<DataControllerViewModel>();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
