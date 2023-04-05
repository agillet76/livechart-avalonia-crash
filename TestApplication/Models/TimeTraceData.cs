using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLiveCharts.Models;

//  The channel arrangement is:
//  0 - Timestamp(ms)
//  1-4 - PMTs(Volts)
//  5-8 - PMTs(Pre-Median Filter) (Volts)
//  9-12 - Analog Output Channels(Volts)
//  13-20 - Digital TTL Outputs(as 0 or 1)
public class TimeTraceData
{
    public double TimestampMs { get; set; }
    public List<double> PMTsVolts { get; set; } = new();
    public List<double> PreFilterPMTsVolts { get; set; } = new();
    public List<double> AnalogOutputChannlesVolts { get; set; } = new();
    public List<bool> DigitalTTLOutputs { get; set; } = new();
}
