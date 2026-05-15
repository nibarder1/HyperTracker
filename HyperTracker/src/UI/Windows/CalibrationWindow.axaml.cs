using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using HyperTracker.Core;
using HyperTracker.CV;
using HyperTracker.UI.UIBuilders;
using OpenCvSharp;

namespace HyperTracker.UI
{
    public partial class CalibrationWindow : Avalonia.Controls.Window
    {
        private Camera? _camera;
        private CalibrationMesh? _mesh;
        private int _workingColumn = 0;
        private int _workingRow = 0;

        public CalibrationWindow(Camera camera)
        {
            this._camera = camera;
            this.Title = "Camera Calibration";
            this.Topmost = true;
            this.SizeChanged += _resize;
            this.Closing += _onClose;
            this.MinHeight = 720;
            this.MinWidth = 1280;
            if (_camera != null && _camera.Properties != null)
            {
                this._mesh = _camera.Properties.CalibrationMesh.Clone();
            }
            else
            {
                this._mesh = new CalibrationMesh(CalibrationType.SIMPLE, new System.Collections.Generic.List<System.Collections.Generic.List<CV.Point>>());
            }
            InitializeComponent();
            _init();
            GlobalEvents.OnUpdateLive += _updateImage;
        }

        private void _onClose(object? sender, WindowClosingEventArgs e)
        {
            GlobalEvents.OnUpdateLive -= _updateImage;
        }

        private void _resize(object? sender, SizeChangedEventArgs e)
        {
            _init();
        }

        private void _init()
        {
            CALIBRATION_CANVAS.Children.Clear();
            Border controlPanel = CanvasBuilder.CreateCanvasWithBorder((int)this.ClientSize.Width, 50, 0, 0, $"CALIBRATION_CONTROL_PANEL");
            Button saveAndCloseButton = ButtonBuilder.CreateButton(150, 30, 10, 10, $"CALIBRATION_CLOSE_BUTTON", "SAVE AND CLOSE", _saveAndClose_Click);
            ComboBox calibrationModeSelector = ComboBoxBuilder.CreateComboBox(150, 30, 180, 10, $"CALIBRATION_MODE_SELECTOR");
            calibrationModeSelector.ItemsSource = Enum.GetValues(typeof(CalibrationType)).Cast<CalibrationType>().ToList();
            calibrationModeSelector.SelectedValue = _mesh!.CalibrationType;
            calibrationModeSelector.SelectionChanged += calibrationMode_Selected;

            CanvasBuilder.AddElement(controlPanel, saveAndCloseButton);
            CanvasBuilder.AddElement(controlPanel, calibrationModeSelector);

            Border optionPanel = CanvasBuilder.CreateCanvasWithBorder(300, (int)this.ClientSize.Height - 70, 10, 60, $"CALIBRATION_OPTION_PANEL");
            Button addColumnButton = ButtonBuilder.CreateButton(200, 30, 10, 10, $"ADD_COLUMN_BUTTON", "ADD COLUMN", _addColumn_Click);
            Button addRowButton = ButtonBuilder.CreateButton(200, 30, 10, 50, $"ADD_ROW_BUTTON", "ADD ROW", _addRow_Click);
            Border columnLabel = TextBlockBuilder.CreateTextBlockWithBox(150, 30, 10, 90, $"COLUMN_EDIT_LABEL", $"EDIT COLUMN");
            ComboBox columnSelector = ComboBoxBuilder.CreateComboBox(50, 30, 180, 90, $"COLUMN_EDIT_SELECTOR");
            List<string> columns = new List<string>();
            for (int i = 0; i < _mesh.MeshPoints.Count; i++)
            {
                columns.Add($"{i}");
            }
            columnSelector.ItemsSource = columns;
            columnSelector.SelectedIndex = _workingColumn;
            columnSelector.SelectionChanged += columnSelector_Selected;
            Button removeColumn = ButtonBuilder.CreateButton(30, 30, 260, 90, "REMOVE_COLUMN_BUTTON", "X", removeColumn_Click);

            Border rowLabel = TextBlockBuilder.CreateTextBlockWithBox(150, 30, 10, 130, $"ROW_EDIT_LABEL", $"EDIT ROW");
            ComboBox rowSelector = ComboBoxBuilder.CreateComboBox(50, 30, 180, 130, $"ROW_EDIT_SELECTOR");
            List<string> rows = new List<string>();
            if(_mesh != null && _mesh.MeshPoints != null && _mesh.MeshPoints.Count > 0)
            {
                for (int i = 0; i < _mesh.MeshPoints[0].Count; i++)
                {
                    rows.Add($"{i}");
                }
            }
            
            rowSelector.ItemsSource = rows;
            rowSelector.SelectedIndex = _workingRow;
            rowSelector.SelectionChanged += rowSelector_Selected;
            Button removeRow = ButtonBuilder.CreateButton(30, 30, 260, 130, "REMOVE_ROW_BUTTON", "X", removeRow_Click);

            Border shiftColumnLabel = TextBlockBuilder.CreateTextBlockWithBox(150, 30, 60, 170, $"SHIFT_COLUMN_LABEL", $"SHIFT COLUMN");
            Border shiftRowLabel = TextBlockBuilder.CreateTextBlockWithBox(150, 30, 60, 210, $"SHIFT_ROW_LABEL", $"SHIFT ROW");
            Button shiftColumnLeft = ButtonBuilder.CreateButton(30, 30, 10, 170, $"SHIFT_COLUMN_LEFT_BUTTON", "<", _shiftColumnLeft_Click);
            Button shiftColumnRight = ButtonBuilder.CreateButton(30, 30, 230, 170, $"SHIFT_COLUMN_RIGHT_BUTTON", ">", _shiftColumnRight_Click);
            Button shiftRowUp = ButtonBuilder.CreateButton(30, 30, 10, 210, $"SHIFT_ROW_UP_BUTTON", "<", _shiftRowUp_Click);
            Button shiftRowDown = ButtonBuilder.CreateButton(30, 30, 230, 210, $"SHIFT_ROW_DOWN_BUTTON", ">", _shiftRowDown_Click);

            CanvasBuilder.AddElement(optionPanel, addColumnButton);
            CanvasBuilder.AddElement(optionPanel, addRowButton);
            CanvasBuilder.AddElement(optionPanel, columnLabel);
            CanvasBuilder.AddElement(optionPanel, columnSelector);
            CanvasBuilder.AddElement(optionPanel, removeColumn);
            CanvasBuilder.AddElement(optionPanel, rowLabel);
            CanvasBuilder.AddElement(optionPanel, rowSelector);
            CanvasBuilder.AddElement(optionPanel, removeRow);
            CanvasBuilder.AddElement(optionPanel, shiftColumnLabel);
            CanvasBuilder.AddElement(optionPanel, shiftColumnLeft);
            CanvasBuilder.AddElement(optionPanel, shiftColumnRight);
            CanvasBuilder.AddElement(optionPanel, shiftRowLabel);
            CanvasBuilder.AddElement(optionPanel, shiftRowUp);
            CanvasBuilder.AddElement(optionPanel, shiftRowDown);


            int imageWidth = (int)(this.ClientSize.Width - 20 - optionPanel.Width);
            int imageHeight = (int)optionPanel.Height;
            if (_camera != null && _camera.Properties != null)
            {
                double aspectRatio = (double)_camera.Properties.Resolution.Width / _camera.Properties.Resolution.Height;
                imageHeight = (int)(imageWidth / aspectRatio);
            }
            Image image = ImageBuilder.CreateImage(imageWidth, imageHeight, 20 + (int)optionPanel.Width, 60, $"CALIBRATION_IMAGE");
            image.PointerReleased += image_Clicked;

            CALIBRATION_CANVAS.Children.Add(controlPanel);
            CALIBRATION_CANVAS.Children.Add(optionPanel);
            CALIBRATION_CANVAS.Children.Add(image);
            _updateImage();
        }

        private void removeColumn_Click(object? sender, RoutedEventArgs e)
        {
            if(_mesh != null)
            {
                _mesh.RemoveColumn(_workingColumn);
                _updateColumnSelector();
                _updateImage();
            }
        }

        private void removeRow_Click(object? sender, RoutedEventArgs e)
        {
            if(_mesh != null)
            {
                _mesh.RemoveRow(_workingRow);
                _updateRowSelector();
                _updateImage();
            }
        }

        private void image_Clicked(object? sender, PointerReleasedEventArgs e)
        {
            if (_mesh != null)
            {
                Image? image = (Image?)sender;
                if (image != null && image.Source != null)
                {
                    var point = e.GetPosition((Image?)sender);
                    _mesh.UpdateMeshPoint(_workingRow, _workingColumn, new CV.Point(point.X / image.Width * 100, point.Y / image.Height * 100));
                }
            }
            _updateImage();
        }

        private void columnSelector_Selected(object? sender, SelectionChangedEventArgs e)
        {
            ComboBox? selector = (ComboBox?)sender;
            if (selector != null)
            {
                _workingColumn = selector.SelectedIndex;
            }
        }

        private void rowSelector_Selected(object? sender, SelectionChangedEventArgs e)
        {
            ComboBox? selector = (ComboBox?)sender;
            if (selector != null)
            {
                _workingRow = selector.SelectedIndex;
            }
        }

        private void calibrationMode_Selected(object? sender, SelectionChangedEventArgs e)
        {
            ComboBox? selector = (ComboBox?)sender;
            if (selector != null && selector.SelectedValue != null && !String.IsNullOrWhiteSpace(selector.SelectedValue.ToString()))
            {
                _mesh!.CalibrationType = Enum.Parse<CalibrationType>(selector.SelectedValue.ToString()!);
                _mesh.Reset();
            }
            _updateImage();
        }

        private void _shiftRowDown_Click(object? sender, RoutedEventArgs e)
        {
            if (_mesh != null)
            {
                _mesh.ShiftRow(_workingRow, 0.1);
            }
            _updateImage();
        }

        private void _shiftRowUp_Click(object? sender, RoutedEventArgs e)
        {
            if (_mesh != null)
            {
                _mesh.ShiftRow(_workingRow, -0.1);
            }
            _updateImage();
        }

        private void _shiftColumnRight_Click(object? sender, RoutedEventArgs e)
        {
            if (_mesh != null)
            {
                _mesh.ShiftColumn(_workingRow, 0.1);
            }
            _updateImage();
        }

        private void _shiftColumnLeft_Click(object? sender, RoutedEventArgs e)
        {
            if (_mesh != null)
            {
                _mesh.ShiftColumn(_workingRow, -0.1);
            }
            _updateImage();
        }

        private void _addRow_Click(object? sender, RoutedEventArgs e)
        {
            if (_mesh != null)
            {
                _mesh.AddRow();
                _updateRowSelector();
            }
            _updateImage();
        }

        private void _addColumn_Click(object? sender, RoutedEventArgs e)
        {
            if (_mesh != null)
            {
                _mesh.AddColumn();
                _updateColumnSelector();
            }
            
            _updateImage();
        }

        private void _updateRowSelector()
        {
            if (_mesh != null)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    ComboBox? selector = UIControl.FindAvaloniaControl<ComboBox>(CALIBRATION_CANVAS, $"ROW_EDIT_SELECTOR");
                    if (selector != null)
                    {
                        List<string> rows = new List<string>();
                        for (int i = 0; i < _mesh.MeshPoints[0].Count; i++)
                        {
                            rows.Add($"{i}");
                        }
                        selector.ItemsSource = rows;
                        if(_workingRow > rows.Count - 1)
                        {
                            _workingRow = rows.Count - 1;
                        }
                        selector.SelectedIndex = _workingRow;
                    }
                });
            }
        }

        private void _updateColumnSelector()
        {
            if (_mesh != null)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    ComboBox? selector = UIControl.FindAvaloniaControl<ComboBox>(CALIBRATION_CANVAS, $"COLUMN_EDIT_SELECTOR");
                    if (selector != null)
                    {
                        List<string> columns = new List<string>();
                        for (int i = 0; i < _mesh.MeshPoints[0].Count; i++)
                        {
                            columns.Add($"{i}");
                        }
                        selector.ItemsSource = columns;
                        if(_workingColumn > columns.Count - 1)
                        {
                            _workingColumn = columns.Count - 1;
                        }
                        selector.SelectedIndex = _workingColumn;
                    }
                });
            }
        }

        private void _saveAndClose_Click(object? sender, RoutedEventArgs e)
        {
            if (_camera != null && _camera.Properties != null && _mesh != null)
            {
                _camera.Properties.CalibrationMesh = _mesh.Clone();
            }
            this.Close();
        }

        private void _updateImage()
        {
            Dispatcher.UIThread.Post(() =>
            {
                Image? image = UIControl.FindAvaloniaControl<Image>(CALIBRATION_CANVAS, $"CALIBRATION_IMAGE");
                if (image != null && _camera != null && _camera.Frame != null)
                {
                    Mat frame = _camera.Frame.Clone();
                    if (_mesh != null && _mesh.MeshPoints != null && _mesh.MeshPoints.Count > 0)
                    {
                        for (int r = 0; r < _mesh.MeshPoints[0].Count; r++)
                        {
                            Scalar color = r == 0 ? Scalar.Blue : Scalar.Red;
                            Cv2.Line(frame, new OpenCvSharp.Point(0, _mesh.MeshPoints[0][r].y * frame.Height / 100), _mesh.MeshPoints[0][r].ToCVPoint(frame.Width, frame.Height), color, 1, LineTypes.AntiAlias);
                            for (int c = 1; c < _mesh.MeshPoints.Count; c++)
                            {
                                Cv2.Line(frame, _mesh.MeshPoints[c - 1][r].ToCVPoint(frame.Width, frame.Height), _mesh.MeshPoints[c][r].ToCVPoint(frame.Width, frame.Height), color, 1, LineTypes.AntiAlias);
                            }
                            Cv2.Line(frame, new OpenCvSharp.Point(frame.Width, _mesh.MeshPoints[_mesh.MeshPoints.Count - 1][r].y * frame.Height / 100), _mesh.MeshPoints[_mesh.MeshPoints.Count - 1][r].ToCVPoint(frame.Width, frame.Height), color, 1, LineTypes.AntiAlias);
                        }

                    }

                    ImageBuilder.UpdateImage(image, frame);
                }
            });

        }
    }
}