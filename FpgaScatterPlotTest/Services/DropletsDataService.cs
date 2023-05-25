
using System.Collections.ObjectModel;
using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using CsvHelper;
using System.Globalization;
using System.IO;
using System.Linq;
using FpgaScatterPlotTest.Models;

namespace FpgaScatterPlotTest.Services;

public interface IDropletsDataService
{
    bool IsDataLoaded { get; set; }
    RangeObservableCollection<DropletData>? DropletsData { get; set; }
    void LoadData();
}

public class DropletsDataService: ReactiveObject, IDropletsDataService
{
    [Reactive] public bool IsDataLoaded { get; set; } = false;
    [Reactive] public RangeObservableCollection<DropletData>? DropletsData { get; set; } = new();


    public void LoadData()
    {
        if (IsDataLoaded) return;
        using (var reader = new StreamReader(@"C:\Users\Alexandre\Desktop\DropletsData\droplets_5min.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csv.GetRecords<DropletData>().ToList();
            DropletsData.ReplaceRange(records);
            IsDataLoaded = true;
        }
    }
}
