using ReactiveUI;
using System.Collections.ObjectModel;
using DynamicData.Binding;
using ReactiveUI.Fody.Helpers;

namespace TestLiveCharts.Models;


public class Polygon : ReactiveObject
{
    [Reactive] public string Name { get; set; }
    
}

