using System.Diagnostics;
using System.Reactive.Concurrency;
using Avalonia;
using Avalonia.ReactiveUI;
using System.Text;
using Autofac;
using Avalonia.Controls;
using Avalonia.Threading;
using TestLiveCharts.DependencyInjection;
using ReactiveUI;
using Splat;
using Splat.Autofac;


namespace TestLiveCharts;

internal class Program
{
    private const int TimeoutSeconds = 3;

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        var mutex = new Mutex(false, typeof(Program).FullName);
        try
        {
            if (!mutex.WaitOne(TimeSpan.FromSeconds(TimeoutSeconds), true))
            {
                return;
            }
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args, ShutdownMode.OnMainWindowClose);
        }
        finally
        {
            mutex.ReleaseMutex();
        }
    }

    private static void ConfigureAutofacDependencyInjection()
    {
        var autofacBuilder = new ContainerBuilder();

        try
        {

            ViewModelsBootstrapper.RegisterViewModels(autofacBuilder);

            // Use Autofac for ReactiveUI dependency resolution.
            // After we call the method below, Locator.Current and
            // Locator.CurrentMutable start using Autofac locator.
            AutofacDependencyResolver resolver = new AutofacDependencyResolver(autofacBuilder);
            Locator.SetLocator(resolver);
            // Register the resolver in Autofac so it can be later resolved
            autofacBuilder.RegisterInstance(resolver);

            // These .InitializeX() methods will add ReactiveUI platform 
            // registrations to your container. They MUST be present if
            // you *override* the default Locator.
            Locator.CurrentMutable.InitializeSplat();
            Locator.CurrentMutable.InitializeReactiveUI();


            PlatformRegistrationManager.SetRegistrationNamespaces(RegistrationNamespace.Avalonia);
            RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;
            Locator.CurrentMutable.RegisterConstant(new AvaloniaActivationForViewFetcher(), typeof(IActivationForViewFetcher));
            Locator.CurrentMutable.RegisterConstant(new AutoDataTemplateBindingHook(), typeof(IPropertyBindingHook));

            var container = autofacBuilder.Build();
            resolver.SetLifetimeScope(container);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
            throw;
        }
    }

    private static void SubscribeToDomainUnhandledEvents() =>
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            LogException((Exception)args.ExceptionObject);
        };

    private static void SubscribeToTaskSchedulerOnUnobserved() =>
        TaskScheduler.UnobservedTaskException += (sender, args) =>
        {
            LogException(args.Exception);
        };


    public static AppBuilder BuildAvaloniaApp()
    {
        RxApp.DefaultExceptionHandler = new GlobalObserverErrorHandler();
        SubscribeToDomainUnhandledEvents();
        SubscribeToTaskSchedulerOnUnobserved();

        ConfigureAutofacDependencyInjection();

      return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .With(new SkiaOptions { MaxGpuResourceSizeBytes = 8096000 })
            .With(new Win32PlatformOptions { AllowEglInitialization = true })
            .LogToTrace();
    }

    private static void LogException(Exception ex)
    {
        Console.Error.WriteLine(ex.ToString());
        Console.Error.WriteLine();


        var logText = new StringBuilder();
        logText.AppendLine("!!! Unhandled application error !!!");
        
        if (ex != null)
        {
            if (ex is AggregateException)
            {
                foreach (var e in ((AggregateException)ex).Flatten().InnerExceptions)
                {
                    logText.AppendLine(ex.Message);
                    logText.AppendLine(ex.StackTrace);
                }
            }
            else
            {
                logText.AppendLine(ex.Message);
                logText.AppendLine(ex.StackTrace);
            }
        }
        Console.Error.WriteLine(logText.ToString());
    }

    internal class GlobalObserverErrorHandler : IObserver<Exception>
    {
        public void OnNext(Exception value)
        {
            if (Debugger.IsAttached) Debugger.Break();

            LogException(value);
    
            RxApp.MainThreadScheduler.Schedule(() => { throw value; });
        }

        public void OnError(Exception error)
        {
            if (Debugger.IsAttached) Debugger.Break();

            LogException(error);

            RxApp.MainThreadScheduler.Schedule(() => { throw error; });
        }

        public void OnCompleted()
        {
            if (Debugger.IsAttached) Debugger.Break();
            RxApp.MainThreadScheduler.Schedule(() => { throw new NotImplementedException(); });
        }
    }
}

