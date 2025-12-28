using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using HyperTracker.Datatypes;
using SixLabors.ImageSharp.Formats.Png;


namespace HyperTracker.Windows;

public partial class MainWindow : Window
{
    private Thread _updateUIThread;
    private Thread _captureThread;
    private bool _rebuildWindow = true;
    /// <summary>
    /// Initialize main Avalonia window.
    /// </summary>
    public MainWindow()
    {
        InputParameters ip = new InputParameters();
        ip.AddParam("CameraIndex", 0);
        ip.AddParam("CameraWidth", 1280);
        ip.AddParam("CameraHeight", 720);
        ip.AddParam("CameraFPS", 120);
        ip.AddParam("IsStereo", false);
        ip.AddParam("IsEnabled", true);
        ip.AddParam("InputName", "test");
        Camera cam = new Camera(ip);
        cam.Initialize();
        if(cam.IsInitialized())
        {
            Console.WriteLine("Camera starting");
            Task.Run(cam.Start);
            Console.WriteLine("Camera started");
        }

        InputParameters ip2 = new InputParameters();
        ip2.AddParam("CameraIndex", 1);
        ip2.AddParam("CameraWidth", 1280);
        ip2.AddParam("CameraHeight", 720);
        ip2.AddParam("CameraFPS", 120);
        ip2.AddParam("IsStereo", false);
        ip2.AddParam("IsEnabled", true);
        ip2.AddParam("InputName", "test2");
        Camera cam2 = new Camera(ip2);
        cam2.Initialize();
        if(cam2.IsInitialized())
        {
            Console.WriteLine("Camera starting");
            Task.Run(cam2.Start);
            Console.WriteLine("Camera started");
        }



        Global.APPLICATION_INPUTS.Add(cam);
        Global.APPLICATION_INPUTS.Add(cam2);
        InitializeComponent();
        this.WindowState = WindowState.FullScreen;
        _updateUIThread = new Thread(UpdateUI);
        _updateUIThread.Start();
        _captureThread = new Thread(Threads.CaptureThread.ThreadLoop);
        _captureThread.Start();
    }

#region LIVE CONTROLS
    public void LiveStartButton_Click(object sender, RoutedEventArgs args)
    {       
        Global.RECORDING_FRAMES.Clear();
        Global.IS_RECORDING = true;
    }
    public void LiveStopButton_Click(object sender, RoutedEventArgs args)
    {       
        Global.IS_RECORDING = false;
    }
#endregion

#region ANALYSIS CONTROLS
    public void AnalysisLoadRecordingButton_Click(object sender, RoutedEventArgs args)
    {       
        Global.CURRENT_FRAME_INDEX = 0;
        foreach(var input in Global.APPLICATION_INPUTS)
        {
            input.UpdateAnalysisControl(ANALYSIS_CANVAS);
        }
    }

    public void AnalysisNextFrameButton_Click(object sender, RoutedEventArgs args)
    {       
        Global.CURRENT_FRAME_INDEX = (Global.CURRENT_FRAME_INDEX + 1) % Global.RECORDING_FRAMES.Count;
    }

    public void AnalysisPreviousFrameButton_Click(object sender, RoutedEventArgs args)
    {       
        Global.CURRENT_FRAME_INDEX = (Global.CURRENT_FRAME_INDEX - 1) % Global.RECORDING_FRAMES.Count;
    }
#endregion

#region CONFIGURATION CONTROLS
    public void ConfigAddCameraButton_Click(object sender, RoutedEventArgs args)
    {       
        
    }
    public void ConfigSaveButton_Click(object sender, RoutedEventArgs args)
    {       
        
    }
#endregion

    /// <summary>
    /// Quit application button handler.
    /// </summary>
    /// <param name="sender">Quit application button.</param>
    /// <param name="args">Arguments.</param>
    public void QuitApplication(object sender, RoutedEventArgs args)
    {       
        Handlers.ApplicationHandler.QuitApplication();
    }
    /// <summary>
    /// Tab control tab changed event handler.
    /// </summary>
    /// <param name="sender">Tab control.</param>
    /// <param name="e">Arguments.</param>
    public void TabSelected(object sender, SelectionChangedEventArgs e)
    {
        Handlers.TabHandler.ChangeTab(sender, e);
        this._rebuildWindow = true;
    }

    public void UpdateUI()
    {
        while(true)
        {
            if(Global.APPLICATION_INPUTS.Count > 0)
            {
                if(this._rebuildWindow)
                {
                    if(Global.CURRENT_TAB == 0)
                    {
                        BuildLiveCanvas();
                    }
                    if(Global.CURRENT_TAB == 1)
                    {
                        BuildAnalysisCanvas();
                    }
                    if(Global.CURRENT_TAB == 2)
                    {
                        BuildConfigurationCanvas();
                    }
                    this._rebuildWindow = false;
                }
                foreach(iInput i in Global.APPLICATION_INPUTS)
                {
                    if(Global.CURRENT_TAB == 0)
                    {
                        i.UpdateLiveControl(LIVE_CANVAS);
                    }
                    if(Global.CURRENT_TAB == 1)
                    {
                        i.UpdateAnalysisControl(ANALYSIS_CANVAS);
                    }
                    if(Global.CURRENT_TAB == 2)
                    {
                        i.UpdateConfigControl(CONFIG_CANVAS);
                    }
                    
                }
            }
            if(Global.CURRENT_TAB == 1)
            {
                Thread.Sleep(10);
            }else
            {
                Thread.Sleep(100);
            }
            
        }
    }

    private void BuildLiveCanvas()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            try
            {
                LIVE_CANVAS.Children.Clear();            
                int controlsPerRow = 2;
                int currentControl = 0;
                int width = (int)LIVE_CANVAS.Bounds.Width / controlsPerRow;
                int height = (int)LIVE_CANVAS.Bounds.Height / (Global.APPLICATION_INPUTS.Count / controlsPerRow + 1);
                if(height > width)
                {
                    height = width/2;
                }
                foreach(iInput i in Global.APPLICATION_INPUTS)
                {
                    int x = width * (currentControl % controlsPerRow);
                    int y = height * (currentControl / controlsPerRow);
                    var control = i.BuildLiveControl(width, height, x, y);
                    Console.WriteLine(control.Name);
                    LIVE_CANVAS.Children.Add(control);
                    currentControl++;
                }
            }catch{}
            
        });
        

    }

    private void BuildAnalysisCanvas()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            try
            {
                ANALYSIS_CANVAS.Children.Clear();            
                int controlsPerRow = 2;
                int currentControl = 0;
                int width = (int)ANALYSIS_CANVAS.Bounds.Width / controlsPerRow;
                int height = (int)ANALYSIS_CANVAS.Bounds.Height / (Global.APPLICATION_INPUTS.Count / controlsPerRow + 1);
                if(height > width)
                {
                    height = width/2;
                }
                foreach(iInput i in Global.APPLICATION_INPUTS)
                {
                    int x = width * (currentControl % controlsPerRow);
                    int y = height * (currentControl / controlsPerRow);
                    var control = i.BuildAnalysisControl(width, height, x, y);
                    Console.WriteLine(control.Name);
                    ANALYSIS_CANVAS.Children.Add(control);
                    currentControl++;
                }
            }catch{}
            
        });
    }
    private void BuildConfigurationCanvas()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            try
            {
                CONFIG_CANVAS.Children.Clear();            
                int controlsPerRow = 2;
                int currentControl = 0;
                int width = (int)CONFIG_CANVAS.Bounds.Width / controlsPerRow;
                int height = (int)CONFIG_CANVAS.Bounds.Height / (Global.APPLICATION_INPUTS.Count / controlsPerRow + 1);
                if(height > width)
                {
                    height = width/2;
                }
                foreach(iInput i in Global.APPLICATION_INPUTS)
                {
                    int x = width * (currentControl % controlsPerRow);
                    int y = height * (currentControl / controlsPerRow);
                    var control = i.BuildConfigControl(width, height, x, y);
                    Console.WriteLine(control.Name);
                    CONFIG_CANVAS.Children.Add(control);
                    currentControl++;
                }
            }catch{}
            
        });
    }
}