# livechart-avalonia-crash

This is a test application to investigate a sporadic crash ( seconds to hours) happening when using LiveChart and AvaloniaUI.
The current code use LiveChart 2.0.0.Beta.300 and AvaloniaUI 0.10.16

The application is displaying a set of Livechart with data refresh at 20Hz..It is to mimic displaying sensors data (temperature/pressure value) from devices.

<img width="900" alt="image" src="https://user-images.githubusercontent.com/92173569/178043924-9f40c682-3d58-46b3-955a-0752a6008290.png">



The stacktrace of the crash is as follow:

System.ArgumentException: An item with the same key has already been added. Key: [41, System.Collections.Generic.List`1[Avalonia.VisualTree.IVisual]]
   at System.Collections.Generic.TreeSet`1.AddIfNotPresent(T item)
   at System.Collections.Generic.SortedDictionary`2.Add(TKey key, TValue value)
   at Avalonia.Rendering.DirtyVisuals.Add(IVisual visual) in /_/src/Avalonia.Visuals/Rendering/DirtyVisuals.cs:line 56
   at LiveChartsCore.CartesianChart`1.Measure()
   at LiveChartsCore.Chart`1.<UpdateThrottlerUnlocked>b__144_1()
   at Avalonia.Threading.JobRunner.RunJobs(Nullable`1 priority) in /_/src/Avalonia.Base/Threading/JobRunner.cs:line 37
   at Avalonia.Win32.Win32Platform.WndProc(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam) in /_/src/Windows/Avalonia.Win32/Win32Platform.cs:line 263
   at Avalonia.Win32.Interop.UnmanagedMethods.DispatchMessage(MSG& lpmsg)
   at Avalonia.Win32.Win32Platform.RunLoop(CancellationToken cancellationToken) in /_/src/Windows/Avalonia.Win32/Win32Platform.cs:line 210
   at Avalonia.Threading.Dispatcher.MainLoop(CancellationToken cancellationToken) in /_/src/Avalonia.Base/Threading/Dispatcher.cs:line 65
   at Avalonia.Controls.ApplicationLifetimes.ClassicDesktopStyleApplicationLifetime.Start(String[] args) in /_/src/Avalonia.Controls/ApplicationLifetimes/ClassicDesktopStyleApplicationLifetime.cs:line 96
   at TestLiveCharts.Program.Main(String[] args) in C:\Users\Alexandre\source\TestCode\LiveChartsTest\TestApplication\Program.cs:line 34

!!! Unhandled application error !!!
An item with the same key has already been added. Key: [41, System.Collections.Generic.List`1[Avalonia.VisualTree.IVisual]]
   at System.Collections.Generic.TreeSet`1.AddIfNotPresent(T item)
   at System.Collections.Generic.SortedDictionary`2.Add(TKey key, TValue value)
   at Avalonia.Rendering.DirtyVisuals.Add(IVisual visual) in /_/src/Avalonia.Visuals/Rendering/DirtyVisuals.cs:line 56
   at LiveChartsCore.CartesianChart`1.Measure()
   at LiveChartsCore.Chart`1.<UpdateThrottlerUnlocked>b__144_1()
   at Avalonia.Threading.JobRunner.RunJobs(Nullable`1 priority) in /_/src/Avalonia.Base/Threading/JobRunner.cs:line 37
   at Avalonia.Win32.Win32Platform.WndProc(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam) in /_/src/Windows/Avalonia.Win32/Win32Platform.cs:line 263
   at Avalonia.Win32.Interop.UnmanagedMethods.DispatchMessage(MSG& lpmsg)
   at Avalonia.Win32.Win32Platform.RunLoop(CancellationToken cancellationToken) in /_/src/Windows/Avalonia.Win32/Win32Platform.cs:line 210
   at Avalonia.Threading.Dispatcher.MainLoop(CancellationToken cancellationToken) in /_/src/Avalonia.Base/Threading/Dispatcher.cs:line 65
   at Avalonia.Controls.ApplicationLifetimes.ClassicDesktopStyleApplicationLifetime.Start(String[] args) in /_/src/Avalonia.Controls/ApplicationLifetimes/ClassicDesktopStyleApplicationLifetime.cs:line 96
   at TestLiveCharts.Program.Main(String[] args) in C:\Users\Alexandre\source\TestCode\LiveChartsTest\TestApplication\Program.cs:line 34
