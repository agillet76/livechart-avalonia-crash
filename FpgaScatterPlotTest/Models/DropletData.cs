using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace FpgaScatterPlotTest.Models;

//  The channel arrangement is:
//  0 - Timestamp(ms)
//  1-4 - PMTs(Volts)
//  5-8 - PMTs(Pre-Median Filter) (Volts)
//  9-12 - Analog Output Channels(Volts)
//  13-20 - Digital TTL Outputs(as 0 or 1)
public class DropletData:ReactiveObject
{
    [Reactive][Name("Entry Time [ms]")] public double EntryTimeInMs { get; set; }
    [Reactive][Name("Time inside [ms]")] public double TimeInsideInMs { get; set; }
    [Reactive][Name("Time outside [ms]")] public double TimeOutsideInMs { get; set; }
    [Reactive][Name("Max 1 [volts]")] public double Max1InVolts { get; set; }
    [Reactive][Name("Max 2 [volts]")] public double Max2InVolts { get; set; }
    [Reactive][Name("Avg 1 [volts]")] public double Avg1InVolts { get; set; }
    [Reactive][Name("Avg 2 [volts]")] public double Avg2InVolts { get; set; }
    [Reactive][Name("Max Unsmoothed 1 [volts]")] public double MaxUnfilter1InVolts { get; set; }
    [Reactive][Name("Max Unsmoothed 2 [volts]")] public double MaxUnfilter2InVolts { get; set; }
    [Reactive][Name("Avg Unsmoothed 1 [volts]")] public double AvgUnfilter1InVolts { get; set; }
    [Reactive][Name("Avg Unsmoothed 2 [volts]")] public double AvgUnfilter2InVolts { get; set; }
    [Reactive][Name("Ratio Max")] public double RatioMax { get; set; }
    [Reactive][Name("Ratio Max Unsmoothed")] public double RatioMaxUnfilter { get; set; }
    [Reactive][Name("Ratio Average")] public double RatioAvg { get; set; }
    [Reactive][Name("Ratio Average Unsmoothed")] public double RatioAvgUnfilter { get; set; }
    [Reactive][Name("Collection Position")] public double Position { get; set; }
    [Reactive]
    [TypeConverter(typeof(CustomBooleanConverter))] 
    public bool Hit { get; set; }
    [Reactive][TypeConverter(typeof(CustomBooleanConverter))] public bool Sorted { get; set; }
}


public class CustomBooleanConverter : DefaultTypeConverter
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        if (bool.TryParse(text, out var result))
        {
            return result;
        }
        else
        {
            return false;
        }
    }
}
