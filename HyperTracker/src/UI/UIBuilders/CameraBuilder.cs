using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using HyperTracker.Core;
using HyperTracker.CV;

namespace HyperTracker.UI.UIBuilders
{
    public class CameraBuilder
    {
        public static Border CreateCameraSetupWithBorder(int width, int height, int rootX, int rootY, string elementName, Camera camera)
        {
            int imageWidth = width - 20;
            int imageHeight = height / 2;
            if(camera != null && camera.Properties != null)
            {
                double aspectRatio = (double)camera.Properties.Resolution.Width / camera.Properties.Resolution.Height;
                imageWidth = (int)(imageHeight * aspectRatio);
            }
            
            Border border = CanvasBuilder.CreateCanvasWithBorder(width, height, rootX, rootY, $"{elementName}_BORDER");
            Canvas canvas = (Canvas)border.Child!;
            canvas.Name = elementName;
            Button deleteCamera = ButtonBuilder.CreateButton(30, 30, (int)border.Width - 40, 10, $"{elementName}_REMOVE_CAMERA_BUTTON", "X", camera.RemoveCamera);
            Image image = ImageBuilder.CreateImage(imageWidth, imageHeight, 10, 50, $"{elementName}_CAMERA_IMAGE");
            ComboBox cameras = ComboBoxBuilder.CreateComboBox(200, 30, 10, 10, $"{elementName}_CAMERA_SELECTOR");
            List<CameraProperties> availableCameras = Camera.GetAvailableCameras();
            List<string> options = new List<string>();
            foreach(CameraProperties prop in availableCameras)
            {
                if(camera.Properties != null)
                {
                    prop.CalibrationCentimeters = camera.Properties.CalibrationCentimeters;
                    prop.MeasurementOffsetCentimeters = camera.Properties.MeasurementOffsetCentimeters;
                }
                options.Add(prop.ToString());
            }
            cameras.ItemsSource = options;
            cameras.SelectedItem = camera.Properties?.ToString();
            cameras.SelectionChanged += camera.propertiesChanged;

            Border calibrationLabel = TextBlockBuilder.CreateTextBlockWithBox(width/2 - 20, 30, 10, imageHeight + 55, $"{elementName}_CALIBRATION_LABEL", "CALIBRATE CM");
            Border offsetLabel = TextBlockBuilder.CreateTextBlockWithBox(width/2 - 20, 30, 10, imageHeight + 95, $"{elementName}_MEASURE_OFFSET_LABEL", "OFFSET CM");
            NumericUpDown calibateValue = NumberInputBuilder.CreateNumberInput(width/2 - 20, 30, width/2, imageHeight + 55, $"{elementName}_CALIBRATION_VALUE");
            calibateValue.Minimum = 0;
            calibateValue.Maximum = 1000;            
            if(camera.Properties != null)
            {
                calibateValue.Value = (decimal)camera.Properties.CalibrationCentimeters;
            }
            calibateValue.ValueChanged += camera.CalibrateValueChanged;
            NumericUpDown offsetValue = NumberInputBuilder.CreateNumberInput(width/2 - 20, 30, width/2, imageHeight + 95, $"{elementName}_OFFSET_VALUE");
            offsetValue.Minimum = 0;
            offsetValue.Maximum = 1000;            
            if(camera.Properties != null)
            {
                offsetValue.Value = (decimal)camera.Properties.MeasurementOffsetCentimeters;
            }
            offsetValue.ValueChanged += camera.OffsetValueChanged;
            Button calibrateButton = ButtonBuilder.CreateButton(width/2 - 20, 30, width/2, imageHeight + 135, $"{elementName}_CALLIBRATE_BUTTON", "CALIBRATE", camera._calibrationClick);  
            

            canvas.Children.Add(deleteCamera);
            canvas.Children.Add(cameras);            
            canvas.Children.Add(image);
            canvas.Children.Add(calibrationLabel);
            canvas.Children.Add(offsetLabel);
            canvas.Children.Add(calibateValue);
            canvas.Children.Add(offsetValue);
            canvas.Children.Add(calibrateButton);

            camera.BindCamera(canvas, CanvasType.SETUP);
            return border;
        }

        public static Border CreateCameraRecordingWithBorder(int width, int height, int rootX, int rootY, string elementName, Camera camera)
        {
            Border border = CanvasBuilder.CreateCanvasWithBorder(width, height, rootX, rootY, $"{elementName}_BORDER");
            Canvas canvas = (Canvas)border.Child!;
            canvas.Name = elementName;
            Image image = ImageBuilder.CreateImage(width - 20, height - 50, 10, 50, $"{elementName}_CAMERA_IMAGE");
            Border fpsLabel = TextBlockBuilder.CreateTextBlockWithBox(width/2 - 20, 30, 10, 10, $"{elementName}_FPS_LABEL", "FPS: ");
             
            canvas.Children.Add(image);
            canvas.Children.Add(fpsLabel);

            camera.BindCamera(canvas, CanvasType.RECORDING);
            return border;
        }

        public static Border CreateCameraAnalysisWithBorder(int width, int height, int rootX, int rootY, string elementName)
        {
            Border border = CanvasBuilder.CreateCanvasWithBorder(width, height, rootX, rootY, $"{elementName}_BORDER");
            Canvas canvas = (Canvas)border.Child!;
            canvas.Name = elementName;
            Image image = ImageBuilder.CreateImage(width - 20, height - 50, 10, 50, $"{elementName}_CAMERA_IMAGE");
            Border timeLabel = TextBlockBuilder.CreateTextBlockWithBox(width/2 - 20, 30, 10, 10, $"{elementName}_TIMESTAMP_LABEL", "");
            Button analyzeButton = ButtonBuilder.CreateButton(width/2 - 20, 30, width/2, 10, $"{elementName}_ANALYZE_BUTTON", "ANALYZE", (object? sender, RoutedEventArgs e) => {AnalysisManager.LaunchAnalysisWindow(elementName);});
             
            canvas.Children.Add(image);
            canvas.Children.Add(timeLabel);
            canvas.Children.Add(analyzeButton);

            return border;
        }        
    }
}