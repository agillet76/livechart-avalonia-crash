using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Splat;
using TestLiveCharts.DependencyInjection;
using TestLiveCharts.ViewModels;

namespace TestLiveCharts.Views
{
    public partial class DataCollectionView : ReactiveUserControl<DataCollectionViewModel>
    {
        public DataCollectionView()
        { 
            InitializeComponent();
            DataContext = Locator.Current.GetRequiredService<DataCollectionViewModel>();
        }

        private void InitializeComponent()
        {
            // ViewModel's WhenActivated block will also get called.
            this.WhenActivated(disposables =>
            {
                /* Handle view activation etc. */
            });
            AvaloniaXamlLoader.Load(this);
        }
    }
}
