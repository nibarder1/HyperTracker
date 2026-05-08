using System.Collections.Generic;

namespace HyperTracker.CV
{
    public class CameraProperties
    {
        public string CameraName {get; set;}
        public int CameraIndex {get; set;}
        public string CameraDevicePath {get; set;}

        public double CalibrationCentimeters {get; set;}
        public double MeasurementOffsetCentimeters {get; set;}
        public CameraResolution Resolution {get; set;}
        public CalibrationMesh CalibrationMesh {get; set;}

        

        public CameraProperties(string CameraName,
                            string CameraDevicePath,
                            int CameraIndex,
                            double CalibrationCentimeters,
                            double MeasurementOffsetCentimeters,
                            CameraResolution Resolution,
                            CalibrationMesh CalibrationMesh)
        {
            this.CameraName = CameraName;
            this.CameraIndex = CameraIndex;
            this.CameraDevicePath = CameraDevicePath;
            this.Resolution = Resolution;
            this.CalibrationCentimeters = CalibrationCentimeters;
            this.MeasurementOffsetCentimeters = MeasurementOffsetCentimeters;
            this.CalibrationMesh = CalibrationMesh;
            if(this.CalibrationMesh == null) this.CalibrationMesh = new CalibrationMesh(Core.CalibrationType.SIMPLE, new List<List<Point>>());
        }

        public override string ToString()
        {
            return $"[{CameraIndex}] {Resolution.ToString()}";
        }
    }
}