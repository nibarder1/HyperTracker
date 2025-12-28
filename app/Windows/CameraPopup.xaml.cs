using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using HyperTracker.Datatypes;
using HyperTracker.Windows.UIBuilders;
using SixLabors.ImageSharp.Formats.Png;


namespace HyperTracker.Windows;

public partial class CameraPopup : Window
{
    private Camera _cam;
    public CameraPopup(Camera cam)
    {
        this._cam = cam;
        this.Title = $"{this._cam.GetParams().GetParameter<string>("InputName")} Configuration";
        this.Topmost= true;
        InitializeComponent();
    }

    public void ShowAnalysisPanel()
    {
        this.Title = $"{this._cam.GetParams().GetParameter<string>("InputName")} Analysis";
        TextBlock textBlock = TextBlockBuilder.CreateTextBlock(200, 50, 10, 10, "ANALYSIS_DISTANCE", "DISTANCE MM: ");
        Image img = ImageBuilder.CreateImage(1200, 700, 0, 100, "CAMERA_FEED");
        img.PointerPressed += cameraFeedImage_Clicked;

        CAMERA_POPUP_CANVAS.Children.Add(textBlock);
        CAMERA_POPUP_CANVAS.Children.Add(img);
        
    }

    public void ShowConfigPanel()
    {
        this.Title = $"{this._cam.GetParams().GetParameter<string>("InputName")} Configuration";
        TextBlock cameraNameLabel = TextBlockBuilder.CreateTextBlock(100,50, 10, 25, "CAMERA_NAME_LABEL", "INPUT NAME");
        TextBox cameraNameInput = TextBoxBuilder.CreateTextBox(150, 50, 250, 10, $"CAMERA_NAME", false);
        cameraNameInput.TextChanged += nameInput_Changed;
        TextBlock cameraFeedSelectorLabel = TextBlockBuilder.CreateTextBlock(100, 50, 10, 85, "CAMERA_SELECTOR_LABEL", "CAMERA SELECT");
        ComboBox cameraFeedSelector = ComboBoxBuilder.CreateComboBox(150, 50, 250, 80, "CAMERA_FEED_SELECTOR");
        TextBlock cameraResolutionLabel = TextBlockBuilder.CreateTextBlock(100, 50, 10, 135, "CAMERA_RESOLUTION_LABEL", "CAMERA RESOLUTION");
        ComboBox cameraResolution = ComboBoxBuilder.CreateComboBox(150, 50, 250, 130, "CAMERA_RESOLUTION");
        TextBlock cameraCalibrationHeightLabel = TextBlockBuilder.CreateTextBlock(100, 50, 10, 190, "CAMERA_CALIBRATION_HEIGHT_LABEL", "CALIBRATION MM");
        NumericUpDown cameraCalibrationHeight = NumberInputBuilder.CreateNumberInput(150, 50, 250, 180, "CAMERA_CALIBRATION_HEIGHT");
        cameraCalibrationHeight.Value = 50;
        TextBlock cameraOffsetLabel = TextBlockBuilder.CreateTextBlock(100, 50, 10, 255, "CAMERA_MEASUREMENT_OFFSET_LABEL", "MEASUREMENT OFFSET MM");
        NumericUpDown cameraOffset = NumberInputBuilder.CreateNumberInput(150, 50, 250, 240, "CAMERA_MEASURE_OFFSET");


        Image img = ImageBuilder.CreateImage(600, 600, 600, 0, "CAMERA_FEED");
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
    }

    public void nameInput_Changed(object? sender, RoutedEventArgs e)
    {
        
    }

    public void cameraFeedImage_Clicked(object? sender, PointerPressedEventArgs e)
    {
        
    }

    public void cameraFeedConfigImage_Clicked(object? sender, PointerPressedEventArgs e)
    {
        
    }
}