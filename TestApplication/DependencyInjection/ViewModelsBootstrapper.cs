
using Autofac;
using ReactiveUI;
using TestLiveCharts.ViewModels;
using TestLiveCharts.Views;

namespace TestLiveCharts.DependencyInjection;

public static class ViewModelsBootstrapper
{
    public static void RegisterViewModels(ContainerBuilder builder)
    {
        builder.RegisterType<MainWindow>().As<IViewFor<MainWindowViewModel>>();
        builder.RegisterType<MainWindowViewModel>().SingleInstance();
        builder.RegisterType<MainContentView>().As<IViewFor<MainContentViewModel>>();
        builder.RegisterType<MainContentViewModel>().SingleInstance();
        builder.RegisterType<DataControllerView>().As<IViewFor<DataControllerViewModel>>();
        builder.RegisterType<DataControllerViewModel>().SingleInstance();
        builder.RegisterType<DataCollectionView>().As<IViewFor<DataCollectionViewModel>>();
        builder.RegisterType<DataCollectionViewModel>().SingleInstance();
    }
}
