using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlashCap;
using FlashCap.Utilities;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using HyperTracker.Windows;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;
using HyperTracker.Windows.UIBuilders;
using Avalonia.Interactivity;
using SixLabors.ImageSharp.Processing;

namespace HyperTracker.Datatypes;

public class CameraModule : iModule
{
    #region Basic
    private bool _isInitialized = false;
    private bool _parametersValid = false;

    private CaptureDeviceDescriptor? _deviceDescriptor;
    private VideoCharacteristics? _deviceCharacteristics;

    private InputParameters? _parameters = null;
    
    private CaptureDevice? _device;
    private DateTime _lastFrameTime = DateTime.Now;
    public int FRAME_RATE = 0;
    private CameraPopup? _currentPopup = null;
    private SixLabors.ImageSharp.Image? _lastScan;
    

    public CameraModule(InputParameters parameters)
    {
        if(parameters.HasKey("CameraIndex") &&
            parameters.HasKey("CameraWidth") &&
            parameters.HasKey("CameraHeight") &&
            parameters.HasKey("CameraFPS") &&
            parameters.HasKey("IsStereo") &&
            parameters.HasKey("IsEnabled") &&
            parameters.HasKey("InputName"))
        {
            this._parametersValid= true;
        }
        else
        {
            Console.WriteLine("Invalid parameters.");
        }
        this._parameters = parameters;
    }

    public SixLabors.ImageSharp.Image? GetScan()
    {
        if(this._lastScan != null)
        {
            return this._lastScan;
        }
        return null;
    }

    /// <summary>
    /// Check if module is initialized.
    /// </summary>
    /// <returns>True if initialized.</returns>
    public bool IsInitialized()
    {
        return this._isInitialized;
    }
    /// <summary>
    /// Initialize input.
    /// </summary>
    /// <returns>True if successful.</returns>
    public bool Initialize()
    {
        try
        {
            this._initializeCamera().Wait();
            this._isInitialized = true;
            return true;
        }catch(Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return false;
    }

    private async Task _initializeCamera()
    {
        if(!this._parametersValid)
        {
            throw new Exception("Camera parameters not valid.");
        }
        var devices = new CaptureDevices();
        var indexer = devices.EnumerateDescriptors();
        int index = this._parameters!.GetParameter<int>("CameraIndex");
        var device = indexer.ElementAt(index);
        Console.WriteLine(device);
        var deviceCharacteristics = device.Characteristics[0];
        VideoCharacteristics settings = new VideoCharacteristics(PixelFormats.JPEG,
                                                                this._parameters.GetParameter<int>("CameraWidth"),
                                                                this._parameters.GetParameter<int>("CameraHeight"),
                                                                new Fraction(this._parameters.GetParameter<int>("CameraFPS") * 1000,1000));
        this._deviceDescriptor = device;
        this._deviceCharacteristics = settings;
    }
    /// <summary>
    /// Start scanning.
    /// </summary>
    public async Task Start()
    {
        if(this._parameters != null && !this._parameters.GetParameter<bool>("IsEnabled"))
        {
            return;
        }
        if(this._device != null && !this._device.IsRunning)
        {
            return;
        }
        if(this._isInitialized && this._deviceDescriptor != null && this._deviceCharacteristics != null)
        {
            try
            {
                Console.WriteLine(this._deviceDescriptor.Name);
                Console.WriteLine(this._deviceCharacteristics);
                Console.WriteLine("Opening feed");
                
                this._device = await this._deviceDescriptor.OpenAsync(
                    this._deviceCharacteristics,
                    _processFrame
                );
                if(this._device != null)
                {
                    Console.WriteLine("Starting feed");
                    await this._device.StartAsync();
                }
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }

    private async void _processFrame(PixelBufferScope buffer)
    {
        try
        {
            DateTime currentTime = DateTime.Now;
            ArraySegment<byte> bufferBytes = buffer.Buffer.ReferImage();
            var ms = new MemoryStream(bufferBytes.ToArray());
            this._lastScan = SixLabors.ImageSharp.Image.Load(ms);
            double timeMS = (currentTime - this._lastFrameTime).TotalMilliseconds;
            this._lastFrameTime = currentTime;
            this.FRAME_RATE = (int)(1000/timeMS);
            if(Global.CURRENT_TAB == 0 && this._currentPopup == null)
            {
                UpdateLiveControl();
            }
        }catch
        {
            Console.WriteLine($"Error processing frame for camera: {this._parameters!.GetParameter<string>("InputName")}");
        }
        
    }
    /// <summary>
    /// Stop scanning.
    /// </summary>
    public async Task Stop()
    {
        if(this._isInitialized && this._device != null)
        {
            if(this._device.IsRunning)
            {
                await this._device.StopAsync();
            }            
        }
    }
    /// <summary>
    /// Update input configuration parameters.
    /// </summary>
    /// <param name="parameters">Input parameters.</param>
    public void UpdateParameters(InputParameters parameters)
    {
        if(!parameters.HasKey("CameraIndex") ||
            !parameters.HasKey("CameraWidth") ||
            !parameters.HasKey("CameraHeight") ||
            !parameters.HasKey("CameraFPS") ||
            !parameters.HasKey("IsStereo") ||
            !parameters.HasKey("IsEnabled") ||
            !parameters.HasKey("InputName"))
        {
            return;
        }
        var previousParameters = this._parameters;
        this._parameters = parameters;
        try
        {
            if (!ReinitializeInput())
            {
                this._parameters = previousParameters;
                ReinitializeInput();
            }
        }
        catch
        {
            this._parameters = previousParameters;
        }
    }
    /// <summary>
    /// Get input configuration parameter.
    /// </summary>
    /// <returns>Parameters.</returns>
    public InputParameters? GetParams()
    {
        return this._parameters;
    }
    /// <summary>
    /// Reinitialize input.
    /// </summary>
    /// <returns>True if successful.</returns>
    public bool ReinitializeInput()
    {
        if(this._deviceDescriptor == null)
        {
            this._isInitialized = false;
            return true;
        }
        try
        {
            this.Stop().Wait();
            this._initializeCamera().Wait();
            this._isInitialized = true;
        }catch{}
        this._isInitialized = false;
        return false;
    }
    /// <summary>
    /// Get input type.
    /// </summary>
    /// <returns>Type of input.</returns>
    public InputTypes GetInputType()
    {
        return InputTypes.CAMERA;
    }
    /// <summary>
    /// Release module resources.
    /// </summary>
    public void Release()
    {
        if(this._device != null && this._device.IsRunning)
        {
            this._device.Dispose();
            this._isInitialized = false;
        }
    }

    #endregion

    #region  Live Feed

    private Canvas? _liveCanvas = null;


    /// <summary>
    /// Build live feed controls.
    /// </summary>
    /// <param name="width">Width of panel.</param>
    /// <param name="height">Height of panel.</param>
    /// <param name="rootX">Root x offset.</param>
    /// <param name="rootY">Root y offset.</param>
    /// <returns>Returns new control.</returns>
    public Control BuildLiveControl(int width, int height, int rootX, int rootY)
    {
        Canvas camCanvas = CanvasBuilder.CreateCanvas(width, height, rootX, rootY, $"{this._parameters!.GetParameter<string>("InputName")}_CANVAS");

        TextBlock nameText = TextBlockBuilder.CreateTextBlock(100, 50, 10, 10, $"{this._parameters.GetParameter<string>("InputName")}_NAME_TEXT", $"CAMERA: {this._parameters.GetParameter<string>("InputName")!.ToUpper()}");

        TextBlock fpsText = TextBlockBuilder.CreateTextBlock(100, 50, 10, 40, $"{this._parameters.GetParameter<string>("InputName")}_FPS_TEXT", $"FRAME RATE: {this.FRAME_RATE}");

        TextBlock calibrateText = TextBlockBuilder.CreateTextBlock(100, 50, 10, height - 30, $"{this._parameters.GetParameter<string>("InputName")}_CALIBRATE_TEXT", $"CLICK IMAGE TO OPEN CALIBRATION WINDOW");
        
        Image camFeed = ImageBuilder.CreateImage(width, height, 0, 0, $"{this._parameters.GetParameter<string>("InputName")}_CAMERA");  
        camFeed.PointerPressed += LiveCameraImage_Click;  

        camCanvas.Children.Add(camFeed);
        camCanvas.Children.Add(nameText);
        camCanvas.Children.Add(fpsText);
        camCanvas.Children.Add(calibrateText);

        this._liveCanvas = camCanvas;

        return camCanvas;
    }

    private void LiveCameraImage_Click(object? sender, PointerPressedEventArgs args)
    {       
        if(((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!).Windows.Count == 1)
        {
            var window = new CameraPopup(this);
            window.Width = 1280;
            window.Height = 820;
            window.Show();
            window.Position = new PixelPoint(10, 10);
            window.ShowCalibrationPanel();
            window.Closing += (object? sender, WindowClosingEventArgs e) =>
            {
                this._currentPopup = null;
            };
            this._currentPopup = window;
        }  
    }
    /// <summary>
    /// Update live feed controls.
    /// </summary>
    /// <param name="Root">Root control.</param>
    /// <returns>Updated control.</returns>
    public Control UpdateLiveControl(Control Root)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var fpsText = Global.FindAvaloniaControl<TextBlock>(Root, $"{this._parameters!.GetParameter<string>("InputName")}_FPS_TEXT");
            if(fpsText != null && DateTime.Now.Millisecond < 20)
            {
                fpsText.Text = $"FRAME RATE: {this.FRAME_RATE}";
            }

            var imgControl = Global.FindAvaloniaControl<Avalonia.Controls.Image>(Root, $"{this._parameters.GetParameter<string>("InputName")}_CAMERA");
            if(imgControl != null && this._lastScan != null)
            {
                ImageBuilder.UpdateImage(imgControl, this._lastScan);
            }
            
        });
        return Root;
    }

    /// <summary>
    /// Update the live feed control internally.
    /// </summary>
    public void UpdateLiveControl()
    {
        if(this._liveCanvas != null)
        {
            UpdateLiveControl(this._liveCanvas);
        }        
    }

    #endregion

    #region Analysis Panel

    private Canvas? _analysisCanvas = null;

    private void AnalysisCameraImage_Click(object? sender, PointerPressedEventArgs args)
    {       
        if(((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!).Windows.Count == 1)
        {
            var window = new CameraPopup(this);
            window.Width = 1280;
            window.Height = 920;
            window.Show();
            window.Position = new PixelPoint(10, 10);
            window.ShowAnalysisPanel();
            window.Closing += (object? sender, WindowClosingEventArgs e) =>
            {
                this._currentPopup = null;
            };
            this._currentPopup = window;
        }  
    }
    /// <summary>
    /// Build analysis controls.
    /// </summary>
    /// <param name="width">Width of panel.</param>
    /// <param name="height">Height of panel.</param>
    /// <param name="rootX">Root x offset.</param>
    /// <param name="rootY">Root y offset.</param>
    /// <returns>Returns new control.</returns>
    public Control BuildAnalysisControl(int width, int height, int rootX, int rootY)
    {
        Canvas camCanvas = CanvasBuilder.CreateCanvas(width, height, rootX, rootY, $"{this._parameters!.GetParameter<string>("InputName")}_ANALYSIS_CANVAS");
        camCanvas.PointerPressed += AnalysisCameraImage_Click;

        Avalonia.Controls.Image camFeed = ImageBuilder.CreateImage(width, height, 0, 0, $"{this._parameters.GetParameter<string>("InputName")}_ANALYSIS_CAMERA"); 
        camFeed.PointerPressed += AnalysisCameraImage_Click;    

        TextBlock timestamp = TextBlockBuilder.CreateTextBlock(width, height, 10, 10, $"{this._parameters.GetParameter<string>("InputName")}_ANALYSIS_TIMESTAMP", "");

        camCanvas.Children.Add(camFeed);
        camCanvas.Children.Add(timestamp);

        this._analysisCanvas = camCanvas;

        return camCanvas;
    }
    /// <summary>
    /// Update analysis controls.
    /// </summary>
    /// <param name="Root">Root control.</param>
    /// <returns>Updated control.</returns>
    public Control UpdateAnalysisControl(Control Root)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            
            var imgControl = Global.FindAvaloniaControl<Avalonia.Controls.Image>(Root, $"{this._parameters!.GetParameter<string>("InputName")}_ANALYSIS_CAMERA");
            var timestamp = Global.FindAvaloniaControl<TextBlock>(Root, $"{this._parameters.GetParameter<string>("InputName")}_ANALYSIS_TIMESTAMP");
            if(imgControl != null && Global.RECORDING_FRAMES.Count > 0)
            {
                var img = Global.RECORDING_FRAMES[Global.CURRENT_FRAME_INDEX].GetImage(this._parameters.GetParameter<string>("InputName")!);
                if(img != null)
                {
                    ImageBuilder.UpdateImage(imgControl, img);
                }

                if(timestamp != null)
                {
                    timestamp.Text = Global.RECORDING_FRAMES[Global.CURRENT_FRAME_INDEX].TimeStamp.ToString("MM/dd/yyyy hh:mm:ss.fff");
                }
                
            }
            
        });
        return Root;
    }

    /// <summary>
    /// Update the analysis control internally.
    /// </summary>
    public void UpdateAnalysisControl()
    {
        if(this._analysisCanvas != null)
        {
            UpdateAnalysisControl(this._analysisCanvas);
        }
        
    }
    #endregion

    #region Configuration Window

    private Canvas? _configCanvas = null;

    private void ConfigCameraImage_Click(object? sender, PointerPressedEventArgs args)
    {     
        if(((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!).Windows.Count == 1)
        {
            var window = new CameraPopup(this);
            window.Width = 1280;
            window.Height = 920;
            window.Show();
            window.Position = new PixelPoint(10, 10);
            window.ShowConfigPanel();
            window.Closing += (object? sender, WindowClosingEventArgs e) =>
            {
                this._currentPopup = null;
            };
            this._currentPopup = window;
        }  
        
    }


    public void BuildConfigWindow(int width, int height, int rootX, int rootY)
    {
        Canvas camCanvas = CanvasBuilder.CreateCanvas(width, height, rootX, rootY, $"{this._parameters!.GetParameter<string>("InputName")}_CONFIG_CANVAS");

        Avalonia.Controls.Image camFeed = ImageBuilder.CreateImage(width, height, 0, 0, $"{this._parameters.GetParameter<string>("InputName")}_CONFIG_CAMERA");
        camFeed.PointerPressed += ConfigCameraImage_Click;

        camCanvas.Children.Add(camFeed);

        this._configCanvas = camCanvas;
    }
    public void UpdateConfigWindow()
    {
        if(this._configCanvas != null)
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                var imgControl = Global.FindAvaloniaControl<Avalonia.Controls.Image>(this._configCanvas, $"{this._parameters!.GetParameter<string>("InputName")}_CONFIG_CAMERA");
                if(imgControl != null && this._lastScan != null)
                {
                    ImageBuilder.UpdateImage(imgControl, this._lastScan);
                }
                
            });
        }
        
    }
    #endregion

    #region  Calibration Window
    public void BuildCalibrationWindow(int width, int height, int rootX, int rootY)
    {
        throw new System.NotImplementedException();
    }
    public void UpdateCalibrationWindow()
    {
        throw new System.NotImplementedException();
    }
    #endregion
}