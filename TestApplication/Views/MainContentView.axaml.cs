using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Splat;
using TestLiveCharts.DependencyInjection;
using TestLiveCharts.ViewModels;

namespace TestLiveCharts.Views
{
    public partial class MainContentView : ReactiveUserControl<MainContentViewModel>
    {
        public MainContentView()
        {
            InitializeComponent();
            DataContext = Locator.Current.GetRequiredService<MainContentViewModel>();

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
