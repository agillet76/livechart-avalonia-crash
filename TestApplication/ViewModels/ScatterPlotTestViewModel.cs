using System.Collections.ObjectModel;
using ReactiveUI;
using System.Reactive;
using CsvHelper;
using System.Globalization;
using DynamicData;
using ICS.Common;
using TestLiveCharts.Models;
using ReactiveUI.Fody.Helpers;

namespace TestLiveCharts.ViewModels;

public class ScatterPlotTestViewModel : ReactiveObject
{
    public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }
    public ObservableCollection<Polygon> Polygons { get; set; } 

    [Reactive] public Polygon PolygonA { get; set; } = new() {Name = "PolygonAT"};
    private ObservableCollection<DropletScatterPlotViewModel> PolygonViewModels { get; set; } = new ();


    private readonly IDropletsDataService _dropletsDataService;
    public ScatterPlotTestViewModel(IDropletsDataService dropletsDataService)
    {
        _dropletsDataService = dropletsDataService;
        Polygons = new ObservableCollection<Polygon>()
        {
            PolygonA, new Polygon(){Name = "PolygonB"}

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
    }
}

