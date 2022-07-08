using System.Collections.ObjectModel;
using ReactiveUI;

namespace TestLiveCharts.ViewModels;

public class DataControllerViewModel : ReactiveObject
{
    public ObservableCollection<DataViewModel> DataViewModels { get; set; }
    
    
    public DataControllerViewModel()
    {
        DataViewModels = new ObservableCollection<DataViewModel>();

        for (var i=0; i< 16; i++)
        {
            DataViewModels.Add(new DataViewModel());
        }
    }
}

