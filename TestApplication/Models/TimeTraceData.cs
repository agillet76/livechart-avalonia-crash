using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
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
    [Reactive] public ObservableCollection<double> PMTsVolts { get; set; } 
    public List<double> PreFilterPMTsVolts { get; set; } 
    public List<double> AnalogOutputChannlesVolts { get; set; } 
    public List<bool> DigitalTTLOutputs { get; set; } 

    public TimeTraceData(int PmtsCount)
    {
        PMTsVolts = new ObservableCollection<double>();
        PMTsVolts.AddRange(Enumerable.Range(0, PmtsCount).Select(_ => 0.0).ToList());
        PreFilterPMTsVolts = Enumerable.Range(0, PmtsCount).Select(_ => 0.0).ToList();
        AnalogOutputChannlesVolts = new List<double>(){ 0, 0, 0, 0 };
        DigitalTTLOutputs= new List<bool> { true, false, false, false, false, false, false, false };
    }
}
