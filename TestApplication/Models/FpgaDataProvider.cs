using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Timers;
using TestLiveCharts.ViewModels;

namespace TestLiveCharts.Models;

public class SimulatedFpga : IFpgaDataProvider, IDisposable
{
   
    private readonly int _dataPollingIntervalMs = 100;
    private readonly int _maxDataArrayToRetain =10;
    private readonly Stopwatch _runtimeWatch = new();
    private readonly System.Timers.Timer _pollingTimer;
    private int _timestampStart = 0;
    private readonly Random _random = new();
    // FPGA is configured with 4 PMT channels PMT1, PMT2, PMT3, PMT4
    // Ideally I will need to display data for each channel , total data points PmtChannelCount * TimeTraceLength
    private const int PmtChannelCount = 4;

    public bool Disposed { get; private set; }

    public int TimeTraceLength
    {
        get => _timeTraceLength;
        set
        {
            _timeTraceLength = value;
            OnPropertyChanged(nameof(TimeTraceLength));
        }
    }
    private int _timeTraceLength = 1000;

    public bool IsConnected
    {
        get => _isConnected;
        set
        {
            _isConnected = value;
            OnPropertyChanged(nameof(IsConnected));
        }
    }
    private bool _isConnected;

    public double RuntimeInSeconds
    {
        get => _runtimeInSeconds;
        set
        {
            _runtimeInSeconds = value;
            OnPropertyChanged(nameof(RuntimeInSeconds));
        }
    }
    private double _runtimeInSeconds;

    public ObservableCollection<TimeTraceData[]> TimeTraceDataList { get; set; } = new();


    public SimulatedFpga() 
    {
        
        _pollingTimer = new System.Timers.Timer(_dataPollingIntervalMs)
        {
            Enabled = false,
            AutoReset = true
        };
        _pollingTimer.Elapsed += OnTimerElapsed;
    }

    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        RuntimeInSeconds = _runtimeWatch.Elapsed.TotalSeconds;
        ReadTimeTraceData();
    }

    public void Connect()
    {
        try
        {
            _runtimeWatch.Reset();
            TimeTraceDataList.Clear();
           
            var sw = Stopwatch.StartNew();
            Thread.Sleep(1000);
            sw.Stop();
            RuntimeInSeconds = 0;
            _timestampStart = 0;
            _runtimeWatch.Restart();
            IsConnected = true;
            _pollingTimer.Start();
            
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Unable to Connect to FPGA", ex);
        }
    }

    public void Disconnect()
    {
        if (!IsConnected)
        {
            return;
        }
        try
        {
            _pollingTimer.Stop();
            // Closes and resets the FPGA device. Must be called at the end of use.
            Thread.Sleep(1000);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Unable to Connect to FPGA", ex);
        }
        finally
        {
            _runtimeWatch.Stop();
            IsConnected = false;
        }
    }

    private void ReadTimeTraceData()
    {
        try
        {
            var convertedDataList = new TimeTraceData[TimeTraceLength];

            //if (TimeTraceDataList.Count > TimeTraceLength * 10) { TimeTraceDataList.Clear(); }
            for (int i = 0; i < TimeTraceLength; i++)
            {
                var timeTraceData = GetTimeTraceData();
                timeTraceData.TimestampMs = _timestampStart;
                _timestampStart++;
                convertedDataList[i]= timeTraceData;
            }
            TimeTraceDataList.Add(convertedDataList);
            if (TimeTraceDataList.Count > _maxDataArrayToRetain)
            {
                TimeTraceDataList.RemoveAt(0);
            }
        }
        catch (Exception ex)
        {
            var message = "Unable to read the time trace data";
            throw new ApplicationException(message, ex);
        }
    }

    private TimeTraceData GetTimeTraceData()
    {
        // 0 - Timestamp (ms)
        // 1 - 4 - PMTs(Volts)
        // 5 - 8 - PMTs(Pre - Median Filter)(Volts)
        // 9 - 12 - Analog Output Channels(Volts)
        // 13 - 20 - Digital TTL Outputs(as 0 or 1)
        var timeTraceData = new TimeTraceData();
        
        for (var i = 0; i < PmtChannelCount; i++)
        {
            var val = Math.Round(_random.NextDouble(), 2);
            timeTraceData.PMTsVolts.Add(val);
            timeTraceData.PreFilterPMTsVolts.Add(val);
        }

        timeTraceData.AnalogOutputChannlesVolts.AddRange(new double[] { 0, 0, 0, 0 });
        timeTraceData.DigitalTTLOutputs.AddRange(new bool[] { true, false, false, false, false, false, false, false });


        return timeTraceData;
    }
    public void Dispose()
    {
        if (Disposed) return;
        Disposed = true;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

