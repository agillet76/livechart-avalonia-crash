using System.Diagnostics;
using TestLiveCharts.Models;

namespace TestLiveCharts.ViewModels;

public class FpgaScottPlotChartViewModel : FpgaChartViewModel
{

    protected Action<TimeTraceData[]> _updateTimeTraceDataAction;
    private Stopwatch _sw = new ();

    protected override void UpdateChart(int startIndexData)
    {
        base.UpdateChart(startIndexData);
        _updateTimeTraceDataAction?.Invoke(LatestData);
        Console.Error.WriteLine($"ScottPlot Chart Update Trigger {_sw.ElapsedMilliseconds}");
        
    }

    public void RegisterTimeTraceUpdateAction(Action<TimeTraceData[]> action)
    {
        _updateTimeTraceDataAction = action;
    }
}

