using Avalonia;
using HyperTracker.Recordings;
using HyperTracker.Threads;
using System;
using System.Threading;
using System.Timers;

namespace HyperTracker;

class Program
{
    public static System.Timers.Timer? _recorder = null;

    public static void Main(string[] args)
    {
        Global.LoadSettings();

        GlobalEvents.OnLoadProfile += Global.LoadProfile;
        GlobalEvents.OnSettingsChange += Global.SaveSettings;
        GlobalEvents.OnRecordStart += _startRecording;
        GlobalEvents.OnRecordEnd += _stopRecording;        
        GlobalEvents.OnLoadProfile += _loadProfile;
        GlobalEvents.OnCaptureFrame += CaptureThread.CaptureFrame;

        _recorder = new System.Timers.Timer(1000);
        _recorder.Elapsed += _captureFrame;



        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    private static void _captureFrame(object? sender, ElapsedEventArgs e)
    {
        GlobalEvents.InvokeCaptureFrame();
    }

    private static void _startRecording()
    {
        if(_recorder != null)
        {
            Global.RECORDING_FRAMES.Clear();
            _recorder!.Start();            
        }
    }

    private static void _stopRecording()
    {
        if(_recorder != null)
        {
            _recorder!.Stop();
        }
        Console.WriteLine($"Frames: {Global.RECORDING_FRAMES.Count}");
    }

    private static void _loadProfile(int profileIndex)
    {
        GlobalEvents.InvokeOnRecordEnd();
        if(_recorder == null && Global.PROFILE_SETTINGS != null && Global.PROFILE_SETTINGS[profileIndex] != null)
        {       
            _recorder = new System.Timers.Timer(Global.PROFILE_SETTINGS[profileIndex].TargetCycleMs);
        }else if(Global.PROFILE_SETTINGS != null && Global.PROFILE_SETTINGS[profileIndex] != null)
        {
            _recorder!.Stop();
            _recorder!.Interval = Global.PROFILE_SETTINGS[profileIndex].TargetCycleMs;
        }
    }


    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<Windows.App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
