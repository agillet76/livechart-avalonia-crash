using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using ReactiveUI;
using SkiaSharp;
using System.Diagnostics;
using System.Reactive.Linq;
using DynamicData.Binding;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using TestLiveCharts.Models;
using System.Reactive;
using ReactiveUI.Fody.Helpers;

namespace TestLiveCharts.ViewModels;

public class FpgaChartViewModel : ReactiveObject, IActivatableViewModel
{
    public object DataSync { get; set; }
    public ViewModelActivator Activator { get; }
    public RangeObservableCollection<TimeTraceData> _latestData = new();
    public DrawMarginFrame DrawMarginFrame => new DrawMarginFrame
    {
        Fill = new SolidColorPaint(SKColors.LightSlateGray),
        Stroke = new SolidColorPaint(SKColors.Black, 3),

    };
    public ObservableCollection<ISeries> DataSeries { get; set; } = new ObservableCollection<ISeries>
    {
        new LineSeries<TimeTraceData>
        {
            Name = "PMT1",
            Fill = null,
            Stroke = new SolidColorPaint(SKColors.LightGreen,3),
            GeometryStroke =  new SolidColorPaint(SKColors.LightGreen,1),
            GeometrySize = 1,
            Mapping = (data, point) =>
            {
                // use the Population property in this series
                point.PrimaryValue = data.PMTsVolts[0];
                point.SecondaryValue = point.Context.Index;
            },
            
        },
        new LineSeries<TimeTraceData>
        {
            Name = "PMT2",
            Fill = null,
            Stroke = new SolidColorPaint(SKColors.LightBlue,3),
            GeometryStroke =  new SolidColorPaint(SKColors.LightBlue,1),
            GeometrySize = 1,
            Mapping = (data, point) =>
            {
                // use the Population property in this series
                point.PrimaryValue = data.PMTsVolts[1];
                point.SecondaryValue = point.Context.Index;
            }
        },
        new LineSeries<TimeTraceData>
        {
            Name = "Pre Filter PMT1",
            Fill = null,
            Stroke = new SolidColorPaint(SKColors.LightSalmon,3),
            GeometryStroke =  new SolidColorPaint(SKColors.LightSalmon,1),
            GeometrySize = 1,
            Mapping = (data, point) =>
            {
                // use the Population property in this series
                point.PrimaryValue = data.PreFilterPMTsVolts[0];
                point.SecondaryValue = point.Context.Index;
            }
        },
        new LineSeries<TimeTraceData>
        {
            Name = "Pre Filter PMT2",
            Fill = null,
            Stroke = new SolidColorPaint(SKColors.LightYellow,3),
            GeometryStroke =  new SolidColorPaint(SKColors.LightYellow,1),
            GeometrySize = 1,
            Mapping = (data, point) =>
            {
                // use the Population property in this series
                point.PrimaryValue = data.PreFilterPMTsVolts[1];
                point.SecondaryValue = point.Context.Index;
            }
        },
        new LineSeries<TimeTraceData>
        {
            Name = "Analog Out 1",
            Fill = null,
            Stroke = new SolidColorPaint(SKColors.LightPink,3),
            GeometryStroke =  new SolidColorPaint(SKColors.LightPink,1),
            GeometrySize = 1,
            Mapping = (data, point) =>
            {
                // use the Population property in this series
                point.PrimaryValue = data.AnalogOutputChannlesVolts[0];
                point.SecondaryValue = point.Context.Index;
            }
        },
    };
    public List<Axis> XAxes { get; set; } = new List<Axis>
    {
        new Axis
        {
            Name = "Time [ms]",
            //MinLimit = 0,
            //Labeler = d => null,
            TextSize = 10,
            NameTextSize = 10,
            NamePaint = new SolidColorPaint { Color = SKColors.White },
            LabelsPaint = new SolidColorPaint { Color = SKColors.White },
            ShowSeparatorLines = false,
            SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray) { StrokeThickness = 2 },
            TicksPaint = new SolidColorPaint { Color = SKColors.White },
        }
    };
    public List<Axis> YAxes { get; set; } = new List<Axis>
    {
        new Axis
        {
            Name = "Amplitude [V]",
            MinLimit = -0.5,
            MaxLimit = 5,
            TextSize = 10,
            NameTextSize = 10,
            NamePaint = new SolidColorPaint { Color = SKColors.White },
            LabelsPaint = new SolidColorPaint { Color = SKColors.White },
            ShowSeparatorLines = true,
            SeparatorsPaint = new SolidColorPaint { Color = SKColors.DarkSlateGray, StrokeThickness = 2, 
                PathEffect = new DashEffect(new float[]{3,3})},
            TicksPaint = new SolidColorPaint { Color = SKColors.White },
        }
    };

    public ReactiveCommand<Unit, Task> ConnectCommand { get; }
    public ReactiveCommand<Unit, Task> DisconnectCommand { get; }

    [Reactive] public IFpgaDataProvider Fpga { get; set; }

    public FpgaChartViewModel()
    {
        Activator = new ViewModelActivator();
        Fpga = new SimulatedFpga();

        var isConnected = this.WhenAnyValue(x => x.Fpga.IsConnected).ObserveOn(RxApp.MainThreadScheduler);
       
        ConnectCommand = ReactiveCommand.Create(async () =>
        {
            try
            {
                await Task.Run(() =>
                {
                    Fpga.Connect();

                }).ConfigureAwait(true);
            }
            catch (Exception)
            {
                Console.Error.WriteLine("Failed Start Monitoring");
            }
        }, isConnected.Select(x => !x));

        DisconnectCommand = ReactiveCommand.Create(async () =>
        {
            try
            {
                await Task.Run(() =>
                {
                    Fpga.Disconnect();

                }).ConfigureAwait(true);
            }
            catch (Exception)
            {
                Console.Error.WriteLine("Failed Stop Monitoring");
            }
        }, isConnected);

        //https://stackoverflow.com/questions/63138397/livecharts-wpf-slow-with-live-data-improve-livecharts-real-time-plotting-perfor
        // https://www.reactiveui.net/docs/handbook/collections/
        // https://stackoverflow.com/questions/60330908/observablechangeset-wait-until-list-is-ready-before-watching
        DataSync = _latestData;
        DataSeries[0].Values = _latestData;
        DataSeries[1].Values = _latestData;
        DataSeries[2].Values = _latestData;
        DataSeries[3].Values = _latestData;
        DataSeries[4].Values = _latestData;

        // var t = Fpga.ConnectToData()
        //     .ObserveOn(RxApp.MainThreadScheduler)
        //     .Throttle(TimeSpan.FromSeconds(1))
        //     .Bind(out _items);

        this.WhenActivated(async (CompositeDisposable disposables) =>
        {

            // t.Subscribe(x =>
            // {
            //     lock (DataSync)
            //     {
            //         _latestData.Add(_items.Last());
            //         
            //         if (_latestData.Count > _keepDataRecords)
            //             _latestData.RemoveAt(0);
            //     }
            // }).DisposeWith(disposables);




            // var t = Fpga.TimeTraceDataList
            //     // Convert the collection to a stream of chunks,
            //     // so we have IObservable<IChangeSet<TKey, TValue>>
            //     // type also known as the DynamicData monad.
            //     .ToObservableChangeSet(x => x)
            //     .ObserveOn(RxApp.MainThreadScheduler);
            // t.Subscribe(_ =>
            // {
            //     Log.Debug($"Adding to Chart");
            //     lock (DataSync)
            //     {
            //         if (Fpga.TimeTraceDataList.Any())
            //         {
            //             var data = Fpga.TimeTraceDataList.Last();
            //             _latestData.Add(data);
            //         }
            //         if (_latestData.Count > _keepDataRecords)
            //             _latestData.RemoveAt(0);
            //     }
            // }).DisposeWith(disposables);

            var t = Fpga.TimeTraceDataList
                // Convert the collection to a stream of chunks,
                // so we have IObservable<IChangeSet<TKey, TValue>>
                // type also known as the DynamicData monad.
                .ToObservableChangeSet().Sample(TimeSpan.FromSeconds(1)).Select(x=>x)
                .ObserveOn(RxApp.MainThreadScheduler);

            t.Subscribe(c =>
            {
                var sw = Stopwatch.StartNew();
               //Log.Debug($"Adding to Chart, totalchanges{c.TotalChanges}");
                if (c.Count > 0)
                {
                    var newItems = c.AsEnumerable().FirstOrDefault().Range.ToList();
                    lock (DataSync)
                    {
                        _latestData.Clear();
                        _latestData.AddRange(newItems);
                    }
                }
                sw.Stop();
                
            }).DisposeWith(disposables);
        });
    }
}

