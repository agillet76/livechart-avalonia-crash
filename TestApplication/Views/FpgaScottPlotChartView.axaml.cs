
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ScottPlot.Avalonia;
using Splat;
using TestLiveCharts.ViewModels;
using TestLiveCharts.Models;

namespace TestLiveCharts.Views;

public partial class FpgaScottPlotChartView : Avalonia.ReactiveUI.ReactiveUserControl<FpgaScottPlotChartViewModel>
{
    AvaPlot _avaScottPlot;
    double[] _timeTraceDataX = new double[1000]; //display 1 minute worth of data at a 4 Hz update rate
    double[] _timeTraceDataY = new double[1000];

    public FpgaScottPlotChartView()
    {
        InitializeComponent();
        DataContext = Locator.Current.GetService<FpgaScottPlotChartViewModel>();

        ((FpgaScottPlotChartViewModel)DataContext).RegisterTimeTraceUpdateAction(OnTimeTraceDataChanged);

        //_avaScottPlot = this.FindControl<AvaPlot>("AvaScottPlot");

        _avaScottPlot = this.Find<AvaPlot>("AvaScottPlot");

        _avaScottPlot.Plot.AddScatter(_timeTraceDataX, _timeTraceDataY,lineWidth:0);
        _avaScottPlot.Plot.AxisAutoX(margin: 0);

        _avaScottPlot.Plot.XAxis.Label("Time (s)", size: 11);
        _avaScottPlot.Plot.YAxis.Label("Amplitude [V]", size: 11);
        
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnTimeTraceDataChanged(TimeTraceData[] newTimeTraceData)
    {
        if(newTimeTraceData == null || newTimeTraceData.Length ==0) return;
        var sw = Stopwatch.StartNew();
        var pmt1Array = newTimeTraceData.Select(x => x.PMTsVolts[0]).ToArray();
        var timeArray = newTimeTraceData.Select(x => x.TimestampMs).ToArray();
        Console.Error.WriteLine($"Scott Chart Select {sw.ElapsedMilliseconds}");
        sw.Restart();
       
        if (timeArray.Length != _timeTraceDataX.Length)
        {
            Array.Resize(ref _timeTraceDataY, pmt1Array.Length);
            Array.Resize(ref _timeTraceDataX, timeArray.Length);
            _avaScottPlot.Plot.Clear();
            _avaScottPlot.Plot.AddScatter(_timeTraceDataX, _timeTraceDataY, lineWidth: 0);

        }
        Array.Copy(pmt1Array, 0, _timeTraceDataY, 0, _timeTraceDataY.Length);
        Array.Copy(timeArray, 0, _timeTraceDataX, 0, _timeTraceDataX.Length);
        Console.Error.WriteLine($"Scott Chart Array Copy {sw.ElapsedMilliseconds}");
        sw.Restart();
       
        _avaScottPlot.Plot.SetAxisLimitsX(timeArray.First(),timeArray.Last());
        _avaScottPlot.Refresh();
        Console.Error.WriteLine($"Scott Chart Refresh {sw.ElapsedMilliseconds} \n");
        sw.Stop();
    }
}

