using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using HyperTracker.Windows;
using System;
using System.Threading;

namespace HyperTracker;

class Program
{
    static Thread? _processThread = null;

    public static void Main(string[] args)
    {
        Global.LoadSettings();
        Global.LoadProfile(0);

        _processThread = new Thread(Threads.CaptureThread.ThreadLoop);
        _processThread.Start();

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }


    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<Windows.App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
