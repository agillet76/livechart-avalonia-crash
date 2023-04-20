
using System.Collections.ObjectModel;
using System.ComponentModel;
using TestLiveCharts.Models;

namespace TestLiveCharts.ViewModels;

public interface IFpgaDataProvider : INotifyPropertyChanged
{
    double RuntimeInSeconds { get; }
    TimeTraceData[] TimeTraceDataFlatArray { get; }
    int LastTimeTraceDataIndex { get; }
    
    int TimeTraceLength { get; }
    bool IsConnected { get; }
    void Connect();
    void Disconnect();
}