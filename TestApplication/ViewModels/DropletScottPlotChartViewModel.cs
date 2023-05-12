using CsvHelper;
using ReactiveUI;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI.Fody.Helpers;
using TestLiveCharts.Models;
using System.Reactive.Disposables;

namespace TestLiveCharts.ViewModels;


public enum AxisType
{
    PMT1,
    PMT2,
    Ratio,

}

public enum PeakMeasureType
{
    Peak,
    Avg,
    PeakRaw,
    AvgRaw
}

public class DropletScottPlotChartViewModel : FpgaChartViewModel
{

    protected Action<List<DropletData>> _updateDropletsDataAction;
    private Stopwatch _sw = new ();

    public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }

    public Array PeakMeasureSupported => Enum.GetValues(typeof(PeakMeasureType));
    public Array AxisTypeSupported => Enum.GetValues(typeof(AxisType));

    [Reactive]public AxisType AxisX { get; set; } = AxisType.PMT1;
    [Reactive] public AxisType AxisY { get; set; } = AxisType.PMT2;
    [Reactive] public PeakMeasureType PeakMeasure { get; set; } = PeakMeasureType.Peak;
    [Reactive] public bool IsDataLoaded { get; set; } = false;
    public  ScottPlot.Avalonia.AvaPlot AvaPlot { get; set; }

    private List<DropletData> currentDropletsData;

    public DropletScottPlotChartViewModel()
    {
        var canExecute = this.WhenAnyValue(
            x => x.IsDataLoaded, isLoaded=> !isLoaded);

        LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                await Task.Run(() =>
                {
                    // load a csv file representing the Droplet data
                    currentDropletsData = LoadData();
                    IsDataLoaded = true;

                    UpdateChart();


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
            this.WhenAnyValue(x => x.AxisX, x => x.AxisY, x => x.PeakMeasure)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => UpdateChart())
                .DisposeWith(disposables);


            Disposable
                .Create(() =>
                {
                    /* handle deactivation */
                    //Camera.StopAcquisition();
                })
                .DisposeWith(disposables);
        });
    }

    protected void  UpdateChart()
    {
        AvaPlot.Plot.Title("Polygon A");
        AvaPlot.Plot.XAxis.Label("PMT1 Max [V]", size: 11);
        AvaPlot.Plot.YAxis.Label("PMT2 Max [V]", size: 11);
        if (IsDataLoaded) OnDropletsDataChanged(currentDropletsData);
        AvaPlot.Refresh();
        Console.Error.WriteLine($"ScottPlot Chart Update Trigger {_sw.ElapsedMilliseconds}");
        
    }

    private void OnDropletsDataChanged(List<DropletData> dropletsData)
    {
        var binsX = 1500;
        var binsY = 1500;
        var min = 0.0;
        var max = 5.0;

        // Calculate the bin edges for the x and y axes
        var histX = new ScottPlot.Statistics.Histogram(min, max, binsX);
        histX.AddRange(dropletsData.Select(p => p.Max1InVolts));
        double[] xEdges = histX.Bins;
        var histY = new ScottPlot.Statistics.Histogram(min, max, binsY);
        histY.AddRange(dropletsData.Select(p => p.Max2InVolts));
        double[] yEdges = histY.Bins;

        // Create an empty 2D array to store the bin counts
        // needed nullable array to represent empty bin as transparent 
        // https://scottplot.net/cookbook/4.1/category/plottable-heatmap/#heatmap-with-empty-squares
        double?[,] counts = new double?[binsX, binsY];


        // Loop through each point and increment the appropriate bin
        foreach (var point in dropletsData)
        {
            int xBin = Array.BinarySearch(xEdges, point.Max1InVolts);
            if (xBin < 0)
                xBin = ~xBin - 1;

            int yBin = Array.BinarySearch(yEdges, point.Max2InVolts);
            if (yBin < 0)
                yBin = ~yBin - 1;
            counts[yBin, xBin] ??= 0;
            counts[yBin, xBin]++;
        }
        AvaPlot.Plot.Clear();
        var heatmap = AvaPlot.Plot.AddHeatmap(counts, ScottPlot.Drawing.Colormap.Turbo);
        heatmap.Update(counts, min: 1);
        heatmap.Smooth = true;
        heatmap.FlipVertically=true; // needed to match with LabView ScatterPlot
        var cb = AvaPlot.Plot.AddColorbar(heatmap);
        cb.Label = "Droplets";
        heatmap.XMin = 0.0;
        heatmap.XMax = 5.0;
        heatmap.YMin = 0.0;
        heatmap.YMax = 5.0;
        AvaPlot.Plot.SetAxisLimits(min, max, min, max);
        AvaPlot.Plot.XAxis.SetBoundary(min, max);
        AvaPlot.Plot.YAxis.SetBoundary(min, max);
    }

    private List<DropletData> LoadData()
    {
        using (var reader = new StreamReader(@"C:\Users\Alexandre\Desktop\DropletsData\droplets_5min.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csv.GetRecords<DropletData>().ToList();
            return records;
        }
    }
}

