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
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.VisualElements;

namespace TestLiveCharts.ViewModels;

public class FpgaChartViewModel : ReactiveObject, IActivatableViewModel
{
    public object DataSync { get; set; }
    public ViewModelActivator Activator { get; }
    public ObservableCollection<TimeTraceData[]> _latestData { get; set; } = new ();

    protected ObservableAsPropertyHelper<bool> isConnected; 
    public bool IsConnected => isConnected.Value;

    public DrawMarginFrame DrawMarginFrame => new DrawMarginFrame
    {
        Fill = new SolidColorPaint(SKColors.LightSlateGray),
        Stroke = new SolidColorPaint(SKColors.Black, 3),

    };


    private static int _strokeThickness = 1;
    private static float[] _strokeDashArray = new float[] { 3 * _strokeThickness, 2 * _strokeThickness };
    private DashEffect _effect = new DashEffect(_strokeDashArray);


    public ObservableCollection<ISeries> DataSeries { get; set; } = new ObservableCollection<ISeries>
    {
        new LineSeries<TimeTraceData>
        {
            Name = "PMT1",
            Fill = null,
            DataPadding = new LvcPoint(0.2f, 0),
            Stroke = null,
            // Stroke = new SolidColorPaint  {
            //     Color = SKColors.LightGreen,
            //     StrokeCap = SKStrokeCap.Round,
            //     StrokeThickness = _strokeThickness,
            //     PathEffect = new DashEffect(_strokeDashArray)
            // },
      
            GeometryStroke =  new SolidColorPaint(SKColors.LightGreen,1),
            LineSmoothness = 1,
            GeometrySize = 5,
            Mapping = (data, point) =>
            {
                // use the Population property in this series
                point.PrimaryValue = data.PMTsVolts[0];
                //point.SecondaryValue = point.Context.Index;
                point.SecondaryValue = data.TimestampMs;
            },
        },
        // new LineSeries<TimeTraceData>
        // {
        //     Name = "PMT2",
        //     Fill = null,
        //     Stroke = new SolidColorPaint(SKColors.LightBlue,3),
        //     GeometryStroke =  new SolidColorPaint(SKColors.LightBlue,1),
        //     GeometrySize = 1,
        //     Mapping = (data, point) =>
        //     {
        //         // use the Population property in this series
        //         point.PrimaryValue = data.PMTsVolts[1];
        //         point.SecondaryValue = data.TimestampMs;
        //         
        //     }
        // },
        // new LineSeries<TimeTraceData>
        // {
        //     Name = "Pre Filter PMT1",
        //     Fill = null,
        //     Stroke = new SolidColorPaint(SKColors.LightSalmon,3),
        //     GeometryStroke =  new SolidColorPaint(SKColors.LightSalmon,1),
        //     GeometrySize = 1,
        //     Mapping = (data, point) =>
        //     {
        //         // use the Population property in this series
        //         point.PrimaryValue = data.PreFilterPMTsVolts[0];
        //         point.SecondaryValue = data.TimestampMs;
        //         //point.SecondaryValue = point.Context.Index;
        //     }
        // },
        // new LineSeries<TimeTraceData>
        // {
        //     Name = "Pre Filter PMT2",
        //     Fill = null,
        //     Stroke = new SolidColorPaint(SKColors.LightYellow,3),
        //     GeometryStroke =  new SolidColorPaint(SKColors.LightYellow,1),
        //     GeometrySize = 1,
        //     Mapping = (data, point) =>
        //     {
        //         // use the Population property in this series
        //         point.PrimaryValue = data.PreFilterPMTsVolts[1];
        //         point.SecondaryValue = data.TimestampMs;
        //         //point.SecondaryValue = point.Context.Index;
        //     }
        // },
        // new LineSeries<TimeTraceData>
        // {
        //     Name = "Analog Out 1",
        //     Fill = null,
        //     Stroke = new SolidColorPaint(SKColors.LightPink,3),
        //     GeometryStroke =  new SolidColorPaint(SKColors.LightPink,1),
        //     GeometrySize = 1,
        //     Mapping = (data, point) =>
        //     {
        //         // use the Population property in this series
        //         point.PrimaryValue = data.AnalogOutputChannlesVolts[0];
        //         point.SecondaryValue = data.TimestampMs;
        //         //point.SecondaryValue = point.Context.Index;
        //     }
        // },
    };

    [Reactive]
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

    [Reactive]
    public LabelVisual Title { get; set; } =
        new LabelVisual
        {
            Text = "Time Series Data",
            TextSize = 25,
            Padding = new Padding(15),
            Paint = new SolidColorPaint(SKColors.White)
        };

    public ReactiveCommand<Unit, Unit> ConnectCommand { get; }
    public ReactiveCommand<Unit, Unit> DisconnectCommand { get; }

    [Reactive] public IFpgaDataProvider Fpga { get; set; }
    [Reactive] public double XMaxLimit { get; set; } = 1;
    [Reactive] public double XMinLimit { get; set; } = 0;

    private Stopwatch _sw = new Stopwatch();
    private IObservable<TimeTraceData[]> _obsTimeTraceData;
    public FpgaChartViewModel()
    {
        Activator = new ViewModelActivator();
        Fpga = new SimulatedFpga();

        var isConnectedObs = this.WhenAnyValue(x => x.Fpga.IsConnected).ObserveOn(RxApp.MainThreadScheduler);
        isConnected = isConnectedObs.ToProperty(this, x => x.IsConnected);

        ConnectCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                await Task.Run(() =>
                {
                    Fpga.Connect();
                    _sw.Restart();

                }).ConfigureAwait(true);
            }
            catch (Exception)
            {
                Console.Error.WriteLine("Failed Start Monitoring");
            }
        }, isConnectedObs.Select(x => !x));

        DisconnectCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                await Task.Run(() =>
                {
                   
                    Fpga.Disconnect();
                    _sw.Stop();
                    _latestData.Clear();

                }).ConfigureAwait(true);
            }
            catch (Exception)
            {
                Console.Error.WriteLine("Failed Stop Monitoring");
            }
        }, isConnectedObs);

        //https://stackoverflow.com/questions/63138397/livecharts-wpf-slow-with-live-data-improve-livecharts-real-time-plotting-perfor
        // https://www.reactiveui.net/docs/handbook/collections/
        // https://stackoverflow.com/questions/60330908/observablechangeset-wait-until-list-is-ready-before-watching

        //2nd option using ChangeEvent
        // var t = Fpga.TimeTraceDataList
        //         // Convert the collection to a stream of chunks,
        //         // so we have IObservable<IChangeSet<TKey, TValue>>
        //         // type also known as the DynamicData monad.
        //         .ToObservableChangeSet().Sample(TimeSpan.FromSeconds(1)).Select(x => x)
        //         .ObserveOn(RxApp.MainThreadScheduler)
        //         .SubscribeOn(RxApp.MainThreadScheduler);

        this.WhenActivated(disposables=>
        {

            HandleActivation(disposables);
            Disposable
                .Create(() => { /* handle deactivation */ })
                .DisposeWith(disposables);
        });
    }

    void HandleActivation(CompositeDisposable disposables)
    {
        var t = this.WhenAnyValue(x => x.Fpga.TimeTraceDataRetrieve)
            .Sample(TimeSpan.FromSeconds(1))
            .ObserveOn(RxApp.MainThreadScheduler);

        t.Subscribe(c =>
        {
            // if (c.Count > 0)
            // {
            var newItems = Fpga.TimeTraceDataList.LastOrDefault();
            UpdateChart(newItems);
            //}
        }).DisposeWith(disposables);
    }


    protected virtual void UpdateChart(TimeTraceData[]? data)
    {
        if (data != null)
        {
            DataSeries[0].Values = data;
            XAxes[0].MinLimit = data.First().TimestampMs;
            XAxes[0].MaxLimit = data.Last().TimestampMs;
            Console.Error.WriteLine($"LVC Chart Update Trigger {_sw.ElapsedMilliseconds}");
        }
    }
}

