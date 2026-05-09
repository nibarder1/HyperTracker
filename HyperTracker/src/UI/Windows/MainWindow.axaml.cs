using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using HyperTracker.Core;
using HyperTracker.CV;
using HyperTracker.UI.UIBuilders;

namespace HyperTracker.UI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        this.MinHeight = 720;
        this.MinWidth = 1280;
        this.SizeChanged += _resize;
        GlobalEvents.OnRebuildUI += _init;
        InitializeComponent();
        GlobalEvents.Ready();
        GlobalEvents.RebuildUI();
    }

    #region UI



    private void _init()
    {
        Dispatcher.UIThread.Invoke(() =>
            {
                MAIN_CANVAS.Width = this.ClientSize.Width;
                MAIN_CANVAS.Height = this.ClientSize.Height;

                TabControl MainTabController = TabControlBuilder.CreateTabControl((int)MAIN_CANVAS.Width, (int)MAIN_CANVAS.Height, 0, 0, "MAIN_CONTROLLER");
                MainTabController.SelectionChanged += _tabChanged;
                _buildSetupTab(MainTabController);
                _buildRecordTab(MainTabController);
                _buildAnalysisTab(MainTabController);

                MAIN_CANVAS.Children.Clear();
                MAIN_CANVAS.Children.Add(MainTabController);
            }
        );



    }

    private void _tabChanged(object? sender, SelectionChangedEventArgs e)
    {
        TabControl? control = (TabControl?)sender;
        if (control != null)
        {
            Global.CurrentTab = control.SelectedIndex;
            
        }
        
    }

    #endregion

    #region TABS

    private void _buildSetupTab(TabControl control)
    {
        Canvas tab = CanvasBuilder.CreateCanvas((int)control.Width, (int)(control.Height - Global.Theme.TabControlHeaderHeight), 0, 0, "SETUP_TAB");
        tab.Background = Global.Theme.TabControlSelectedBrush;
        Border leftPanel = CanvasBuilder.CreateCanvasWithBorder(300, (int)(tab.Height - Global.Theme.TabControlHeaderHeight), 0, 10, $"{tab.Name}_LEFT_PANEL");
        Button openProgram = ButtonBuilder.CreateButton((int)leftPanel.Width - 20, 30, 10, 10, $"{tab.Name}_OPEN_PROGRAM", "OPEN PROGRAM", _openProgramClick);
        Button saveProgram = ButtonBuilder.CreateButton((int)leftPanel.Width - 20, 30, 10, 50, $"{tab.Name}_SAVE_PROGRAM", "SAVE PROGRAM", _saveProgramClick);
        Button addCamera = ButtonBuilder.CreateButton((int)leftPanel.Width - 20, 30, 10, 90, $"{tab.Name}_ADD_CAMERA", "ADD CAMERA", _addCameraClick);

        Border programNameLabel = TextBlockBuilder.CreateTextBlockWithBox((int)leftPanel.Width / 2 - 20, 30, 10, 150, $"{tab.Name}_PROGRAM_LABEL", "NAME");
        TextBox programName = TextBoxBuilder.CreateTextBox((int)leftPanel.Width / 2 - 10, 30, (int)leftPanel.Width / 2, 150, $"{tab.Name}_PROGRAM_NAME", false);
        programName.Text = Global.Config!.ProgramName;
        programName.TextChanged += programNameChanged;

        Border modeLabel = TextBlockBuilder.CreateTextBlockWithBox((int)leftPanel.Width / 2 - 20, 30, 10, 190, $"{tab.Name}_PROGRAM_MODE_LABEL", "MODE");
        ComboBox modeSelector = ComboBoxBuilder.CreateComboBox((int)leftPanel.Width / 2 - 20, 30, (int)leftPanel.Width / 2, 190, $"{tab.Name}_PROGRAM_MODE_SELECTOR");
        modeSelector.ItemsSource = Enum.GetValues(typeof(ProgramType)).Cast<ProgramType>().ToList();
        modeSelector.SelectedValue = Global.Config.ProgramMode;
        modeSelector.SelectionChanged += _modeSelectionChanged;

        Border recordTimeLabel = TextBlockBuilder.CreateTextBlockWithBox((int)leftPanel.Width / 2 - 20, 30, 10, 230, $"{tab.Name}_RECORD_TIME_LABEL", "RECORD TIME");
        Border cycleTimeLabel = TextBlockBuilder.CreateTextBlockWithBox((int)leftPanel.Width / 2 - 20, 30, 10, 270, $"{tab.Name}_RECORD_CYCLE_TIME_LABEL", "CYCLE MS");
        NumericUpDown recordTime = NumberInputBuilder.CreateNumberInput((int)leftPanel.Width / 2 - 10, 30, (int)leftPanel.Width / 2, 230, $"{tab.Name}_RECORD_TIME_VALUE");
        recordTime.Minimum = 5;
        recordTime.Maximum = 10;
        recordTime.Value = Global.Config.RecordingTime;
        recordTime.ValueChanged += recordTimeChanged;

        NumericUpDown cycleTime = NumberInputBuilder.CreateNumberInput((int)leftPanel.Width / 2 - 10, 30, (int)leftPanel.Width / 2, 270, $"{tab.Name}_RECORD_CYCLE_TIME_VALUE");
        cycleTime.Minimum = 5;
        cycleTime.Maximum = 1000;
        cycleTime.Value = Global.Config.RecordingCycleMs;
        cycleTime.ValueChanged += cycleTimeChanged;


        CanvasBuilder.AddElement(leftPanel, openProgram);
        CanvasBuilder.AddElement(leftPanel, saveProgram);
        CanvasBuilder.AddElement(leftPanel, addCamera);
        CanvasBuilder.AddElement(leftPanel, programNameLabel);
        CanvasBuilder.AddElement(leftPanel, programName);
        CanvasBuilder.AddElement(leftPanel, modeLabel);
        CanvasBuilder.AddElement(leftPanel, modeSelector);
        CanvasBuilder.AddElement(leftPanel, recordTimeLabel);
        CanvasBuilder.AddElement(leftPanel, cycleTimeLabel);
        CanvasBuilder.AddElement(leftPanel, recordTime);
        CanvasBuilder.AddElement(leftPanel, cycleTime);


        tab.Children.Add(leftPanel);
        Border scrollPanel = ScrollPanelBuilder.CreateScrollPanelWithBorder((int)(tab.Width - 20 - leftPanel.Width), (int)(tab.Height - Global.Theme.TabControlHeaderHeight), (int)(20 + leftPanel.Width), 10, $"{tab.Name}_CAMERA_SCROLL_PANEL");


        Canvas cameraCanvas = CanvasBuilder.CreateCanvas((int)(tab.Width - 20 - leftPanel.Width), (int)(tab.Height - Global.Theme.TabControlHeaderHeight), 0, 0, $"{tab.Name}_CAMERA_PANEL");

        int camBox = 400;
        int maxCamsPerRow = (int)cameraCanvas.Width / camBox;
        int camCanvasHeight = _rows(Global.Cameras.Count, maxCamsPerRow) * camBox;
        if (camCanvasHeight > cameraCanvas.Height)
        {
            cameraCanvas.Height = camCanvasHeight;
        }

        for (int i = 0; i < Global.Cameras.Count; i++)
        {
            int x = (i % maxCamsPerRow) * camBox;
            int y = i / maxCamsPerRow * camBox;
            cameraCanvas.Children.Add(CameraBuilder.CreateCameraSetupWithBorder(camBox, camBox, x, y, $"{tab.Name}_CAMERA_{i}", Global.Cameras[i]));

        }

        ScrollPanelBuilder.AddElement(scrollPanel, cameraCanvas);
        tab.Children.Add(scrollPanel);


        TabControlBuilder.AddTab(control, tab, "SETUP");
    }



    private int _rows(int count, int maxRowCount)
    {
        if (count % maxRowCount > 0)
        {
            return count / maxRowCount + 1;
        }
        return count / maxRowCount;
    }



    private void _buildRecordTab(TabControl control)
    {
        Canvas tab = CanvasBuilder.CreateCanvas((int)control.Width, (int)(control.Height - Global.Theme.TabControlHeaderHeight), 0, 0, "RECORD_TAB");
        tab.Background = Global.Theme.TabControlSelectedBrush;

        Border leftPanel = CanvasBuilder.CreateCanvasWithBorder(300, (int)(tab.Height - Global.Theme.TabControlHeaderHeight), 0, 10, $"{tab.Name}_LEFT_PANEL");
        Border recordingStatus = TextBlockBuilder.CreateTextBlockWithBox((int)leftPanel.Width - 20, 30, 10, 10, $"{tab.Name}_RECORDING_STATUS", "IDLE");
        recordingStatus.Background = Global.Theme.IdleBrush;
        Button openProgram = ButtonBuilder.CreateButton((int)leftPanel.Width - 20, 30, 10, 50, $"{tab.Name}_START_RECORDING", "ARM RECORDING", _startRecordingClick);
        Button saveProgram = ButtonBuilder.CreateButton((int)leftPanel.Width - 20, 30, 10, 90, $"{tab.Name}_STOP_RECORDING", "RECORD", _stopRecordingClick);
        Button addCamera = ButtonBuilder.CreateButton((int)leftPanel.Width - 20, 30, 10, 130, $"{tab.Name}_ADD_CAMERA", "RESET", _cancelRecordingClick);

        CanvasBuilder.AddElement(leftPanel, recordingStatus);
        CanvasBuilder.AddElement(leftPanel, openProgram);
        CanvasBuilder.AddElement(leftPanel, saveProgram);
        CanvasBuilder.AddElement(leftPanel, addCamera);


        tab.Children.Add(leftPanel);
        Border scrollPanel = ScrollPanelBuilder.CreateScrollPanelWithBorder((int)(tab.Width - 20 - leftPanel.Width), (int)(tab.Height - Global.Theme.TabControlHeaderHeight), (int)(20 + leftPanel.Width), 10, $"{tab.Name}_CAMERA_SCROLL_PANEL");


        Canvas cameraCanvas = CanvasBuilder.CreateCanvas((int)(tab.Width - 20 - leftPanel.Width), (int)(tab.Height - Global.Theme.TabControlHeaderHeight), 0, 0, $"{tab.Name}_CAMERA_PANEL");

        int camBox = 400;
        int maxCamsPerRow = (int)cameraCanvas.Width / camBox;
        int camCanvasHeight = _rows(Global.Cameras.Count, maxCamsPerRow) * camBox;
        if (camCanvasHeight > cameraCanvas.Height)
        {
            cameraCanvas.Height = camCanvasHeight;
        }

        for (int i = 0; i < Global.Cameras.Count; i++)
        {
            int x = (i % maxCamsPerRow) * camBox;
            int y = i / maxCamsPerRow * camBox;
            cameraCanvas.Children.Add(CameraBuilder.CreateCameraRecordingWithBorder(camBox, camBox - 100, x, y, $"{tab.Name}_CAMERA_{i}", Global.Cameras[i]));

        }

        ScrollPanelBuilder.AddElement(scrollPanel, cameraCanvas);
        tab.Children.Add(scrollPanel);


        TabControlBuilder.AddTab(control, tab, "RECORD");
    }

    private void _buildAnalysisTab(TabControl control)
    {
        Canvas tab = CanvasBuilder.CreateCanvas((int)control.Width, (int)(control.Height - Global.Theme.TabControlHeaderHeight), 0, 0, "ANALYSIS_TAB");
        tab.Background = Global.Theme.TabControlSelectedBrush;

        Border leftPanel = CanvasBuilder.CreateCanvasWithBorder(300, (int)(tab.Height - Global.Theme.TabControlHeaderHeight), 0, 10, $"{tab.Name}_LEFT_PANEL");

        //Add buttons for last 10 recordings
        Button loadLastButton = ButtonBuilder.CreateButton((int)leftPanel.Width - 20, 30, 10, 10, $"{tab.Name}_LOAD_LAST_BUTTON", "LOAD LAST RECORDING", AnalysisManager.LoadLast);

        CanvasBuilder.AddElement(leftPanel, loadLastButton);
        tab.Children.Add(leftPanel);

        Border controlPanel = CanvasBuilder.CreateCanvasWithBorder((int)(tab.Width - 20 - leftPanel.Width), 50, (int)(20 + leftPanel.Width), 10, $"{tab.Name}_CONTROL_PANEL");
        Button previousFrame = ButtonBuilder.CreateButton(30, 30, 10, 10, $"{tab.Name}_PREVIOUS_FRAME", "<", AnalysisManager.PreviousFrame);
        Slider frameSlider = SliderBuilder.CreateSlider((int)controlPanel.Width * 2 / 3, 40, 20 + (int)previousFrame.Width, 0, $"{tab.Name}_FRAME_SLIDER");
        frameSlider.ValueChanged += AnalysisManager.SliderChanged;
        AnalysisManager.AnalysisSliders.Add(frameSlider);
        Button nextFrame = ButtonBuilder.CreateButton(30, 30, 30 + (int)(previousFrame.Width + frameSlider.Width), 10, $"{tab.Name}_PREVIOUS_FRAME", ">", AnalysisManager.NextFrame);



        CanvasBuilder.AddElement(controlPanel, previousFrame);
        CanvasBuilder.AddElement(controlPanel, frameSlider);
        CanvasBuilder.AddElement(controlPanel, nextFrame);
        tab.Children.Add(controlPanel);
        Border scrollPanel = ScrollPanelBuilder.CreateScrollPanelWithBorder((int)(tab.Width - 20 - leftPanel.Width), (int)(tab.Height - Global.Theme.TabControlHeaderHeight - 70), (int)(20 + leftPanel.Width), 80, $"{tab.Name}_CAMERA_SCROLL_PANEL");


        Canvas cameraCanvas = CanvasBuilder.CreateCanvas((int)(tab.Width - 20 - leftPanel.Width), (int)(tab.Height - Global.Theme.TabControlHeaderHeight), 0, 0, $"{tab.Name}_CAMERA_PANEL");


        AnalysisManager.AnalysisCanvas = cameraCanvas;

        ScrollPanelBuilder.AddElement(scrollPanel, cameraCanvas);
        tab.Children.Add(scrollPanel);


        TabControlBuilder.AddTab(control, tab, "ANALYSIS");
    }

    #endregion

    #region EVENTS

    private void _cancelRecordingClick(object? sender, RoutedEventArgs e)
    {
        GlobalEvents.CancelRecording();
        TextBlock? recordingStatus = UIControl.FindAvaloniaControl<TextBlock>(MAIN_CANVAS, "RECORD_TAB_RECORDING_STATUS");
        if (recordingStatus != null)
        {
            recordingStatus.Text = "IDLE";
            recordingStatus.Foreground = Global.Theme.PrimaryForegroundBrush;
        }
        Border? recordingStatusBackground = UIControl.FindAvaloniaControl<Border>(MAIN_CANVAS, "RECORD_TAB_RECORDING_STATUS_BORDER");
        if (recordingStatusBackground != null)
        {
            recordingStatusBackground.Background = Global.Theme.IdleBrush;

        }
    }

    private void _stopRecordingClick(object? sender, RoutedEventArgs e)
    {

        TextBlock? recordingStatus = UIControl.FindAvaloniaControl<TextBlock>(MAIN_CANVAS, "RECORD_TAB_RECORDING_STATUS");
        if (recordingStatus != null)
        {
            recordingStatus.Text = $"RECORDING LAST {Global.Config!.RecordingTime} SECONDS";
            recordingStatus.Foreground = Global.Theme.PrimaryBackgroundBrush;
        }
        Border? recordingStatusBackground = UIControl.FindAvaloniaControl<Border>(MAIN_CANVAS, "RECORD_TAB_RECORDING_STATUS_BORDER");
        if (recordingStatusBackground != null)
        {
            recordingStatusBackground.Background = Global.Theme.SavingBrush;
        }
        GlobalEvents.StopRecording();
        if (recordingStatus != null)
        {
            recordingStatus.Text = $"IDLE";
            recordingStatus.Foreground = Global.Theme.PrimaryForegroundBrush;
        }
        if (recordingStatusBackground != null)
        {
            recordingStatusBackground.Background = Global.Theme.IdleBrush;
        }
    }

    private void _startRecordingClick(object? sender, RoutedEventArgs e)
    {
        GlobalEvents.StartRecording();
        TextBlock? recordingStatus = UIControl.FindAvaloniaControl<TextBlock>(MAIN_CANVAS, "RECORD_TAB_RECORDING_STATUS");
        if (recordingStatus != null)
        {
            recordingStatus.Text = "ARMED";
            recordingStatus.Foreground = Global.Theme.PrimaryForegroundBrush;
        }
        Border? recordingStatusBackground = UIControl.FindAvaloniaControl<Border>(MAIN_CANVAS, "RECORD_TAB_RECORDING_STATUS_BORDER");
        if (recordingStatusBackground != null)
        {
            recordingStatusBackground.Background = Global.Theme.RecordingBrush;
        }
    }

    private void _openProgramClick(object? sender, RoutedEventArgs e)
    {
        Task.Run(_openProgram);
    }

    private void _saveProgramClick(object? sender, RoutedEventArgs e)
    {
        Task.Run(_saveProgram);
    }

    private async Task _openProgram()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var jsonFileType = new FilePickerFileType("Program")
        {
            Patterns = new[] { "*.hyprog" }
        };
        var files = await topLevel!.StorageProvider.OpenFilePickerAsync(new Avalonia.Platform.Storage.FilePickerOpenOptions
        {
            Title = "Open Program",
            AllowMultiple = false,
            FileTypeFilter = new List<FilePickerFileType>
            {
                jsonFileType
            }
        });
        if (files.Count > 0)
        {
            Global.Config = Config.LoadConfig(files[0].Path.AbsolutePath);
            try
            {
                while (Global.Cameras.Count > 0)
                {
                    Global.Cameras[0].RemoveCamera();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Global.Cameras.Clear();
            Console.WriteLine("Starting new program.");
            if (Global.Config != null)
            {
                Console.WriteLine($"Starting program {Global.Config.ProgramName} with [{Global.Config.Cameras.Count}] cameras.");
                foreach (CameraProperties props in Global.Config.Cameras)
                {
                    Console.WriteLine(props.ToString());
                    Camera camera = new Camera();
                    camera.SetProperties(props);
                    camera.StartThreaded();
                    Global.Cameras.Add(camera);
                }
                GlobalEvents.RebuildUI();
            }

        }
    }

    private async Task _saveProgram()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var jsonFileType = new FilePickerFileType("Program")
        {
            Patterns = new[] { "*.hyprog" }
        };
        var files = await topLevel!.StorageProvider.SaveFilePickerAsync(new Avalonia.Platform.Storage.FilePickerSaveOptions
        {
            Title = "Save Program",
            FileTypeChoices = new List<FilePickerFileType>
            {
                jsonFileType
            }
        });
        if (files != null && files.Path.AbsolutePath.Length > 0)
        {
            if (Global.Config != null)
            {
                Global.Config.Cameras = new List<CameraProperties>();
                foreach (Camera c in Global.Cameras)
                {
                    if (c.Properties != null)
                    {
                        Global.Config.Cameras.Add(c.Properties);
                    }
                }
                Config.SaveConfig(Global.Config, files.Path.AbsolutePath);
            }
        }
    }

    private void _resize(object? sender, SizeChangedEventArgs e)
    {
        GlobalEvents.RebuildUI();
    }

    private void _addCameraClick(object? sender, RoutedEventArgs e)
    {
        Global.Cameras.Add(new Camera());
        GlobalEvents.RebuildUI();
    }

    private void programNameChanged(object? sender, TextChangedEventArgs e)
    {
        TextBox? text = (TextBox?)sender;
        if (text != null && Global.Config != null && text.Text != null)
        {
            Global.Config.ProgramName = text.Text;
        }
    }

    private void recordTimeChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        NumericUpDown? number = (NumericUpDown?)sender;
        if (number != null && number.Value != null && Global.Config != null)
        {
            Global.Config.RecordingTime = (int)number.Value;
        }

    }

    private void cycleTimeChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        NumericUpDown? number = (NumericUpDown?)sender;
        if (number != null && number.Value != null && Global.Config != null)
        {
            Global.Config.RecordingCycleMs = (int)number.Value;
            GlobalEvents.UpdateRecordingCycle((int)number.Value);
        }
    }

    private void _modeSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        ComboBox? box = (ComboBox?)sender;
        if (box != null)
        {
            if (box.SelectedValue != null && !String.IsNullOrWhiteSpace(box.SelectedValue.ToString()) && Global.Config != null)
            {
                Global.Config.ProgramMode = Enum.Parse<ProgramType>(box.SelectedValue.ToString()!);
            }
        }
    }

    #endregion
}