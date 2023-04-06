
using System.Collections.ObjectModel;
using System.ComponentModel;
using TestLiveCharts.Models;

namespace TestLiveCharts.ViewModels;

public interface IFpgaDataProvider : INotifyPropertyChanged
{
    double RuntimeInSeconds { get; }
    ObservableCollection<TimeTraceData[]> TimeTraceDataList { get; }
    bool IsConnected { get; }
    void Connect();
    void Disconnect();
}