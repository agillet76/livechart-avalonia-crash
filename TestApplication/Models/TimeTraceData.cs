using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace TestLiveCharts.Models;

//  The channel arrangement is:
//  0 - Timestamp(ms)
//  1-4 - PMTs(Volts)
//  5-8 - PMTs(Pre-Median Filter) (Volts)
//  9-12 - Analog Output Channels(Volts)
//  13-20 - Digital TTL Outputs(as 0 or 1)
public class TimeTraceData:ReactiveObject
{
    [Reactive] public double TimestampMs { get; set; }
    [Reactive] public ObservableCollection<double> PMTsVolts { get; set; } = new();
    public List<double> PreFilterPMTsVolts { get; set; } = new();
    public List<double> AnalogOutputChannlesVolts { get; set; } = new();
    public List<bool> DigitalTTLOutputs { get; set; } = new();
}
