using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using HyperTracker.Datatypes;
using SixLabors.ImageSharp.Formats.Png;


namespace HyperTracker.Windows;

public partial class MainWindow : Window
{
    /// <summary>
    /// UI processing thread.
    /// </summary>
    private Thread _updateUIThread;
    /// <summary>
    /// Thread to capture frames.
    /// </summary>
    private Thread _captureThread;
    /// <summary>
    /// Initialize main Avalonia window.
    /// </summary>
    public MainWindow()
    {
        Global.LoadSettings();
        Global.LoadProfile(0);
        InitializeComponent();
        this.WindowState = WindowState.FullScreen;
        _updateUIThread = new Thread(UpdateUI);
        _updateUIThread.Start();
        _captureThread = new Thread(Threads.CaptureThread.ThreadLoop);
        _captureThread.Start();
    }

    public void ProfileComboBox_SelectionChanged(object sender, RoutedEventArgs args)
    {
        var box = sender as ComboBox;
        if(box != null)
        {
            
            int selected = box.SelectedIndex;
            Console.WriteLine($"Changine Profiles: {selected}");
            try
            {
                Global.LoadProfile(selected);
            }
            catch
            {
                
            }
            
        }
    }

#region LIVE CONTROLS
    public void LiveStartButton_Click(object sender, RoutedEventArgs args)
    {       
        Global.RECORDING_FRAMES.Clear();
        Global.IS_RECORDING = true;
        LIVE_STATUS_TEXT.Text = "STATUS: RECORDING";
    }
    public void LiveStopButton_Click(object sender, RoutedEventArgs args)
    {       
        Global.IS_RECORDING = false;
        LIVE_STATUS_TEXT.Text = "STATUS: IDLE";
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

    public void AnalysisFrameSlider_Changed(object sender, RoutedEventArgs args)
    {       
        if(Global.RECORDING_FRAMES.Count == 0)
        {
            return;
        }
        var slider = sender as Avalonia.Controls.Slider;
        if(slider != null)
        {
            int index = (int)slider.Value;
            Global.CURRENT_FRAME_INDEX = index;
        }
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
        Global.REBUILD_UI = true;
    }

    public void UpdateUI()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            List<string> items = new List<string>();
            foreach(var profile in Global.PROFILE_SETTINGS)
            {
                items.Add(profile.ProfileName);
            }
            PROFILE_COMBOBOX.ItemsSource = items;
            PROFILE_COMBOBOX.SelectedIndex = 0;
        });
        while(true)
        {
            if(Global.APPLICATION_INPUTS.Count > 0)
            {
                if(Global.REBUILD_UI)
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
                    
                    Global.REBUILD_UI = false;
                }
                foreach(iInput i in Global.APPLICATION_INPUTS)
                {
                    if(Global.CURRENT_TAB == 0)
                    {
                        i.UpdateLiveControl(LIVE_CANVAS);
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            if(Global.IS_RECORDING)
                            {
                                
                                LIVE_BORDER.BorderBrush = CreateGradient(new Color(255, 0, 150, 0), new Color(255, 0, 0, 0));
                                LIVE_BORDER.Background = CreateGradient(new Color(255, 0, 150, 0), new Color(255, 0, 0, 0));
                            }else
                            {
                                LIVE_BORDER.BorderBrush = CreateGradient(new Color(255, 150, 0, 0), new Color(255, 0, 0, 0));
                                LIVE_BORDER.Background = CreateGradient(new Color(255, 150, 0, 0), new Color(255, 0, 0, 0));
                            }
                            
                        });
                    }
                    if(Global.CURRENT_TAB == 1)
                    {
                        i.UpdateAnalysisControl(ANALYSIS_CANVAS);
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            ANALYSIS_FRAME_SLIDER.Maximum = Global.RECORDING_FRAMES.Count - 1;
                        });
                        
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

    public LinearGradientBrush CreateGradient(Color startColor, Color endColor)
    {
        LinearGradientBrush gradientBrush = new LinearGradientBrush();
        gradientBrush.StartPoint = new Avalonia.RelativePoint(0,0, Avalonia.RelativeUnit.Relative);
        gradientBrush.EndPoint = new Avalonia.RelativePoint(0,1, Avalonia.RelativeUnit.Relative);

        gradientBrush.GradientStops.Add(new GradientStop(startColor, 0.0));
        gradientBrush.GradientStops.Add(new GradientStop(endColor, 1));

        return gradientBrush;
    }
}