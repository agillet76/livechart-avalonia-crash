using FpgaScatterPlotTest.Models;

namespace FpgaScatterPlotTest.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";
        public Polygon PolygonA { get; set; } = new Polygon(){ Name = "TestAAAA"};
    }
}