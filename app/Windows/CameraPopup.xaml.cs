using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using HyperTracker.Config;
using HyperTracker.Datatypes;
using HyperTracker.Windows.UIBuilders;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.Processing;



namespace HyperTracker.Windows;

public partial class CameraPopup : Window
{
    private Camera _cam;
    private int cameraSettingIndex = -1;
    private double measurePoint = -1;
    public CameraPopup(Camera cam)
    {
        this._cam = cam;
        this.Title = $"{this._cam.GetParams().GetParameter<string>("InputName")} Configuration";
        this.Topmost= true;
        InitializeComponent();
    }

    public void ShowAnalysisPanel()
    {
        for(int i = 0; i < Global.PROFILE_SETTINGS[Global.LOADED_PROFILE].Cameras.Count; i++)
        {
            CameraSettings camSettings = Global.PROFILE_SETTINGS[Global.LOADED_PROFILE].Cameras[i];
            if(camSettings.CameraName.Equals(this._cam.GetParams().GetParameter<string>("InputName")))
            {
                this.cameraSettingIndex = i;
                break;
            }
        }
        this.Title = $"{this._cam.GetParams().GetParameter<string>("InputName")} Analysis";
        TextBlock textBlock = TextBlockBuilder.CreateTextBlock(200, 50, 10, 10, "ANALYSIS_DISTANCE", "DISTANCE MM: ");
        TextBlock helpBlock = TextBlockBuilder.CreateTextBlock(200, 50, 10, 100, "MEASURE_HELP", "CLICK ON IMAGE TO MEASURE");
        Avalonia.Controls.Image img = ImageBuilder.CreateImage(1280, 720, 0, 100, "CAMERA_FEED");
        
        SixLabors.ImageSharp.Image? frameImage = Global.RECORDING_FRAMES[Global.CURRENT_FRAME_INDEX].GetImage(this._cam.GetParams().GetParameter<string>("InputName")!);
        if(frameImage != null)
        {
            ImageBuilder.UpdateImage(img, frameImage);
        }  
        img.PointerPressed += cameraFeedAnalysisImage_Clicked;
        

        CAMERA_POPUP_CANVAS.Children.Add(textBlock);
        CAMERA_POPUP_CANVAS.Children.Add(img);
        CAMERA_POPUP_CANVAS.Children.Add(helpBlock);
        UpdateAnalysisPanel();
    }

    public void ShowConfigPanel()
    {
        for(int i = 0; i < Global.PROFILE_SETTINGS[Global.LOADED_PROFILE].Cameras.Count; i++)
        {
            CameraSettings camSettings = Global.PROFILE_SETTINGS[Global.LOADED_PROFILE].Cameras[i];
            if(camSettings.CameraName.Equals(this._cam.GetParams().GetParameter<string>("InputName")))
            {
                this.cameraSettingIndex = i;
                break;
            }
        }
        this.Title = $"{this._cam.GetParams().GetParameter<string>("InputName")} Configuration";
        TextBlock cameraNameLabel = TextBlockBuilder.CreateTextBlock(100,50, 10, 25, "CAMERA_NAME_LABEL", "NAME");
        TextBox cameraNameInput = TextBoxBuilder.CreateTextBox(200, 50, 10, 50, $"CAMERA_NAME", false);
        cameraNameInput.TextChanged += nameInput_Changed;
        TextBlock cameraFeedSelectorLabel = TextBlockBuilder.CreateTextBlock(100, 50, 220, 25, "CAMERA_SELECTOR_LABEL", "CAMERA SELECT");
        ComboBox cameraFeedSelector = ComboBoxBuilder.CreateComboBox(200, 50, 220, 50, "CAMERA_FEED_SELECTOR");
        TextBlock cameraResolutionLabel = TextBlockBuilder.CreateTextBlock(100, 50, 430, 25, "CAMERA_RESOLUTION_LABEL", "CAMERA RESOLUTION");
        ComboBox cameraResolution = ComboBoxBuilder.CreateComboBox(200, 50, 430, 50, "CAMERA_RESOLUTION");        
        TextBlock cameraOffsetLabel = TextBlockBuilder.CreateTextBlock(100, 50, 640, 25, "CAMERA_MEASUREMENT_OFFSET_LABEL", "MEASUREMENT OFFSET MM");
        NumericUpDown cameraOffset = NumberInputBuilder.CreateNumberInput(200, 50, 640, 50, "CAMERA_MEASURE_OFFSET");
        TextBlock baseLineText = TextBlockBuilder.CreateTextBlock(100, 50, 10, 110, "MEASURE_BASE_LINE_TEXT", "BASE LINE");
        NumericUpDown baseLine = NumberInputBuilder.CreateNumberInput(200, 50, 10, 140, "MEASURE_BASE_LINE");
        TextBlock calibrationLineText = TextBlockBuilder.CreateTextBlock(100, 50, 220, 110, "MEASURE_CALIBRATION_LINE_TEXT", "CALIBRATE LINE");
        NumericUpDown calibrationLine = NumberInputBuilder.CreateNumberInput(200, 50, 220, 140, "MEASURE_CALIBRATION_LINE");
        TextBlock cameraCalibrationHeightLabel = TextBlockBuilder.CreateTextBlock(100, 50, 430, 110, "CAMERA_CALIBRATION_HEIGHT_LABEL", "CALIBRATION MM");
        NumericUpDown cameraCalibrationHeight = NumberInputBuilder.CreateNumberInput(200, 50, 430, 140, "CAMERA_CALIBRATION_HEIGHT");
        cameraCalibrationHeight.Value = 50;
        TextBlock baseLineHelp = TextBlockBuilder.CreateTextBlock(100, 50, 10, 200, "BASE_LINE_HELP", "CLICK IMAGE TO PLACE BASE LINE.");



        Avalonia.Controls.Image img = ImageBuilder.CreateImage(1280, 720, 0, 200, "CAMERA_FEED");
        if(this._cam.GetScan() != null)
        {
            ImageBuilder.UpdateImage(img, this._cam.GetScan()!);
        } 
        img.PointerPressed += cameraFeedConfigImage_Clicked;
        CAMERA_POPUP_CANVAS.Children.Add(cameraNameLabel);
        CAMERA_POPUP_CANVAS.Children.Add(cameraNameInput);
        CAMERA_POPUP_CANVAS.Children.Add(cameraFeedSelectorLabel);
        CAMERA_POPUP_CANVAS.Children.Add(cameraFeedSelector);
        CAMERA_POPUP_CANVAS.Children.Add(cameraResolutionLabel);
        CAMERA_POPUP_CANVAS.Children.Add(cameraResolution);
        CAMERA_POPUP_CANVAS.Children.Add(cameraCalibrationHeightLabel);
        CAMERA_POPUP_CANVAS.Children.Add(cameraCalibrationHeight);
        CAMERA_POPUP_CANVAS.Children.Add(cameraOffsetLabel);
        CAMERA_POPUP_CANVAS.Children.Add(cameraOffset);
        CAMERA_POPUP_CANVAS.Children.Add(img);
        CAMERA_POPUP_CANVAS.Children.Add(baseLineText);
        CAMERA_POPUP_CANVAS.Children.Add(baseLine);
        CAMERA_POPUP_CANVAS.Children.Add(calibrationLineText);
        CAMERA_POPUP_CANVAS.Children.Add(calibrationLine);
        CAMERA_POPUP_CANVAS.Children.Add(baseLineHelp);
    }

    public void ShowCalibrationPanel()
    {
        for(int i = 0; i < Global.PROFILE_SETTINGS[Global.LOADED_PROFILE].Cameras.Count; i++)
        {
            CameraSettings camSettings = Global.PROFILE_SETTINGS[Global.LOADED_PROFILE].Cameras[i];
            if(camSettings.CameraName.Equals(this._cam.GetParams().GetParameter<string>("InputName")))
            {
                this.cameraSettingIndex = i;
                break;
            }
        }
        this.Title = $"{this._cam.GetParams().GetParameter<string>("InputName")} Calibration";
        TextBlock baseLineText = TextBlockBuilder.CreateTextBlock(100, 50, 10, 10, "MEASURE_BASE_LINE_TEXT", "BASE LINE %");
        NumericUpDown baseLine = NumberInputBuilder.CreateNumberInput(200, 50, 10, 40, "MEASURE_BASE_LINE");
        baseLine.Value = (decimal)Global.PROFILE_SETTINGS[Global.LOADED_PROFILE].Cameras[cameraSettingIndex].MEASURE_FLOOR_POSITION;
        baseLine.Increment = 0.1M;
        baseLine.ValueChanged += calibrateBaseLine_Changed;
        TextBlock calibrationLineText = TextBlockBuilder.CreateTextBlock(100, 50, 220, 10, "MEASURE_CALIBRATION_LINE_TEXT", "CALIBRATE LINE %");
        NumericUpDown calibrationLine = NumberInputBuilder.CreateNumberInput(200, 50, 220, 40, "MEASURE_CALIBRATION_LINE");
        calibrationLine.Value = (decimal)Global.PROFILE_SETTINGS[Global.LOADED_PROFILE].Cameras[cameraSettingIndex].CALIBRATION_LINE_POSITION;
        calibrationLine.Increment = 0.1M;
        calibrationLine.ValueChanged += calibrateCalibrateLine_Changed;
        TextBlock cameraCalibrationHeightLabel = TextBlockBuilder.CreateTextBlock(100, 50, 430, 10, "CAMERA_CALIBRATION_HEIGHT_LABEL", "CALIBRATION MM");
        NumericUpDown cameraCalibrationHeight = NumberInputBuilder.CreateNumberInput(200, 50, 430, 40, "CAMERA_CALIBRATION_HEIGHT");
        cameraCalibrationHeight.Value = 50;
        TextBlock cameraOffsetLabel = TextBlockBuilder.CreateTextBlock(100, 50, 640, 10, "CAMERA_MEASUREMENT_OFFSET_LABEL", "MEASUREMENT OFFSET MM");
        NumericUpDown cameraOffset = NumberInputBuilder.CreateNumberInput(200, 50, 640, 40, "CAMERA_MEASURE_OFFSET");
        cameraOffset.Value = (decimal)Global.PROFILE_SETTINGS[Global.LOADED_PROFILE].Cameras[cameraSettingIndex].MEASUREMENT_OFFSET_MM;
        cameraOffset.ValueChanged += calibrateOffset_Changed;
        TextBlock baseLineHelp = TextBlockBuilder.CreateTextBlock(100, 50, 10, 100, "BASE_LINE_HELP", "LEFT CLICK IMAGE TO PLACE BASE LINE.\nRIGHT CLICK IMAGE TO PLACE CALIBRATION LINE.");



        Avalonia.Controls.Image img = ImageBuilder.CreateImage(1280, 720, 0, 100, "CAMERA_FEED");
        if(this._cam.GetScan() != null)
        {
            ImageBuilder.UpdateImage(img, this._cam.GetScan()!);
        }        
        img.PointerPressed += cameraFeedCalibrateImage_Clicked;
        CAMERA_POPUP_CANVAS.Children.Add(img);
        CAMERA_POPUP_CANVAS.Children.Add(baseLineText);
        CAMERA_POPUP_CANVAS.Children.Add(baseLine);
        CAMERA_POPUP_CANVAS.Children.Add(calibrationLineText);
        CAMERA_POPUP_CANVAS.Children.Add(calibrationLine);
        CAMERA_POPUP_CANVAS.Children.Add(cameraCalibrationHeightLabel);
        CAMERA_POPUP_CANVAS.Children.Add(cameraCalibrationHeight);
        CAMERA_POPUP_CANVAS.Children.Add(cameraOffsetLabel);
        CAMERA_POPUP_CANVAS.Children.Add(cameraOffset);
        CAMERA_POPUP_CANVAS.Children.Add(baseLineHelp);
    }

    public void UpdateCalibratePanel()
    {
        var baseLine = Global.FindAvaloniaControl<NumericUpDown>(CAMERA_POPUP_CANVAS, "MEASURE_BASE_LINE");
        if(baseLine != null && baseLine.Value != (decimal)Global.PROFILE_SETTINGS[Global.LOADED_PROFILE].Cameras[cameraSettingIndex].MEASURE_FLOOR_POSITION)
        {
            baseLine.Value = (decimal)Global.PROFILE_SETTINGS[Global.LOADED_PROFILE].Cameras[cameraSettingIndex].MEASURE_FLOOR_POSITION;
        }

        var calibrateLine = Global.FindAvaloniaControl<NumericUpDown>(CAMERA_POPUP_CANVAS, "MEASURE_CALIBRATION_LINE");
        if(calibrateLine != null && calibrateLine.Value != (decimal)Global.PROFILE_SETTINGS[Global.LOADED_PROFILE].Cameras[cameraSettingIndex].CALIBRATION_LINE_POSITION)
        {
            calibrateLine.Value = (decimal)Global.PROFILE_SETTINGS[Global.LOADED_PROFILE].Cameras[cameraSettingIndex].CALIBRATION_LINE_POSITION;
        }
        
        var image = Global.FindAvaloniaControl<Avalonia.Controls.Image>(CAMERA_POPUP_CANVAS, "CAMERA_FEED");
        if(image != null)
        {
            SixLabors.ImageSharp.Image? scan = this._cam.GetScan();
            if(scan != null)
            {
                float baseY = (float)Global.PROFILE_SETTINGS[Global.LOADED_PROFILE].Cameras[cameraSettingIndex].MEASURE_FLOOR_POSITION / 100 * scan.Height;
                PointF baseStart = new PointF(0, baseY);
                PointF baseEnd = new PointF(scan.Width, baseY);
                var basePoints = new PointF[]
                {
                  baseStart,
                  baseEnd  
                };
                
                float calibrateY = (float)Global.PROFILE_SETTINGS[Global.LOADED_PROFILE].Cameras[cameraSettingIndex].CALIBRATION_LINE_POSITION / 100 * scan.Height;
                PointF calibrateStart = new PointF(0, calibrateY);
                PointF calibrateEnd = new PointF(scan.Width, calibrateY);
                var calibratePoints = new PointF[]
                {
                    calibrateStart,
                    calibrateEnd  
                };

                scan.Mutate(imageContext =>
                {
                    imageContext.DrawLine(Color.Blue, 2, basePoints);
                    imageContext.DrawLine(Color.Green, 2, calibratePoints);
                });
                ImageBuilder.UpdateImage(image, scan);

                
                var calibrationInput = Global.FindAvaloniaControl<Avalonia.Controls.NumericUpDown>(CAMERA_POPUP_CANVAS, "CAMERA_CALIBRATION_HEIGHT");
                if(calibrationInput != null)
                {
                    double calibration = Math.Round(Math.Abs(baseY - calibrateY) / (double)calibrationInput.Value!, 4);
                    Global.PROFILE_SETTINGS[Global.LOADED_PROFILE].Cameras[cameraSettingIndex].PIXELS_PER_MM = calibration;
                }

                var offsetInput = Global.FindAvaloniaControl<Avalonia.Controls.NumericUpDown>(CAMERA_POPUP_CANVAS, "CAMERA_MEASURE_OFFSET");
                if(offsetInput != null)
                {
                    Global.PROFILE_SETTINGS[Global.LOADED_PROFILE].Cameras[cameraSettingIndex].MEASUREMENT_OFFSET_MM = (double)offsetInput.Value!;
                }
            }
        }
    }

    public void UpdateAnalysisPanel()
    {        
        var image = Global.FindAvaloniaControl<Avalonia.Controls.Image>(CAMERA_POPUP_CANVAS, "CAMERA_FEED");
        if(image != null)
        {
            SixLabors.ImageSharp.Image? scan = Global.RECORDING_FRAMES[Global.CURRENT_FRAME_INDEX].GetImage(this._cam.GetParams().GetParameter<string>("InputName")!)!.Clone(ctx => {});
            if(scan != null)
            {
                float baseY = (float)Global.PROFILE_SETTINGS[Global.LOADED_PROFILE].Cameras[cameraSettingIndex].MEASURE_FLOOR_POSITION / 100 * scan.Height;
                PointF baseStart = new PointF(0, baseY);
                PointF baseEnd = new PointF(scan.Width, baseY);
                var basePoints = new PointF[]
                {
                baseStart,
                baseEnd  
                };

                float calibrateY = (float)this.measurePoint / 100 * scan.Height;
                PointF calibrateStart = new PointF(0, calibrateY);
                PointF calibrateEnd = new PointF(scan.Width, calibrateY);
                var calibratePoints = new PointF[]
                {
                    calibrateStart,
                    calibrateEnd  
                };

                scan.Mutate(imageContext =>
                {
                    imageContext.DrawLine(Color.Blue, 2, basePoints);
                    imageContext.DrawLine(Color.Green, 2, calibratePoints);
                });
                ImageBuilder.UpdateImage(image, scan);

                //Calculate distance
                double pixelsPerMM = Global.PROFILE_SETTINGS[Global.LOADED_PROFILE].Cameras[cameraSettingIndex].PIXELS_PER_MM;
                double distance = Math.Round(Math.Abs(baseY - calibrateY) / pixelsPerMM, 2) + Global.PROFILE_SETTINGS[Global.LOADED_PROFILE].Cameras[cameraSettingIndex].MEASUREMENT_OFFSET_MM;
                var distanceText = Global.FindAvaloniaControl<Avalonia.Controls.TextBlock>(CAMERA_POPUP_CANVAS, "ANALYSIS_DISTANCE");
                if(distanceText != null)
                {
                    distanceText.Text = $"DISTANCE MM: {distance}";
                }
                
            }

            

        }
    }

    public void nameInput_Changed(object? sender, RoutedEventArgs e)
    {
        
    }

    public void calibrateBaseLine_Changed(object? sender, RoutedEventArgs e)
    {
        var number = sender as NumericUpDown;
        if(number != null)
        {
            if(number.Value >= 0 && number.Value <= 100)
            {
                Global.PROFILE_SETTINGS[Global.LOADED_PROFILE].Cameras[cameraSettingIndex].MEASURE_FLOOR_POSITION = (double)number.Value!;
            }
            UpdateCalibratePanel();
        }
    }
    public void calibrateCalibrateLine_Changed(object? sender, RoutedEventArgs e)
    {
        var number = sender as NumericUpDown;
        if(number != null)
        {
            if(number.Value >= 0 && number.Value <= 100)
            {
                Global.PROFILE_SETTINGS[Global.LOADED_PROFILE].Cameras[cameraSettingIndex].CALIBRATION_LINE_POSITION = (double)number.Value!;
            }
            UpdateCalibratePanel();
        }
    }

    public void calibrateOffset_Changed(object? sender, RoutedEventArgs e)
    {
        var number = sender as NumericUpDown;
        if(number != null)
        {
            Global.PROFILE_SETTINGS[Global.LOADED_PROFILE].Cameras[cameraSettingIndex].MEASUREMENT_OFFSET_MM = (double)number.Value!;
            UpdateCalibratePanel();
        }
    }

    public void cameraFeedCalibrateImage_Clicked(object? sender, PointerPressedEventArgs e)
    {
        if(sender != null)
        {
            double Position = GetPosition(sender, e);
            if(e.Properties.IsLeftButtonPressed)
            {
                //Base Line
                Global.PROFILE_SETTINGS[Global.LOADED_PROFILE].Cameras[cameraSettingIndex].MEASURE_FLOOR_POSITION = Position;
            }
            if(e.Properties.IsRightButtonPressed)
            {
                //Calibrate Line
                Global.PROFILE_SETTINGS[Global.LOADED_PROFILE].Cameras[cameraSettingIndex].CALIBRATION_LINE_POSITION = Position;
            }
            UpdateCalibratePanel();
        }
        
    }

    public void cameraFeedAnalysisImage_Clicked(object? sender, PointerPressedEventArgs e)
    {
        if(sender != null)
        {
            this.measurePoint = GetPosition(sender, e);
            UpdateAnalysisPanel();
        }
    }

    public void cameraFeedConfigImage_Clicked(object? sender, PointerPressedEventArgs e)
    {
        
    }

    private double GetPosition(object sender, PointerPressedEventArgs e)
    {
        var image = (Avalonia.Controls.Image)sender;        
        double xRel = e.GetPosition(image).X / image.Bounds.Width;
        double yRel = e.GetPosition(image).Y / image.Bounds.Height;
        if(Global.PROFILE_SETTINGS[Global.LOADED_PROFILE].TrackingMode == TrackMode.VERTICLE_DISTANCE || Global.PROFILE_SETTINGS[Global.LOADED_PROFILE].TrackingMode == TrackMode.VERTICLE_TIME)
        {
            return Math.Round(yRel * 100, 2);
        }
        return -1;
    }
}