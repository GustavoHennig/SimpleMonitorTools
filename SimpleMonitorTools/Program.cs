using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using System;
using System.Threading;

namespace SimpleMonitorTools
{
    class Program
    {
        private static Mutex _mutex = null;
        const string appName = "SimpleMonitorTools";

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            bool createdNew;
            _mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                // App is already running
                return;
            }

            var lifetime = new ClassicDesktopStyleApplicationLifetime();

            BuildAvaloniaApp()
                .SetupWithLifetime(lifetime);

            lifetime.ShutdownMode = Avalonia.Controls.ShutdownMode.OnExplicitShutdown;

            lifetime.Start(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
    }
}
