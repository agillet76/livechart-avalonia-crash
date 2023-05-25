using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using DynamicData.Binding;
using FpgaScatterPlotTest.Models;
using FpgaScatterPlotTest.Services;
using ReactiveUI;
using ScottPlot.Avalonia;


namespace FpgaScatterPlotTest.CustomsControls;


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

public class DropletScatterPlot:TemplatedControl, IActivatableView
{

    public Array PeakMeasureSupported => Enum.GetValues(typeof(PeakMeasureType));
    public Array AxisTypeSupported => Enum.GetValues(typeof(AxisType));
    
    public static readonly StyledProperty<AxisType> AxisXProperty =
        AvaloniaProperty.Register<DropletScatterPlot, AxisType>(
            nameof(AxisX),
            defaultValue: AxisType.PMT1);
    public AxisType AxisX
    {
        get { return GetValue(AxisXProperty); }
        set { SetValue(AxisXProperty, value); }
    }

    public static readonly StyledProperty<AxisType> AxisYProperty =
        AvaloniaProperty.Register<DropletScatterPlot, AxisType>(
            nameof(AxisY),
            defaultValue: AxisType.PMT2);
    public AxisType AxisY
    {
        get { return GetValue(AxisYProperty); }
        set { SetValue(AxisYProperty, value); }
    }

    public static readonly StyledProperty<PeakMeasureType> PeakMeasureProperty =
        AvaloniaProperty.Register<DropletScatterPlot, PeakMeasureType>(
            nameof(PeakMeasure),
            defaultValue: PeakMeasureType.Peak);
    public PeakMeasureType PeakMeasure
    {
        get { return GetValue(PeakMeasureProperty); }
        set { SetValue(PeakMeasureProperty, value); }
    }

    public static readonly StyledProperty<Polygon> PolygonItemProperty =
        AvaloniaProperty.Register<DropletScatterPlot, Polygon>(
            nameof(PolygonItem),
            defaultValue: null);
    public Polygon PolygonItem
    {
        get { return GetValue(PolygonItemProperty); }
        set { SetValue(PolygonItemProperty, value); }
    }

    public static readonly StyledProperty<string> HeaderProperty =
        AvaloniaProperty.Register<DropletScatterPlot, string>(
            nameof(Header),
            defaultValue: "");
    public string Header
    {
        get { return GetValue(HeaderProperty); }
        set { SetValue(HeaderProperty, value); }
    }

    public static readonly DirectProperty<DropletScatterPlot, IDropletsDataService> DropletDataProviderProperty =
        AvaloniaProperty.RegisterDirect<DropletScatterPlot, IDropletsDataService>(
            nameof(DropletDataProvider),
            o => o.DropletDataProvider,
            (o, v) => o.DropletDataProvider = v);

    private IDropletsDataService _dropletDataProvider;
    private RangeObservableCollection<DropletData>? _dropletData => _dropletDataProvider?.DropletsData;

    public IDropletsDataService DropletDataProvider
    {
        get { return _dropletDataProvider; }
        set { SetAndRaise(DropletDataProviderProperty, ref _dropletDataProvider, value); }
    }

    private AvaPlot? _avaPlot { get; set; }

    public ViewModelActivator Activator { get; } = new ViewModelActivator();

    public DropletScatterPlot()
    {
        this.WhenActivated((CompositeDisposable disposables) =>
        {
            /* handle activation */

            var changeSet = _dropletDataProvider.DropletsData.ToObservableChangeSet();
            changeSet.Subscribe(changes =>
            {
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


    // We override what happens when the control template is being applied. 
    // That way we can for example listen to events of controls which are part of the template
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        // try to find the control with the given name
        _avaPlot = e.NameScope.Find("DropletPlot") as AvaPlot;
        UpdateChart();
    }

    // We override OnPropertyChanged of the base class. That way we can react on property changes
    protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
    {
        base.OnPropertyChanged(change);

        // if the changed property is the NumberOfStarsProperty, we need to update the stars
       if( change.Property == AxisXProperty || change.Property == AxisYProperty || change.Property == PeakMeasureProperty)
       {
            UpdateChart();
       }
    }

    protected void UpdateChart()
    {
        if (_avaPlot is null) return;
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            _avaPlot.Plot.Title(PolygonItem == null ? "Not Set" : PolygonItem.Name);
            var (labelX, labelY) = GetAxisLabel();
            _avaPlot.Plot.XAxis.Label(labelX, size: 11);
            _avaPlot.Plot.YAxis.Label(labelY, size: 11);
            if (_dropletDataProvider?.DropletsData.Count > 0) RefreshPlot();
            _avaPlot.Refresh();
        });
    }

    private (string, string) GetAxisLabel()
    {
        var unitX = GetAxisLabelUnit(AxisX);
        var unitY = GetAxisLabelUnit(AxisX);
        var peakMeasure = GetAxisLabelPeakMeasure(PeakMeasure);
        var labelX = $"{AxisX} {peakMeasure} {unitX}";
        var labelY = $"{AxisY} {peakMeasure} {unitY}";
        return (labelX, labelY);
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
                    PeakMeasureType.Avg => _dropletData?.Select(p => p.Avg1InVolts).ToArray(),
                    PeakMeasureType.AvgRaw => _dropletData?.Select(p => p.AvgUnfilter1InVolts).ToArray(),
                    PeakMeasureType.Peak => _dropletData?.Select(p => p.Max1InVolts).ToArray(),
                    PeakMeasureType.PeakRaw => _dropletData?.Select(p => p.MaxUnfilter1InVolts).ToArray(),
                };
            case AxisType.PMT2:
                return PeakMeasure switch
                {
                    PeakMeasureType.Avg => _dropletData?.Select(p => p.Avg2InVolts).ToArray(),
                    PeakMeasureType.AvgRaw => _dropletData?.Select(p => p.AvgUnfilter2InVolts).ToArray(),
                    PeakMeasureType.Peak => _dropletData?.Select(p => p.Max2InVolts).ToArray(),
                    PeakMeasureType.PeakRaw => _dropletData?.Select(p => p.MaxUnfilter2InVolts).ToArray(),
                };
            case AxisType.Ratio:
                return PeakMeasure switch
                {
                    PeakMeasureType.Avg => _dropletData?.Select(p => p.RatioAvg).ToArray(),
                    PeakMeasureType.AvgRaw => _dropletData?.Select(p => p.RatioAvgUnfilter).ToArray(),
                    PeakMeasureType.Peak => _dropletData?.Select(p => p.RatioMax).ToArray(),
                    PeakMeasureType.PeakRaw => _dropletData?.Select(p => p.RatioMaxUnfilter).ToArray(),
                };
            default:
                throw new ArgumentOutOfRangeException(nameof(axis), $"Not expected axis type value: {axis}");
        }
    }


    private void RefreshPlot()
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
        for (var i = 0; i < arrayX.Length; i++)
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
        _avaPlot.Plot.Clear();
        var heatmap = _avaPlot.Plot.AddHeatmap(counts, ScottPlot.Drawing.Colormap.Turbo);
        heatmap.Update(counts, min: 1);
        heatmap.Smooth = true;
        heatmap.FlipVertically = true; // needed to match with LabView ScatterPlot
        var cb = _avaPlot.Plot.AddColorbar(heatmap);
        cb.Label = "Droplets";
        heatmap.XMin = 0.0;
        heatmap.XMax = 5.0;
        heatmap.YMin = 0.0;
        heatmap.YMax = 5.0;
        _avaPlot.Plot.SetAxisLimits(min, max, min, max);
        _avaPlot.Plot.XAxis.SetBoundary(min, max);
        _avaPlot.Plot.YAxis.SetBoundary(min, max);
    }
}
