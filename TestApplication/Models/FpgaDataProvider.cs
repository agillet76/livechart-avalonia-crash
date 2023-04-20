using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
    private object _lock = new object();

    public bool Disposed { get; private set; }

    public int TimeTraceLength
    {
        get => _timeTraceLength;
        set
        {
            _timeTraceLength = value;
            _nextTimeTraceDataIndex = 0;
            TimeTraceDataFlatArray = Enumerable.Range(0, _maxDataArrayToRetain * _timeTraceLength).Select(_ => new TimeTraceData(PmtChannelCount)).ToArray();
            OnPropertyChanged(nameof(TimeTraceLength));
        }
    }
    private int _timeTraceLength = 10000;

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

    public TimeTraceData[] TimeTraceDataFlatArray { get; set; }
    public int LastTimeTraceDataIndex
    {
        get => _lastTimeTraceDataIndex;
        set
        {
            _lastTimeTraceDataIndex = value;
            OnPropertyChanged(nameof(LastTimeTraceDataIndex));
        }
    }
    private int _lastTimeTraceDataIndex = 0;
    private int _nextTimeTraceDataIndex = 0;
    private Thread _dataPollingThread;
    private ManualResetEvent _waitThread = new ManualResetEvent(true);
    
    public SimulatedFpga() 
    {
        TimeTraceDataFlatArray = Enumerable.Range(0, _maxDataArrayToRetain * _timeTraceLength).Select(_ => new TimeTraceData(PmtChannelCount)).ToArray();

        _dataPollingThread = new Thread(DataPollingThreadProc);
        _dataPollingThread.Start();

    }

    private void DataPollingThreadProc()
    {
        
        while (true)
        {
            _waitThread.WaitOne();
            RuntimeInSeconds = _runtimeWatch.Elapsed.TotalSeconds;
            ReadTimeTraceData();
            Thread.Sleep(_dataPollingIntervalMs);
        }
    }

    public void Connect()
    {
        try
        {
            Thread.Sleep(500);
            _runtimeWatch.Reset();
            RuntimeInSeconds = 0;
            _timestampStart = 0;
            _runtimeWatch.Restart();
            IsConnected = true;
            _waitThread.Set();
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
            Thread.Sleep(500);
            
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Unable to Connect to FPGA", ex);
        }
        finally
        {
            _runtimeWatch.Stop();
            _waitThread.Reset();
            IsConnected = false;
        }
    }

    private void ReadTimeTraceData()
    {
        try
        {
            for (int i = 0; i < TimeTraceLength; i++)
            {
                UpdateTimeTraceData(TimeTraceDataFlatArray[_nextTimeTraceDataIndex + i]);
                TimeTraceDataFlatArray[_nextTimeTraceDataIndex + i].TimestampMs = _timestampStart;
                _timestampStart++;
            }
            LastTimeTraceDataIndex = _nextTimeTraceDataIndex;


            if (_lastTimeTraceDataIndex + TimeTraceLength >= TimeTraceDataFlatArray.Length)
            {
                _nextTimeTraceDataIndex = 0;
            }
            else
            {
                _nextTimeTraceDataIndex += TimeTraceLength;
            }
        }
        catch (Exception ex)
        {
            var message = "Unable to read the time trace data";
            throw new ApplicationException(message, ex);
        }
    }

    private void UpdateTimeTraceData(TimeTraceData data)
    {
        // 0 - Timestamp (ms)
        // 1 - 4 - PMTs(Volts)
        // 5 - 8 - PMTs(Pre - Median Filter)(Volts)
        // 9 - 12 - Analog Output Channels(Volts)
        // 13 - 20 - Digital TTL Outputs(as 0 or 1)

        for (var i = 0; i < PmtChannelCount; i++)
        {
            var val = Math.Round(_random.NextDouble(), 2);
            data.PMTsVolts[i]=val;
            data.PreFilterPMTsVolts[i] = val;
        }
    }

    private TimeTraceData GetTimeTraceData()
    {
        // 0 - Timestamp (ms)
        // 1 - 4 - PMTs(Volts)
        // 5 - 8 - PMTs(Pre - Median Filter)(Volts)
        // 9 - 12 - Analog Output Channels(Volts)
        // 13 - 20 - Digital TTL Outputs(as 0 or 1)
        var timeTraceData = new TimeTraceData(PmtChannelCount);
        
        for (var i = 0; i < PmtChannelCount; i++)
        {
            var val = Math.Round(_random.NextDouble(), 2);
            timeTraceData.PMTsVolts[i]=val;
            timeTraceData.PreFilterPMTsVolts[i]=val;
        }

        //timeTraceData.AnalogOutputChannlesVolts.AddRange(new double[] { 0, 0, 0, 0 });
        //timeTraceData.DigitalTTLOutputs.AddRange(new bool[] { true, false, false, false, false, false, false, false });


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

