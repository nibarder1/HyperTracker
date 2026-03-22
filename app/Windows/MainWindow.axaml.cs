using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using HyperTracker.Datatypes;
using HyperTracker.Recordings;
using HyperTracker.UI;


namespace HyperTracker.Windows;

public partial class MainWindow : Window
{

    private System.Timers.Timer _uiTimer;

    /// <summary>
    /// Initialize main Avalonia window.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        this.WindowState = WindowState.FullScreen;  
        this._uiTimer = new System.Timers.Timer(100);
        this._uiTimer.Elapsed += _uiUpdate;
        this._uiTimer.AutoReset = true;   

        Task.Run(() =>
        {
            _initialize();
        });
    }

    private void _uiUpdate(object? sender, ElapsedEventArgs e)
    {
        GlobalEvents.InvokeOnLiveUIUpdate();
    }

    /// <summary>
    /// Initialize application.
    /// </summary>
    private Task _initialize()
    {
        Task.Delay(1000);
        GlobalEvents.OnRecordStart += _startRecording;
        GlobalEvents.OnRecordEnd += _preStopRecording;
        GlobalEvents.OnLiveUIUpdate += _updateClock;
        GlobalEvents.OnRebuildUI += _rebuildUI;
        GlobalEvents.OnFrameChange += _updateSlider;

        this._uiTimer.Enabled = true;
        GlobalEvents.InvokeLoadProfile(Global.LOADED_PROFILE, true);

        return Task.CompletedTask;
    }

    #region UI

    

    #endregion

    #region UI ELEMENT EVENT HANDLERS

    private void _updateSlider(int index)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            ANALYSIS_FRAME_SLIDER.Maximum = Global.RECORDING_FRAMES.Count;
            ANALYSIS_FRAME_SLIDER.Value = index;
        });
    }

    private void _startRecording_Click(object sender, RoutedEventArgs args)
    {
        GlobalEvents.InvokeOnRecordStart();
    }

    private void _stopRecording_Click(object sender, RoutedEventArgs args)
    {       
        GlobalEvents.InvokeOnRecordEnd();        
    }

    private void _profileChange_Select(object sender, RoutedEventArgs args)
    {
        var box = sender as ComboBox;
        if(box != null)
        {
            int selected = box.SelectedIndex;
            GlobalEvents.InvokeLoadProfile(selected);
        }                    
    }

    private void _loadRecording_Click(object sender, RoutedEventArgs args)
    {       
        GlobalEvents.InvokeFrameChange(0);
        _buildAnalysis();
    }

    private void _nextFrame_Click(object sender, RoutedEventArgs args)
    {       
        GlobalEvents.InvokeNextFrame();
    }

    private void _previousFrame_Click(object sender, RoutedEventArgs args)
    {       
        GlobalEvents.InvokePreviousFrame();
    }

    private void _openRecording_Click(object sender, RoutedEventArgs args)
    {       
        _openRecordingPicker();
    }

    private async Task _openRecordingPicker()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var jsonFileType = new FilePickerFileType("Recording JSON")
        {
            Patterns = new[] {"*.json"}
        };
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new Avalonia.Platform.Storage.FilePickerOpenOptions
        {
            Title = "Open Recording",
            AllowMultiple = false,
            FileTypeFilter = new List<FilePickerFileType>
            {
                jsonFileType
            }
        });
        if(files.Count > 0)
        {
            Console.WriteLine($"Opening recording: {files[0].Path}");
            LoadRecording.OpenRecording(files[0].Path.AbsolutePath);
            _buildAnalysis();
        }
        
    }

    private void _frameSlider_Changed(object sender, RoutedEventArgs args)
    {       
        if(Global.RECORDING_FRAMES.Count == 0)
        {
            return;
        }
        var slider = sender as Avalonia.Controls.Slider;
        if(slider != null)
        {
            int index = (int)slider.Value;
            GlobalEvents.InvokeFrameChange(index);
        }
    }

    /// <summary>
    /// Quit application button handler.
    /// </summary>
    /// <param name="sender">Quit application button.</param>
    /// <param name="args">Arguments.</param>
    private void _exitApplication_Click(object sender, RoutedEventArgs args)
    {       
        this._uiTimer.Stop();
        Handlers.ApplicationHandler.QuitApplication();
    }

    /// <summary>
    /// Tab control tab changed event handler.
    /// </summary>
    /// <param name="sender">Tab control.</param>
    /// <param name="e">Arguments.</param>
    public void _applicationTab_Select(object sender, SelectionChangedEventArgs e)
    {
        Handlers.TabHandler.ChangeTab(sender, e);        
    }

    #endregion

    #region UI EVENTS

    private void _startRecording()
    {
        LIVE_STATUS_TEXT.Text = "STATUS: RECORDING";
        LIVE_BORDER.BorderBrush = Stylings.CreateGradient(Stylings.GREEN, Stylings.BLACK);
        LIVE_BORDER.Background = Stylings.CreateGradient(Stylings.GREEN, Stylings.BLACK);
    }

    private void _stopRecording()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            LIVE_STATUS_TEXT.Text = "STATUS: IDLE";
            LIVE_BORDER.BorderBrush = Stylings.CreateGradient(Stylings.RED, Stylings.BLACK);
            LIVE_BORDER.Background = Stylings.CreateGradient(Stylings.RED, Stylings.BLACK);
        });
        
    }

    private void _preStopRecording()
    {
        LIVE_STATUS_TEXT.Text = "STATUS: SAVING RECORDING";
        LIVE_BORDER.BorderBrush = Stylings.CreateGradient(Stylings.YELLOW, Stylings.BLACK);
        LIVE_BORDER.Background = Stylings.CreateGradient(Stylings.YELLOW, Stylings.BLACK);
        Task.Run(async () =>
        {
            await SaveRecording.Save();
            _stopRecording();
        });
    }

    private void _updateClock()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            CURRENT_TIME.Text = DateTime.Now.ToString();
        });        
    }


    private void _rebuildUI()
    {
        List<string> items = new List<string>();
        foreach(var profile in Global.PROFILE_SETTINGS)
        {
            items.Add(profile.ProfileName);
        }
        Dispatcher.UIThread.Invoke(() =>
        {
            PROFILE_COMBOBOX.ItemsSource = items;
            PROFILE_COMBOBOX.SelectedIndex = Global.LOADED_PROFILE;
            _buildLive();
        });        


    }

    private void _buildLive()
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
        foreach(iModule i in Global.APPLICATION_INPUTS)
        {
            GlobalEvents.OnLiveUIUpdate -= i.UpdateLiveControl;
            int x = width * (currentControl % controlsPerRow);
            int y = height * (currentControl / controlsPerRow);
            var control = i.BuildLiveControl(width, height, x, y);
            GlobalEvents.OnLiveUIUpdate += i.UpdateLiveControl;
            Console.WriteLine(control.Name);
            LIVE_CANVAS.Children.Add(control);
            currentControl++;
        }
    }

    private void _buildAnalysis()
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
        foreach(iModule i in Global.APPLICATION_INPUTS)
        {
            GlobalEvents.OnFrameChange -= i.UpdateAnalysisControl;
            int x = width * (currentControl % controlsPerRow);
            int y = height * (currentControl / controlsPerRow);
            var control = i.BuildAnalysisControl(width, height, x, y);
            GlobalEvents.OnFrameChange += i.UpdateAnalysisControl;
            Console.WriteLine(control.Name);
            ANALYSIS_CANVAS.Children.Add(control);
            currentControl++;
        }
    }

    #endregion
    
}