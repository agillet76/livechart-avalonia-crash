using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Timers;
using TestLiveCharts.ViewModels;

namespace TestLiveCharts.Models;

public class SimulatedFpga : IFpgaDataProvider, IDisposable
{
    private int TimeTraceLength = 1000;
    private int DataPollingIntervalMs = 500;

    private Stopwatch _runtimeWatch = new();
    protected System.Timers.Timer _pollingTimer;
    // the NI FPGA is configured with 4 PMT channels PMT1, PMT2, PMT3, PMT4
    // however we only use PMT1 and PMT2 but we need to account for all of them retrieving 
    // data from the fpga
    private const int PmtChannelCount = 4;

    private readonly Random _random = new();

    public bool Disposed { get; private set; }

    private int _timestampStart = 0;
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

    public RangeObservableCollection<TimeTraceData> TimeTraceDataList { get; set; } = new();
    

    public SimulatedFpga() 
    {
        
        _pollingTimer = new System.Timers.Timer(DataPollingIntervalMs)
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
            Thread.Sleep(3000);
            sw.Stop();
            RuntimeInSeconds = 0;
            
            _runtimeWatch.Start();
            IsConnected = true;
            _pollingTimer.Enabled = true;
            
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
            _pollingTimer.Enabled = false;
            // Closes and resets the FPGA device. Must be called at the end of use.
            Thread.Sleep(3000);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Unable to Connect to FPGA", ex);
        }
        finally
        {
            _runtimeWatch.Reset();
            IsConnected = false;
        }
    }

    private void ReadTimeTraceData()
    {
        try
        {
            var convertedDataList = new List<TimeTraceData>();
            for (int i = 0; i < TimeTraceLength; i++)
            {
                var timeTraceData = GetTimeTraceData();
                timeTraceData.TimestampMs = _timestampStart;
                _timestampStart++;
                convertedDataList.Add(timeTraceData);
            }
            TimeTraceDataList.AddRange(convertedDataList);
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

