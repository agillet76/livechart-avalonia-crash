
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SkiaSharp;

namespace TestLiveCharts.ViewModels;

public class DataViewModel : ReactiveObject
{
    [Reactive] public IDataProvider DataProvider { get; set; }

    private int _keepDataRecords = 200;

    readonly ObservableAsPropertyHelper<double> currentValue;
    public double CurrentValue => currentValue.Value;


    public ObservableCollection<double> _latestData = new ObservableCollection<double>();
    
 
    public ObservableCollection<ISeries> SeriesData { get; set; } = new ObservableCollection<ISeries>
    {
        new LineSeries<double>
        {
            Name = "Data",
            Fill = null,
            Stroke = new SolidColorPaint(SKColors.LightGreen,1),
            GeometryStroke =  new SolidColorPaint(SKColors.LightGreen,1),
            GeometrySize = 1
        }
    };

  

    public List<Axis> XAxes { get; set; } = new List<Axis>
    {
        new Axis
        {
            Name = "Time",
            Labeler = d => null,
            TextSize = 8,
            NameTextSize = 8,
            NamePaint = new SolidColorPaint { Color = SKColors.Gray },
        }
    };
    public List<Axis> YAxes { get; set; } = new List<Axis>
    {
        new Axis
        {
            Name = "Data",
            MinLimit = 0,
            MaxLimit = 1000,
            TextSize = 8,
            NameTextSize = 8,
            NamePaint = new SolidColorPaint { Color = SKColors.Gray },
        }
    };

  
    public object Sync { get; }

    public DataViewModel() 
    {
        DataProvider = new DataProvider();

        Sync = _latestData;
        SeriesData[0].Values = _latestData;

        var currentVal = this.WhenAnyValue(x => x.DataProvider.CurrentValue).ObserveOn(RxApp.MainThreadScheduler);
        currentValue = currentVal.ToProperty(this, x => x.CurrentValue);
        //currentValue = currentVal.Sample(TimeSpan.FromMilliseconds(500)).ToProperty(this, x => x.CurrentValue);

        //currentVal.Sample(TimeSpan.FromMilliseconds(500)).Subscribe(x =>
        currentVal.Subscribe( x =>
        {
            // this is called on the main thread
            lock (Sync)
            {
                var formatX = Math.Round(x, 2);

                _latestData.Add(formatX);
                if (_latestData.Count > _keepDataRecords)
                    _latestData.RemoveAt(0);

                if (formatX > YAxes[0].MaxLimit)
                    YAxes[0].MaxLimit = formatX;
            }
        });
    }
}
