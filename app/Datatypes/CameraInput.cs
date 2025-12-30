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
using Avalonia.Media;
namespace HyperTracker.Datatypes;

public class Camera : iInput
{
    private InputParameters _parameters;
    private SixLabors.ImageSharp.Image? _lastScan;
    private bool _initialized = false;
    private CaptureDeviceDescriptor? _deviceDescriptor;
    private VideoCharacteristics? _deviceCharacteristics;
    private CaptureDevice? _device;
    private bool _parametersValid = false;
    private DateTime _lastFrameTime = DateTime.Now;
    public int FRAME_RATE = 0;
    private CameraPopup? _currentPopup = null;
    public Camera(InputParameters parameters)
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
#region BASIC
    public InputTypes GetInputType()
    {
        return InputTypes.CAMERA;
    }

    public InputParameters GetParams()
    {
        return this._parameters;
    }

    public SixLabors.ImageSharp.Image? GetScan()
    {
        return this._lastScan;
    }

    public bool Initialize()
    {
        try
        {
            this._initializeCamera().Wait();
            this._initialized = true;
            return true;
        }catch(Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return false;
    }

    public bool IsInitialized()
    {
        return this._initialized;
    }

    public bool ReinitializeInput()
    {
        if(this._deviceDescriptor == null)
        {
            this._initialized = false;
            return true;
        }
        try
        {
            this.Stop().Wait();
            this._initializeCamera().Wait();
            this._initialized = true;
        }catch{}
        this._initialized = false;
        return false;
    }

    public async Task Start()
    {
        if(!this._parameters.GetParameter<bool>("IsEnabled"))
        {
            return;
        }
        if(this._device != null && !this._device.IsRunning)
        {
            return;
        }
        if(this._initialized && this._deviceDescriptor != null && this._deviceCharacteristics != null)
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

    public async Task Stop()
    {
        if(this._initialized && this._device != null)
        {
            if(this._device.IsRunning)
            {
                await this._device.StopAsync();
            }            
        }
    }

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

    T? iInput.GetScan<T>() where T : default
    {
        throw new NotImplementedException();
    }

    private async Task _initializeCamera()
    {
        if(!this._parametersValid)
        {
            throw new Exception("Camera parameters not valid.");
        }
        var devices = new CaptureDevices();
        var indexer = devices.EnumerateDescriptors();
        int index = this._parameters.GetParameter<int>("CameraIndex");
        var device = indexer.ElementAt(index);
        Console.WriteLine(device);
        var deviceCharacteristics = device.Characteristics[0];
        VideoCharacteristics settings = new VideoCharacteristics(PixelFormats.JPEG,
                                                                1280,
                                                                720,
                                                                new Fraction(120000,1000));
        this._deviceDescriptor = device;
        this._deviceCharacteristics = settings;
    }

    private async void _processFrame(PixelBufferScope buffer)
    {
        try
        {
            DateTime currentTime = DateTime.Now;
            byte[] bufferBytes = buffer.Buffer.ExtractImage();
            var ms = new MemoryStream(bufferBytes);
            this._lastScan = SixLabors.ImageSharp.Image.Load(ms);
            double timeMS = (currentTime - this._lastFrameTime).TotalMilliseconds;
            this._lastFrameTime = currentTime;
            this.FRAME_RATE = (int)(1000/timeMS);
        }catch
        {
            Console.WriteLine($"Error processing frame for camera: {this._parameters.GetParameter<string>("InputName")}");
        }
        
    }

    
#endregion

#region LIVE TAB
    public Control BuildLiveControl(int width, int height, int rootX, int rootY)
    {
        Canvas camCanvas = CanvasBuilder.CreateCanvas(width, height, rootX, rootY, $"{this._parameters.GetParameter<string>("InputName")}_CANVAS");

        TextBlock fpsText = TextBlockBuilder.CreateTextBlock(width, height, 10, 10, $"{this._parameters.GetParameter<string>("InputName")}_FPS_TEXT", $"FRAME RATE: {this.FRAME_RATE}");
        
        Image camFeed = ImageBuilder.CreateImage(width, height, 0, 0, $"{this._parameters.GetParameter<string>("InputName")}_CAMERA");  

        camCanvas.Children.Add(camFeed);
        camCanvas.Children.Add(fpsText);

        return camCanvas;
    }

    public Control UpdateLiveControl(Avalonia.Controls.Control Root)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var canvas = Global.FindAvaloniaControl<Canvas>(Root, $"{this._parameters.GetParameter<string>("InputName")}_CANVAS");
            if(canvas != null)
            {
                var fpsText = Global.FindAvaloniaControl<TextBlock>(Root, $"{this._parameters.GetParameter<string>("InputName")}_FPS_TEXT");
                if(fpsText != null)
                {
                    fpsText.Text = $"FRAME RATE: {this.FRAME_RATE}";
                }

                var imgControl = Global.FindAvaloniaControl<Avalonia.Controls.Image>(Root, $"{this._parameters.GetParameter<string>("InputName")}_CAMERA");
                if(imgControl != null && this._lastScan != null)
                {
                    ImageBuilder.UpdateImage(imgControl, this._lastScan);
                }
            }
            
        });

        return Root;
    }
#endregion

#region  ANALYSIS TAB
    private void AnalysisCameraImage_Click(object? sender, PointerPressedEventArgs args)
    {       
        if(((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!).Windows.Count == 1)
        {
            var window = new CameraPopup(this);
            window.Width = 1200;
            window.Height = 800;
            window.Show();
            window.ShowAnalysisPanel();
            window.Closing += (object? sender, WindowClosingEventArgs e) =>
            {
                this._currentPopup = null;
            };
            this._currentPopup = window;
        }  
    }

    public Control UpdateAnalysisControl(Avalonia.Controls.Control Root)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var canvas = Global.FindAvaloniaControl<Canvas>(Root, $"{this._parameters.GetParameter<string>("InputName")}_ANALYSIS_CANVAS");
            if(canvas != null)
            {
                var imgControl = Global.FindAvaloniaControl<Avalonia.Controls.Image>(Root, $"{this._parameters.GetParameter<string>("InputName")}_ANALYSIS_CAMERA");
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
            }
            
        });
        return Root;
    }

    public Control BuildAnalysisControl(int width, int height, int rootX, int rootY)
    {
        Canvas camCanvas = CanvasBuilder.CreateCanvas(width, height, rootX, rootY, $"{this._parameters.GetParameter<string>("InputName")}_ANALYSIS_CANVAS");
        camCanvas.PointerPressed += AnalysisCameraImage_Click;

        Avalonia.Controls.Image camFeed = ImageBuilder.CreateImage(width, height, 0, 0, $"{this._parameters.GetParameter<string>("InputName")}_ANALYSIS_CAMERA"); 
        camFeed.PointerPressed += AnalysisCameraImage_Click;    

        TextBlock timestamp = TextBlockBuilder.CreateTextBlock(width, height, 10, 10, $"{this._parameters.GetParameter<string>("InputName")}_ANALYSIS_TIMESTAMP", "");

        camCanvas.Children.Add(camFeed);
        camCanvas.Children.Add(timestamp);

        return camCanvas;
    }
#endregion

#region CONFIGURATION TAB
    private void ConfigCameraImage_Click(object? sender, PointerPressedEventArgs args)
    {     
        if(((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!).Windows.Count == 1)
        {
            var window = new CameraPopup(this);
            window.Width = 1200;
            window.Height = 800;
            window.Show();
            window.ShowConfigPanel();
            window.Closing += (object? sender, WindowClosingEventArgs e) =>
            {
                this._currentPopup = null;
            };
            this._currentPopup = window;
        }  
        
    }

    public Control BuildConfigControl(int width, int height, int rootX, int rootY)
    {
        Canvas camCanvas = CanvasBuilder.CreateCanvas(width, height, rootX, rootY, $"{this._parameters.GetParameter<string>("InputName")}_CONFIG_CANVAS");

        Avalonia.Controls.Image camFeed = ImageBuilder.CreateImage(width, height, 0, 0, $"{this._parameters.GetParameter<string>("InputName")}_CONFIG_CAMERA");
        camFeed.PointerPressed += ConfigCameraImage_Click;

        camCanvas.Children.Add(camFeed);

        return camCanvas;
    }

    public Control UpdateConfigControl(Control Root)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var canvas = Global.FindAvaloniaControl<Canvas>(Root, $"{this._parameters.GetParameter<string>("InputName")}_CONFIG_CANVAS");
            if(canvas != null)
            {
                var imgControl = Global.FindAvaloniaControl<Avalonia.Controls.Image>(Root, $"{this._parameters.GetParameter<string>("InputName")}_CONFIG_CAMERA");
                if(imgControl != null && this._lastScan != null)
                {
                    ImageBuilder.UpdateImage(imgControl, this._lastScan);
                }
            }
            
        });

        return Root;
    }
#endregion

#region POPUP WINDOW
    
    public Control UpdatePopupAnalysisControl(Control Root)
    {
        throw new NotImplementedException();
    }

    public Control UpdatePopupConfigControl(Control Root)
    {
        throw new NotImplementedException();
    }

    public void nameInput_Changed(object? sender, RoutedEventArgs e)
    {
        
    }
#endregion
    

    
}