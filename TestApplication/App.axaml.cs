using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using TestLiveCharts.DependencyInjection;
using TestLiveCharts.ViewModels;
using TestLiveCharts.Views;
using Splat;

namespace TestLiveCharts;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var dataContext = Locator.Current.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = new  MainWindow
            {
                DataContext = dataContext,
            };
            desktop.Exit += OnExit;
            
        }

        base.OnFrameworkInitializationCompleted();
    }

    void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
    }
}

