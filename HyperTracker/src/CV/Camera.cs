using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using HyperTracker.UI;
using HyperTracker.UI.UIBuilders;
using DirectShowLib;
using OpenCvSharp;
using FlashCap;
using FlashCap.Utilities;
using System.IO;
namespace HyperTracker.CV
{

    #region FLASHCAP MODE
    public class Camera : IDisposable
    {

        private CameraProperties? _cameraProperties { get; set; }
        /// <summary>
        /// Camera.
        /// </summary>
        private CaptureDevice? _camera;
        /// <summary>
        /// Last frame.
        /// </summary>
        private Mat? _lastFrame;
        /// <summary>
        /// Timestamp of last frame.
        /// </summary>
        private DateTime _lastFrameTime;
        private double _realFrameRate = 0;
        /// <summary>
        /// Get last frame.
        /// </summary>
        public Mat? Frame => _lastFrame?.Clone();
        /// <summary>
        /// Get last frame timestamp.
        /// </summary>
        public DateTime FrameTimestamp => _lastFrameTime;
        /// <summary>
        /// Get camera name.
        /// </summary>
        public string CameraName => $"{_cameraProperties!.CameraName} [{_cameraProperties!.CameraIndex}]";
        /// <summary>
        /// Get real frame rate.
        /// </summary>
        public double FrameRate => _cameraProperties!.Resolution.FrameRate;
        public double RealFrameRate => _realFrameRate;
        public CameraProperties? Properties => _cameraProperties;
        /// <summary>
        /// Camera capture thread.
        /// </summary>
        private Thread? _captureThread;

        private Canvas? _setupCanvas;
        private Canvas? _recordingCanvas;
        private Canvas? _analysisCanvas;
        private bool _runThread = true;
        private bool _isInitialized = false;
        private bool _toDispose = false;

        public Camera()
        {
            _lastFrameTime = DateTime.UtcNow;
            _lastFrame = new Mat();
            GlobalEvents.OnExit += _exit;
        }

        public void SetProperties(CameraProperties properties)
        {
            foreach (Camera cam in Global.Cameras)
            {
                if (cam.Properties?.CameraIndex == properties.CameraIndex && cam != this)
                {
                    return;
                }
            }
            Console.WriteLine($"Setting parameters for camera {properties.CameraName}");
            _cameraProperties = properties;
            
            InitCamera();

        }

        private void _exit()
        {
            _runThread = false;
        }

        /// <summary>
        /// Capture a frame.
        /// </summary>
        public void CaptureFrame()
        {
            try
            {
                double fps = Math.Floor(1000 / (DateTime.UtcNow - _lastFrameTime).TotalMilliseconds);
                if (fps < _cameraProperties!.Resolution.FrameRate * 2)
                {
                    _realFrameRate = fps;
                }
                _lastFrameTime = DateTime.UtcNow;
            }
            catch
            {

            }

        }

        public async void InitCamera()
        {
            if (_cameraProperties == null)
            {
                return;
            }
            if (_camera != null)
            {
                _isInitialized = false;
                _camera.StopAsync().Wait(1000);
                _camera.Dispose();
            }
            
            try{
                _camera = await new CaptureDevices().EnumerateDescriptors().ElementAt(_cameraProperties.CameraIndex).OpenAsync(new VideoCharacteristics(PixelFormats.JPEG,
                                                                                                        _cameraProperties.Resolution.Width,
                                                                                                        _cameraProperties.Resolution.Height,
                                                                                                        new Fraction(_cameraProperties.Resolution.FrameRate * 1000, 1000)),
                                                                                                        _processFrame);
            }catch
            {
                _camera = null;
            }
            
            if(_cameraProperties.CalibrationMesh == null || _cameraProperties.CalibrationMesh.MeshPoints.Count == 0)
            {
                _cameraProperties.CalibrationMesh = new CalibrationMesh(Core.CalibrationType.SIMPLE, new List<List<Point>>());
                _cameraProperties.CalibrationMesh.AddColumn();
            }                                                 

            _lastFrame = new Mat(new Size(_cameraProperties.Resolution.Width, _cameraProperties.Resolution.Height), MatType.CV_8UC3);
            _isInitialized = true;
            if(_camera != null)
            {
                await _camera.StartAsync();
                Console.WriteLine("Camera initialized");
            }
            StartThreaded();
        }

        private void _processFrame(PixelBufferScope bufferScope)
        {
            if(_cameraProperties == null)
            {
                return;
            }
            try
            {
                var bitmap = bufferScope.Buffer.ExtractImage();
                
                var mat = Mat.FromImageData(bitmap);
                _lastFrame = mat.Clone();
            }catch
            {
                
            }

            CaptureFrame();
        }

        /// <summary>
        /// Start capture in thread mode.
        /// </summary>
        public void StartThreaded()
        {
            GlobalEvents.OnUpdateLive -= UpdateSetupCanvas;
            GlobalEvents.OnUpdateLive += UpdateSetupCanvas;
            Console.WriteLine($"Total live events: {GlobalEvents.LiveUpdateSubscriptions}");
        }
        /// <summary>
        /// Capture thread.
        /// </summary>
        private void _thread()
        {
            while (_runThread)
            {
                CaptureFrame();
            }
        }

        public static List<CameraProperties> GetAvailableCameras()
        {
            List<CameraProperties> cameras = new List<CameraProperties>();
            var devices = new CaptureDevices();
            var indexer = devices.EnumerateDescriptors().ToList();
            for(int i = 0; i < indexer.Count; i++)
            {
                foreach(CameraResolution res in GetResolutionAndFPS(i))
                {
                    if(res.FrameRate == 1)
                    {
                        continue;
                    }
                    cameras.Add(new CameraProperties(indexer[i].Name,
                                                                indexer[i].Description,
                                                                i,
                                                                1,
                                                                0,
                                                                res,
                                                                new CalibrationMesh(Core.CalibrationType.SIMPLE, new List<List<Point>>())));
                }
            }

            return cameras;
        }

        public static List<CameraResolution> GetResolutionAndFPS(int deviceIndex)
        {
            var results = new List<CameraResolution>();
            var devices = new CaptureDevices();
            var indexer = devices.EnumerateDescriptors().ToList();
            if(deviceIndex < indexer.Count)
            {
                var deviceCharacteristics = indexer[deviceIndex].Characteristics;
                foreach(VideoCharacteristics vc in deviceCharacteristics)
                {
                    results.Add(new CameraResolution(vc.Width, vc.Height, vc.FramesPerSecond.Numerator/vc.FramesPerSecond.Denominator));
                }
            }

            return results;
        }

        internal void RemoveCamera(object? sender, RoutedEventArgs e)
        {
            RemoveCamera();
        }

        public void RemoveCamera()
        {
            this.Dispose();
            _runThread = false;
            _captureThread?.Interrupt();
            while (_captureThread != null && _captureThread.ThreadState == ThreadState.Running)
            {

            }     
            _camera?.Dispose();       
            Global.Cameras.Remove(this);
            GlobalEvents.RebuildUI();
        }

        internal void BindCamera(Canvas cameraCanvas, CanvasType canvasType)
        {
            if (canvasType == CanvasType.SETUP) _setupCanvas = cameraCanvas;
            if (canvasType == CanvasType.RECORDING) _recordingCanvas = cameraCanvas;
            if (canvasType == CanvasType.ANALYSIS) _analysisCanvas = cameraCanvas;
        }

        public void UpdateSetupCanvas()
        {
            try
            {
                Dispatcher.UIThread.Post(() =>
                {

                    if (_setupCanvas != null && Global.CurrentTab == 0)
                    {
                        Image? image = UIControl.FindAvaloniaControl<Image>(_setupCanvas, $"{_setupCanvas.Name}_CAMERA_IMAGE");
                        if (image != null && _lastFrame != null)
                        {
                            ImageBuilder.UpdateImage(image, _lastFrame.Clone());
                        }
                    }
                    if (_recordingCanvas != null && Global.CurrentTab == 1)
                    {
                        Image? image = UIControl.FindAvaloniaControl<Image>(_recordingCanvas, $"{_recordingCanvas.Name}_CAMERA_IMAGE");
                        if (image != null && _lastFrame != null)
                        {
                            ImageBuilder.UpdateImage(image, _lastFrame.Clone());
                        }
                        TextBlock? fpsLabel = UIControl.FindAvaloniaControl<TextBlock>(_recordingCanvas, $"{_recordingCanvas.Name}_FPS_LABEL");
                        if (fpsLabel != null)
                        {
                            fpsLabel.Text = $"FPS: {_realFrameRate}";
                        }
                    }
                    if (_analysisCanvas != null && Global.CurrentTab == 2)
                    {
                        
                        if (Global.Recording.Frames.Count > 0 && Global.CurrentFrame < Global.Recording.Frames.Count)
                        {
                            Image? image = UIControl.FindAvaloniaControl<Image>(_analysisCanvas, $"{_analysisCanvas.Name}_CAMERA_IMAGE");
                            Mat? frameImage = Global.Recording.Frames[Global.CurrentFrame].GetImage(this.CameraName);
                            if(frameImage != null && image != null)
                            {
                                ImageBuilder.UpdateImage(image, frameImage);
                            }
                            TextBlock? timeLabel = UIControl.FindAvaloniaControl<TextBlock>(_analysisCanvas, $"{_analysisCanvas.Name}_TIMESTAMP_LABEL");
                            if (timeLabel != null)
                            {
                                timeLabel.Text = $"{Global.Recording.Frames[Global.CurrentFrame].Timestamp.ToString()}";
                            }                            
                        }                        
                    }


                });
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public void Dispose()
        {
            GlobalEvents.OnUpdateLive -= UpdateSetupCanvas;
            GlobalEvents.OnExit -= _exit;
        }

        internal void propertiesChanged(object? sender, SelectionChangedEventArgs e)
        {
            ComboBox? box = (ComboBox?)sender;
            if (box != null)
            {
                List<CameraProperties> props = GetAvailableCameras();
                if (box.SelectedIndex < props.Count)
                {
                    SetProperties(props[box.SelectedIndex]);
                }
            }
        }

        internal void CalibrateValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
        {
            NumericUpDown? number = (NumericUpDown?)sender;
            if (number != null && number.Value != null && _cameraProperties != null)
            {
                _cameraProperties.CalibrationCentimeters = (double)number.Value;
            }
        }

        internal void OffsetValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
        {
            NumericUpDown? number = (NumericUpDown?)sender;
            if (number != null && number.Value != null && _cameraProperties != null)
            {
                _cameraProperties.MeasurementOffsetCentimeters = (double)number.Value;
            }
        }

        public void _calibrationClick(object? sender, RoutedEventArgs e)
        {
            CalibrationWindow calibrateWindow = new CalibrationWindow(this);
            calibrateWindow.Width = 1280;
            calibrateWindow.Height = 720;
            calibrateWindow.Show();
        }

        internal void AnalyzeImage(object? sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
    #endregion

}