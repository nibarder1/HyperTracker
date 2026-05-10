using Avalonia;
using HyperTracker.Core;
using HyperTracker.CV;
using OpenCvSharp;
using System;
using System.Threading;
using HyperTracker.UI;
using System.Threading.Tasks;
using Avalonia.Threading;
using System.IO;

namespace HyperTracker;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {        
        try
        {
            //build file system
            

            GlobalEvents.OnCaptureFrame += Frame.CaptureFrame;
            GlobalEvents.OnReady += _init;

            
            var builder = BuildAvaloniaApp(); 
            builder.StartWithClassicDesktopLifetime(args);
            Console.WriteLine("Exiting program.");  
            GlobalEvents.Exit();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unhandled exception: {ex}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
        //No code after here.
    }

    private static void _init()
    {       

        // try
        // {            
        //     Camera camera = new Camera();
        //     camera.SetProperties(new CameraProperties("test", "", 0, new CameraResolution(1280, 720, 120)));
        //     camera.StartThreaded();
        //     Global.Cameras.Add(camera);
        // }catch(Exception e)
        // {
        //     Console.WriteLine(e);
        // }
        

        GlobalEvents.Init();        
    }


    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
