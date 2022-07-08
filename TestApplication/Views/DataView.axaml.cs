using Avalonia.Markup.Xaml;

using TestLiveCharts.ViewModels;

namespace TestLiveCharts.Views
{
    public partial class DataView : Avalonia.ReactiveUI.ReactiveUserControl<DataViewModel>
    {
        public DataView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
