using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using DynamicData.Binding;
using FpgaScatterPlotTest.Models;
using FpgaScatterPlotTest.Services;
using FpgaScatterPlotTest.ViewModels;
using ReactiveUI.Fody.Helpers;



namespace FpgaScatterPlotTest.ViewModels;

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

public class ScatterPlotViewModel : ViewModelBase, IActivatableViewModel
{
    public ViewModelActivator Activator { get; }

    protected Action<List<DropletData>> _updateDropletsDataAction;
    private Stopwatch _sw = new ();

    public Array PeakMeasureSupported => Enum.GetValues(typeof(PeakMeasureType));
    public Array AxisTypeSupported => Enum.GetValues(typeof(AxisType));

    [Reactive]public AxisType AxisX { get; set; } = AxisType.PMT1;
    [Reactive] public AxisType AxisY { get; set; } = AxisType.PMT2;
    [Reactive] public PeakMeasureType PeakMeasure { get; set; } = PeakMeasureType.Peak;

    [Reactive] public Polygon? Polygon { get; set; }
    [Reactive] public string? PolygonName { get; set; }
    public  ScottPlot.Avalonia.AvaPlot  AvaPlot { get; set; }

    private readonly IDropletsDataService _dropletsDataService;

    public ScatterPlotViewModel()
    {
        _dropletsDataService = new DropletsDataService();
        Activator = new ViewModelActivator();

        this.WhenActivated((CompositeDisposable disposables) =>
        {
            /* handle activation */

            this.WhenAnyValue(x => x.Polygon, x => x.AxisX, x => x.AxisY, x => x.PeakMeasure)
                .ObserveOn(RxApp.MainThreadScheduler).Skip(1)
                .Subscribe(_ => UpdateChart())
                .DisposeWith(disposables);

            var changeSet = _dropletsDataService.DropletsData.ToObservableChangeSet();
            changeSet.Skip(1).Subscribe(changes =>
            {
                // Console.WriteLine("Collection changed!");
                // foreach (var change in changes)
                // {
                //     Console.WriteLine($"Change: {change.Range}");
                // }
                UpdateChart();
            });

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
        if (AvaPlot.Plot == null) return;
        AvaPlot.Plot.Title(Polygon == null ? "Not Set": Polygon.Name );
        var (labelX,labelY) = GetAxisLabel();
        AvaPlot.Plot.XAxis.Label(labelX, size: 11);
        AvaPlot.Plot.YAxis.Label(labelY, size: 11);
        if (_dropletsDataService.DropletsData?.Count >0) OnDropletsDataChanged();
        AvaPlot.Refresh();
    }

    private (string,string) GetAxisLabel()
    {
        var unitX = GetAxisLabelUnit(AxisX);
        var unitY = GetAxisLabelUnit(AxisX);
        var peakMeasure = GetAxisLabelPeakMeasure(PeakMeasure);
        var labelX = $"{AxisX} {peakMeasure} {unitX}";
        var labelY = $"{AxisY} {peakMeasure} {unitY}";
        return(labelX,labelY);
    }

    private string GetAxisLabelPeakMeasure(PeakMeasureType peakMeasure)
    {
        return peakMeasure switch
        {
            PeakMeasureType.Peak => "Max",
            PeakMeasureType.PeakRaw => "Max Unsmoothed",
            PeakMeasureType.Avg => "Average",
            PeakMeasureType.AvgRaw => "Average Unsmoothed",
            _ => throw new ArgumentOutOfRangeException(nameof(peakMeasure), $"Not expected axis type value: {peakMeasure}"),
        };
    }
    private string GetAxisLabelUnit(AxisType axis)
    {
        return axis switch
        {
            AxisType.Ratio => "",
            AxisType.PMT1 => "[V]",
            AxisType.PMT2 => "[V]",
            _ => throw new ArgumentOutOfRangeException(nameof(axis), $"Not expected axis type value: {axis}"),
        };
    }

    private double[] GetAxisData(AxisType axis)
    {
        switch (axis)
        {
            case AxisType.PMT1:
                return PeakMeasure switch
                {
                    PeakMeasureType.Avg => _dropletsDataService.DropletsData.Select(p => p.Avg1InVolts).ToArray(),
                    PeakMeasureType.AvgRaw => _dropletsDataService.DropletsData.Select(p => p.AvgUnfilter1InVolts).ToArray(),
                    PeakMeasureType.Peak => _dropletsDataService.DropletsData.Select(p => p.Max1InVolts).ToArray(),
                    PeakMeasureType.PeakRaw => _dropletsDataService.DropletsData.Select(p => p.MaxUnfilter1InVolts).ToArray(),
                };
            case AxisType.PMT2:
                return PeakMeasure switch
                {
                    PeakMeasureType.Avg => _dropletsDataService.DropletsData.Select(p => p.Avg2InVolts).ToArray(),
                    PeakMeasureType.AvgRaw => _dropletsDataService.DropletsData.Select(p => p.AvgUnfilter2InVolts).ToArray(),
                    PeakMeasureType.Peak => _dropletsDataService.DropletsData.Select(p => p.Max2InVolts).ToArray(),
                    PeakMeasureType.PeakRaw => _dropletsDataService.DropletsData.Select(p => p.MaxUnfilter2InVolts).ToArray(),
                };
            case AxisType.Ratio:
                return PeakMeasure switch
                {
                    PeakMeasureType.Avg => _dropletsDataService.DropletsData.Select(p => p.RatioAvg).ToArray(),
                    PeakMeasureType.AvgRaw => _dropletsDataService.DropletsData.Select(p => p.RatioAvgUnfilter).ToArray(),
                    PeakMeasureType.Peak => _dropletsDataService.DropletsData.Select(p => p.RatioMax).ToArray(),
                    PeakMeasureType.PeakRaw => _dropletsDataService.DropletsData.Select(p => p.RatioMaxUnfilter).ToArray(),
                };
            default:
                throw new ArgumentOutOfRangeException(nameof(axis), $"Not expected axis type value: {axis}");
        }
    }


    private void OnDropletsDataChanged()
    {
        var binsX = 1500;
        var binsY = 1500;
        var min = 0.0;
        var max = 5.0;

        var arrayX = GetAxisData(AxisX);
        var arrayY = GetAxisData(AxisY);


        // Calculate the bin edges for the x and y axes
        var histX = new ScottPlot.Statistics.Histogram(min, max, binsX);
        histX.AddRange(arrayX);
        double[] xEdges = histX.Bins;
        var histY = new ScottPlot.Statistics.Histogram(min, max, binsY);
        histY.AddRange(arrayY);
        double[] yEdges = histY.Bins;

        // Create an empty 2D array to store the bin counts
        // needed nullable array to represent empty bin as transparent 
        // https://scottplot.net/cookbook/4.1/category/plottable-heatmap/#heatmap-with-empty-squares
        double?[,] counts = new double?[binsX, binsY];


        // Loop through each point and increment the appropriate bin
        for(var i=0;i< arrayX.Length;i++)
        {
            int xBin = Array.BinarySearch(xEdges, arrayX[i]);
            if (xBin < 0)
                xBin = ~xBin - 1;

            int yBin = Array.BinarySearch(yEdges, arrayY[i]);
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
}

