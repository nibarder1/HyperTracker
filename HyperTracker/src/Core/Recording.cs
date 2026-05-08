using System.Collections.Generic;
using HyperTracker.CV;

namespace HyperTracker.Core
{
    public class Recording
    {
        public Dictionary<string, CameraProperties> Properties = new Dictionary<string, CameraProperties>();
        public List<Frame> Frames = new List<Frame>();
    }
}