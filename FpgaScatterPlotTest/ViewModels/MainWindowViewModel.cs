using System;
using FpgaScatterPlotTest.Models;
using FpgaScatterPlotTest.Services;
using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;

namespace FpgaScatterPlotTest.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";
        public Polygon PolygonA { get; set; } = new Polygon(){ Name = "TestAAAA"};
        public IDropletsDataService _dropletsDataService { get; set; } = new DropletsDataService();

        public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }

        public MainWindowViewModel()
        {

            var canExecute = this.WhenAnyValue(
                x => x._dropletsDataService.IsDataLoaded, isLoaded => !isLoaded);


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
}