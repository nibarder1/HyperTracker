using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using HyperTracker.CV;
using HyperTracker.UI.UIBuilders;
using OpenCvSharp;

namespace HyperTracker.UI
{
    public partial class AnalysisWindow : Avalonia.Controls.Window
    {
        private string _cameraName = "";
        private CV.Point? _measurePoint;
        public AnalysisWindow(string name)
        {
            this._cameraName = name;
            this.Title = "Analysis";
            this.Topmost = true;
            this.SizeChanged += _resize;
            this.MinHeight = 720;
            this.MinWidth = 1280;

            InitializeComponent();
        }

        private void _resize(object? sender, SizeChangedEventArgs e)
        {
            _init();
        }

        private void _init()
        {
            Border controlPanel = CanvasBuilder.CreateCanvasWithBorder((int)this.ClientSize.Width, 50, 0, 0, $"ANALYSIS_CONTROL_PANEL");
            Border distanceLabel = TextBlockBuilder.CreateTextBlockWithBox(250, 30, 10, 10, $"ANALYSIS_DISTANCE", "DISTANCE:");

            CanvasBuilder.AddElement(controlPanel, distanceLabel);

            Border optionPanel = CanvasBuilder.CreateCanvasWithBorder(300, (int)this.ClientSize.Height - 70, 10, 60, $"ANALYSIS_OPTION_PANEL");


            ANALYSIS_CANVAS.Children.Clear();
            ANALYSIS_CANVAS.Children.Add(controlPanel);
            ANALYSIS_CANVAS.Children.Add(optionPanel);

            
            int imageWidth = (int)(this.ClientSize.Width - 20 - optionPanel.Width);
            int imageHeight = (int)optionPanel.Height;
            if(Global.Recording.Frames != null && Global.CurrentFrame < Global.Recording.Frames.Count)
            {
                Mat? frameImage = Global.Recording.Frames[0].GetImage(this._cameraName);
                if(frameImage != null)
                {
                    double aspectRatio = (double)frameImage.Width / frameImage.Height;
                    imageHeight = (int)(imageWidth / aspectRatio);
                    Image image = ImageBuilder.CreateImage(imageWidth, imageHeight, 20 + (int)optionPanel.Width, 60, $"ANALYSIS_IMAGE");
                    image.PointerReleased += image_Clicked;
                    ANALYSIS_CANVAS.Children.Add(image);
                }                
            }  

            _updateImage();
        }

        private void image_Clicked(object? sender, PointerReleasedEventArgs e)
        {
            CalibrationMesh? _mesh = Global.Recording.Properties[_cameraName]?.CalibrationMesh;
            if (_mesh != null)
            {
                Image? image = (Image?)sender;
                if (image != null && image.Source != null)
                {
                    var point = e.GetPosition((Image?)sender);
                    _measurePoint = new CV.Point(point.X / image.Width * 100, point.Y / image.Height * 100);
                }               
            }
            _updateImage();
        }

        private void _updateImage()
        {
            Dispatcher.UIThread.Post(() =>
            {
                Image? image = UIControl.FindAvaloniaControl<Image>(ANALYSIS_CANVAS, $"ANALYSIS_IMAGE");
                if(Global.Recording.Frames != null && Global.CurrentFrame < Global.Recording.Frames.Count && image != null)
                {
                    Mat? frameImage = Global.Recording.Frames[Global.CurrentFrame].GetImage(this._cameraName);
                    if(frameImage != null)
                    {
                        CalibrationMesh _mesh = Global.Recording.Properties[this._cameraName].CalibrationMesh;
                        if (_mesh != null && _mesh.MeshPoints != null && _mesh.MeshPoints.Count > 0)
                        {
                            for (int r = 0; r < _mesh.MeshPoints[0].Count; r++)
                            {
                                Scalar color = r == 0 ? Scalar.Blue : Scalar.Red;
                                Cv2.Line(frameImage, new OpenCvSharp.Point(0, _mesh.MeshPoints[0][r].y * frameImage.Height / 100), _mesh.MeshPoints[0][r].ToCVPoint(frameImage.Width, frameImage.Height), color, 1, LineTypes.AntiAlias);
                                for (int c = 1; c < _mesh.MeshPoints.Count; c++)
                                {
                                    Cv2.Line(frameImage, _mesh.MeshPoints[c - 1][r].ToCVPoint(frameImage.Width, frameImage.Height), _mesh.MeshPoints[c][r].ToCVPoint(frameImage.Width, frameImage.Height), color, 1, LineTypes.AntiAlias);
                                }
                                Cv2.Line(frameImage, new OpenCvSharp.Point(frameImage.Width, _mesh.MeshPoints[_mesh.MeshPoints.Count - 1][r].y * frameImage.Height / 100), _mesh.MeshPoints[_mesh.MeshPoints.Count - 1][r].ToCVPoint(frameImage.Width, frameImage.Height), color, 1, LineTypes.AntiAlias);
                            }
                            if(_measurePoint != null)
                            {
                                Cv2.Line(frameImage, new CV.Point(0, _measurePoint.y * frameImage.Height / 100).ToCVPoint(), new CV.Point(frameImage.Width, _measurePoint.y * frameImage.Height / 100).ToCVPoint(), Scalar.Green, 1, LineTypes.AntiAlias, 0);
                                double measurement = Math.Round(_mesh.MeasureDistance(_measurePoint, Global.Recording.Properties[_cameraName]!.CalibrationCentimeters, Global.Recording.Properties[_cameraName].MeasurementOffsetCentimeters), 2);
                                TextBlock? distanceLabel = UIControl.FindAvaloniaControl<TextBlock>(ANALYSIS_CANVAS, "ANALYSIS_DISTANCE");
                                if(distanceLabel != null)
                                {
                                    distanceLabel.Text = $"DISTANCE: {measurement} cm";
                                }
                            } 
                        }
                                              
                        ImageBuilder.UpdateImage(image, frameImage);
                    }                
                }
                
            });
        }
    }
}