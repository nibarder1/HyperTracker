using System;
using System.Collections.Generic;
using System.Linq;
using HyperTracker.Core;

namespace HyperTracker.CV
{
    public class CalibrationMesh
    {
        public CalibrationType CalibrationType {get; set;}
        public List<List<Point>> MeshPoints {get; set;}

        public CalibrationMesh(CalibrationType CalibrationType,
                                List<List<Point>> MeshPoints)
        {
            this.CalibrationType = CalibrationType;
            this.MeshPoints = MeshPoints;
        }

        public void UpdateMeshPoint(int rowIndex, int columnIndex, Point newPoint)
        {
            if(CalibrationType == CalibrationType.SIMPLE)
            {
                if(rowIndex == 0 && MeshPoints[0][1].y < newPoint.y)
                {
                    MeshPoints[0][0] = newPoint;
                }
                if(rowIndex == 1 && MeshPoints[0][0].y > newPoint.y)
                {
                    MeshPoints[0][1] = newPoint;
                }
            }
            if(CalibrationType == CalibrationType.LINE)
            {
                if(rowIndex == 0 && MeshPoints[0].Count > 1 && MeshPoints[0][1].y >= newPoint.y) return;
                if(rowIndex == MeshPoints[0].Count - 1 && MeshPoints[0].Count > 1 && MeshPoints[0][rowIndex - 1].y <= newPoint.y) return;
                if(rowIndex != 0 && rowIndex != MeshPoints[0].Count -1 && (MeshPoints[0][rowIndex - 1].y <= newPoint.y || MeshPoints[0][rowIndex + 1].y >= newPoint.y)) return;


                MeshPoints[0][rowIndex] = newPoint;
            }
        }

        public void Reset()
        {
            MeshPoints.Clear();
            AddColumn();
        }

        public void ShiftRow(int rowIndex, double amount)
        {
            if(_canShiftRow(rowIndex, amount))
            {
                foreach(var column in MeshPoints)
                {
                    column[rowIndex].y = column[rowIndex].y + amount;
                }
            }
        }

        public void ShiftColumn(int columnIndex, double amount)
        {
            if(_canShiftColumn(columnIndex, amount))
            {
                foreach(Point point in MeshPoints[columnIndex])
                {
                    point.x = point.x + amount;
                }
            }
        }

        private bool _canShiftColumn(int index, double amount)
        {
            if(index >= MeshPoints.Count || MeshPoints[index].Count == 0)
            {
                return false;
            }
            if(MeshPoints[index][0].x + amount >= 100 || MeshPoints[index][0].x + amount <= 0) return false;
            if(MeshPoints.Count > 1 && index == 0 && MeshPoints[0][0].x + amount <= MeshPoints[1][0].x) return false;
            if(MeshPoints.Count > 1 && index == MeshPoints.Count - 1 && MeshPoints[index][0].x + amount >= MeshPoints[index - 1][0].x) return false;
            if(MeshPoints.Count > 1 && index != 0 && index != MeshPoints.Count - 1 && (MeshPoints[index][0].x + amount >= MeshPoints[index - 1][0].x || MeshPoints[index][0].x + amount <= MeshPoints[index + 1][0].x)) return false;
            return true;
        }

        private bool _canShiftRow(int index, double amount)
        {
            foreach(var column in MeshPoints)
            {
                if(index > column.Count) return false;
                if(column[index].y + amount >= 100 || column[index].y + amount <= 0) return false;
                if(column.Count > 1 && index == 0 && column[0].y + amount <= column[1].y) return false;
                if(column.Count > 1 && index == column.Count - 1 && column[index].y + amount >= column[index - 1].y) return false;
                if(column.Count > 1 && index != 0 && index != column.Count - 1 && (column[index].y + amount <= column[index + 1].y || column[index].y + amount >= column[index - 1].y)) return false;
            }
            return true;
        }

        public void AddColumn()
        {
            if(!_canAddColumn())
            {
                return;
            }
            List<Point> newColumn = new List<Point>();
            if(MeshPoints.Count == 0)
            {
                MeshPoints.Add(newColumn);
                AddRow();
                AddRow();
                return;
            }
            var columnSplit = 100 / (MeshPoints.Count + 2);
            for(int c = 0; c < MeshPoints.Count; c++)
            {
                var column = MeshPoints[c];
                for(int row = 0; row < column.Count; row++)
                {
                    column[row].x = 100 - (c + 1) * columnSplit;
                }
            }  
            foreach(Point point in MeshPoints[MeshPoints.Count - 1])
            {
                newColumn.Add(new Point(point.x + columnSplit, point.y));
            }
            MeshPoints.Add(newColumn);
        }

        public void AddRow()
        {
            if(!_canAddRow())
            {
                return;
            }
            var rowSplit = 100 / (MeshPoints[0].Count + 2);
            for(int c = 0; c < MeshPoints.Count; c++)
            {
                var column = MeshPoints[c];
                for(int row = 0; row < column.Count; row++)
                {
                    column[row].y = 100 - (row + 1) * rowSplit;
                }
                column.Add(new Point(column.Count > 0? column[c].x : 50, rowSplit));
            }            
        }

        public void RemoveColumn(int index)
        {
            if(index < MeshPoints.Count && MeshPoints.Count > 1)
            {
                MeshPoints.RemoveAt(index);
            }
        }

        public void RemoveRow(int index)
        {
            foreach(var column in MeshPoints)
            {
                if(index < column.Count && column.Count > 2)
                {
                    column.RemoveAt(index);
                }
            }
        }

        private bool _canAddColumn()
        {
            if(CalibrationType == CalibrationType.SIMPLE && MeshPoints.Count == 0)
            {
                return true;
            }
            if(CalibrationType == CalibrationType.LINE && MeshPoints.Count == 0)
            {
                return true;
            }
            return false;
        }

        private bool _canAddRow()
        {
            if(CalibrationType == CalibrationType.SIMPLE && MeshPoints.Count == 1 && MeshPoints[0].Count < 2)
            {
                return true;
            }
            if(CalibrationType == CalibrationType.LINE && MeshPoints.Count == 1)
            {
                return true;
            }
            return false;
        }

        public double MeasureDistance(Point testPoint, double calibration, double offset)
        {
            if(CalibrationType == CalibrationType.SIMPLE)
            {
                return _measureSimple(testPoint, calibration, offset);
            }
            if(CalibrationType == CalibrationType.LINE)
            {
                return _measureLine(testPoint, calibration, offset);
            }
            return -1;
        }

        private double _measureSimple(Point testPoint, double calibration, double offset)
        {
            if(MeshPoints.Count == 1 && MeshPoints[0].Count > 1)
            {
                double pixelPerCM = Math.Abs(MeshPoints[0][0].y - MeshPoints[0][1].y) / calibration;
                if(pixelPerCM <= 0 ) pixelPerCM = 1;
                double measurePixels = MeshPoints[0][0].y - testPoint.y;
                return measurePixels / pixelPerCM + offset;
            }
            return -1;
        }

        private double _measureLine(Point testPoint, double calibration, double offset)
        {
            if(MeshPoints.Count == 1 && MeshPoints[0].Count > 1)
            {
                if(testPoint.y < MeshPoints[0][0].y)
                {
                    return _measureSimple(testPoint, calibration, offset);
                }

                double trueOffset = offset + (MeshPoints[0].Count - 1) * calibration;
                double pixelPerCM = _getPixelPerCM(MeshPoints[0][MeshPoints[0].Count - 2], MeshPoints[0][MeshPoints[0].Count - 1], calibration);
                double measurePixels = MeshPoints[0][MeshPoints[0].Count - 1].y - testPoint.y;
                for(int i = 0; i < MeshPoints[0].Count - 1; i++)
                {
                    if(testPoint.y <= MeshPoints[0][i].y && testPoint.y > MeshPoints[0][i+1].y)
                    {
                        trueOffset = offset + i * calibration;
                        pixelPerCM = _getPixelPerCM(MeshPoints[0][i], MeshPoints[0][i+1], calibration);
                        measurePixels = MeshPoints[0][i].y - testPoint.y;
                    }
                } 
                return measurePixels / pixelPerCM + trueOffset;
            }
            return -1;
        }

        private double _getPixelPerCM(Point first, Point second, double calibration)
        {
            double pixelPerCM = Math.Abs(first.y - second.y) / calibration;
            if(pixelPerCM <= 0 ) pixelPerCM = 1;
            return pixelPerCM;
        }

        public CalibrationMesh Clone()
        {
            List<Point>[] meshCopy = new List<Point>[MeshPoints.Count];
            this.MeshPoints.CopyTo(meshCopy);
            return new CalibrationMesh(this.CalibrationType, meshCopy.ToList());
        }
    }
}