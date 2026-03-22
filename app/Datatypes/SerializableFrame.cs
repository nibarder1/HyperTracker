using System;
using System.Collections.Generic;

namespace HyperTracker.Datatypes
{
    public class SerializableFrame
    {
        public DateTime TimeStamp { get; set; }
        public Dictionary<string, string> CameraFramePaths { get; set; } = new();
        public Dictionary<string, double> TOFReadings { get; set; } = new();
    }
}