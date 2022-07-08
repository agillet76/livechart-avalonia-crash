
using System.ComponentModel;

namespace TestLiveCharts.ViewModels;
public interface IDataProvider : INotifyPropertyChanged
{
    double CurrentValue { get; }
}
