
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using JetBrains.Annotations;
using TestLiveCharts.ViewModels;
using Timer = System.Timers.Timer;

namespace TestLiveCharts;

public class DataProvider : IDataProvider
{
    private readonly Random _random = new();
    private double _targetSetPoint;
    protected Timer _pollTimer;

    public double CurrentValue
    {
        get => _currentPressureValue;
        set
        {
            _currentPressureValue = value;
            OnPropertyChanged(nameof(CurrentValue));
        }
    }

    private double _currentPressureValue;

    private int _tickCount;

    public DataProvider()
    {
        _targetSetPoint = _random.NextInt64(1, 10) * 1000;

        _pollTimer = new Timer(50);
        _pollTimer.Elapsed += OnPollTimerTick;
        _pollTimer.AutoReset = false;
        _pollTimer.Start();
    }

    private void OnPollTimerTick(object? sender, ElapsedEventArgs e)
    {
        try
        {
            UpdateDataOnPollTick();
        }

        finally
        {
            _pollTimer.Start();
        }
    }


    public void UpdateDataOnPollTick()
    {
        if (_tickCount++ >= 100)
        {
            _tickCount = 0;
            _targetSetPoint = _random.NextInt64(1, 10) * 1000;
        }

        var adjustment = AdjustRateOfChange();

        if (CurrentValue < _targetSetPoint)
            CurrentValue += Math.Round(_random.NextDouble() * adjustment, 2);
        else
            CurrentValue -= Math.Round(_random.NextDouble() * adjustment, 2);
    }

    private float AdjustRateOfChange()
    {
        if (Math.Abs(CurrentValue - _targetSetPoint) > 10000)
            return 5000;
        if (Math.Abs(CurrentValue - _targetSetPoint) > 5000)
            return 1000;
        if (Math.Abs(CurrentValue - _targetSetPoint) > 1000)
            return 500;
       
        return 100;
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
