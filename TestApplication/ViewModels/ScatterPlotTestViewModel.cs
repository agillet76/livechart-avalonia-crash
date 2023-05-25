using System.Collections.ObjectModel;
using ReactiveUI;
using System.Reactive;
using TestLiveCharts.Models;
using ReactiveUI.Fody.Helpers;
using Splat.ModeDetection;
using DynamicData.Binding;
using System.Reactive.Disposables;

namespace TestLiveCharts.ViewModels;

public class ScatterPlotTestViewModel : ReactiveObject, IActivatableViewModel
{
    public ViewModelActivator Activator { get; }
    public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }
    public ObservableCollection<Polygon> Polygons { get; set; } 

    [Reactive] public Polygon PolygonA { get; set; } 
    private ObservableCollection<DropletScatterPlotViewModel> PolygonViewModels { get; set; } = new ();


    private readonly IDropletsDataService _dropletsDataService;
    public ScatterPlotTestViewModel(IDropletsDataService dropletsDataService)
    {
        Activator = new ViewModelActivator();
        _dropletsDataService = dropletsDataService;
        PolygonA = new Polygon() { Name = "TestPolygon"};
        Polygons = new ObservableCollection<Polygon>()
        {
            new Polygon(){Name = "PolygonB"}, new Polygon(){Name = "PolygonC"}

        };
        var canExecute = this.WhenAnyValue(
            x => x._dropletsDataService.IsDataLoaded, isLoaded => !isLoaded);

        // foreach (var polygon in Polygons)
        // {
        //     PolygonViewModels.Add(new DropletScottPlotChartViewModel(polygon));
        // }

        LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                await Task.Run(() =>
                {
                    // load a csv file representing the Droplet data
                    _dropletsDataService.LoadData();
                }).ConfigureAwait(true);
            }
            catch (Exception)
            {
                Console.Error.WriteLine("Failed Start Monitoring");
            }
        }, canExecute);
        this.WhenActivated((CompositeDisposable disposables) =>
        {
            /* handle activation */

           

            Disposable
                .Create(() =>
                {
                    /* handle deactivation */
                    //Camera.StopAcquisition();
                })
                .DisposeWith(disposables);
        });
    }
}


