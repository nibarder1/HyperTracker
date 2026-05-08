using System;
using System.Collections.Generic;
using HyperTracker.CV;
using OpenCvSharp;

namespace HyperTracker.Core
{
    public class Frame
    {
        /// <summary>
        /// Frame timestamp.
        /// </summary>
        public DateTime Timestamp {get; set;}
        /// <summary>
        /// Frame images.
        /// </summary>
        public Dictionary<string, Mat?> FrameImages {get; set;}
        public Frame(DateTime timestamp)
        {
            this.Timestamp = timestamp;
            this.FrameImages = new Dictionary<string, Mat?>();
        }

        /// <summary>
        /// Add image to the frame.
        /// </summary>
        /// <param name="camera">Name of the camera.</param>
        /// <param name="image">Camera image.</param>
        public void AddFrame(string camera, Mat? image)
        {
            this.FrameImages.Add(camera, image);
        }

        /// <summary>
        /// Get image.
        /// </summary>
        /// <param name="camera">Camera name.</param>
        /// <returns>OpenCVSharp image if exists.</returns>
        public Mat? GetImage(string camera)
        {
            if(!FrameImages.ContainsKey(camera))
            {
                return null;
            }
            return FrameImages[camera]?.Clone();
        }

        /// <summary>
        /// Capture a frame.
        /// </summary>
        /// <returns>Captured frame.</returns>
        public static void CaptureFrame()
        {
            try
            {
                Frame frame = new Frame(DateTime.UtcNow);
                foreach(Camera camera in Global.Cameras)
                {
                    frame.AddFrame($"{camera.CameraName}", camera.Frame);
                }
                Global.Recording.Frames.Add(frame);
                if(Global.Config != null)
                {
                    int maxFrames = 1000 * Global.Config.RecordingTime / Global.Config.RecordingCycleMs;
                    if(Global.Recording.Frames.Count > maxFrames)
                    {
                        Global.Recording.Frames.RemoveAt(0);
                    }
                }
            }catch
            {
                
            }
            
        }
    }
}