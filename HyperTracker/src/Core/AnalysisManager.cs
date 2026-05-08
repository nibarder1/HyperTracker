using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Threading;
using HyperTracker.UI;
using HyperTracker.UI.UIBuilders;
using OpenCvSharp;

namespace HyperTracker.Core
{
    public class AnalysisManager
    {
        public static Canvas? AnalysisCanvas;
        public static Slider? AnalysisSlider;
        internal static void LoadLast(object? sender, RoutedEventArgs e)
        {
            if(AnalysisCanvas != null)
            {
                Global.CurrentFrame = 0;
                _buildCanvas();
                if(AnalysisSlider != null)
                {
                    AnalysisSlider.Minimum = 0;
                    AnalysisSlider.Maximum = Global.Recording.Frames.Count;
                    AnalysisSlider.Value = Global.CurrentFrame;
                }
            }
        }

        private static void _updateImages()
        {
            if(AnalysisCanvas == null)
            {
                return;
            }
            if(Global.Recording.Frames.Count > 0 && Global.CurrentFrame < Global.Recording.Frames.Count && Global.CurrentFrame >= 0)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    List<string> keys = Global.Recording.Frames[0].FrameImages.Keys.ToList();
                    for(int i = 0; i < keys.Count; i++)
                    {
                        Image? image = UIControl.FindAvaloniaControl<Image>(AnalysisCanvas, $"{keys[i]}_CAMERA_IMAGE");
                        if(image != null)
                        {
                            Mat? frameImage = Global.Recording.Frames[Global.CurrentFrame].GetImage(keys[i]);
                            if(frameImage != null)
                            {
                                ImageBuilder.UpdateImage(image, frameImage);
                            }                            
                        }
                        TextBlock? timeLabel = UIControl.FindAvaloniaControl<TextBlock>(AnalysisCanvas, $"{keys[i]}_TIMESTAMP_LABEL");
                        if(timeLabel != null)
                        {
                            timeLabel.Text = Global.Recording.Frames[Global.CurrentFrame].Timestamp.ToString("MM/dd hh:mm:ss.fff");
                        }
                    }
                });
            }
        }

        private static void _buildCanvas()
        {
            if(Global.Recording.Frames.Count > 0 && Global.CurrentFrame < Global.Recording.Frames.Count && Global.CurrentFrame >= 0)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    AnalysisCanvas!.Children.Clear();
                    List<string> keys = Global.Recording.Frames[0].FrameImages.Keys.ToList();
                    for(int i = 0; i < keys.Count; i++)
                    {
                        int camBox = 400;
                        int maxCamsPerRow = (int)AnalysisCanvas.Width / camBox;
                        int camCanvasHeight = _rows(keys.Count, maxCamsPerRow) * camBox;
                        if (camCanvasHeight > AnalysisCanvas.Height)
                        {
                            AnalysisCanvas.Height = camCanvasHeight;
                        }
                        int x = (i % maxCamsPerRow) * camBox;
                        int y = i / maxCamsPerRow * camBox;
                        
                        Border camera = CameraBuilder.CreateCameraAnalysisWithBorder(camBox, camBox - 100, x, y, keys[i]);


                        AnalysisCanvas.Children.Add(camera);
                    }
                });
            }
            _updateImages();
            
        }

        private static int _rows(int count, int maxRowCount)
        {
            if (count % maxRowCount > 0)
            {
                return count / maxRowCount + 1;
            }
            return count / maxRowCount;
        }

        internal static void PreviousFrame(object? sender, RoutedEventArgs e)
        {
            if(Global.Recording.Frames.Count > 0 && Global.CurrentFrame > 0)
            {
                Global.CurrentFrame--;
                if(AnalysisSlider != null)
                {
                    AnalysisSlider.Value = Global.CurrentFrame;
                }
                _updateImages();
            }
        }
        internal static void NextFrame(object? sender, RoutedEventArgs e)
        {
            if(Global.Recording.Frames.Count > 0 && Global.CurrentFrame < Global.Recording.Frames.Count - 1)
            {
                Global.CurrentFrame++;
                if(AnalysisSlider != null)
                {
                    AnalysisSlider.Value = Global.CurrentFrame;
                }
                _updateImages();
            }
        }

        internal static void SliderChanged(object? sender, RangeBaseValueChangedEventArgs e)
        {
            Slider? slider = (Slider?)sender;
            if(slider != null && Global.CurrentFrame != (int)slider.Value)
            {
                Global.CurrentFrame = (int)slider.Value;
                _updateImages();
            }
        }

        public static void LaunchAnalysisWindow(string name)
        {
            AnalysisWindow window = new AnalysisWindow(name);
            window.Width = 1280;
            window.Height = 720;
            window.Show();
        }
    }
}