

using System.Diagnostics;

namespace TestLiveCharts.Utils;

public static class AsyncErrorHandler
{
    public static void HandleException(Exception exception)
    {
        Debug.WriteLine(exception);
    }
}


