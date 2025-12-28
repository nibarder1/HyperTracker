using System;
using System.Collections.Generic;
using SixLabors.ImageSharp;

namespace HyperTracker.Datatypes;

public class Frame
{
    private Dictionary<string, CameraFrame> _cameraFrames = new Dictionary<string, CameraFrame>();
    private Dictionary<string, TOFFrame> _tofFrames = new Dictionary<string, TOFFrame>();

    public readonly DateTime TimeStamp;

    public Frame(DateTime timestamp)
    {
        this.TimeStamp = timestamp;
    }

    public void AddImageToFrame(Image image, string SourceInput)
    {
        _cameraFrames.Add(SourceInput, new CameraFrame(image));
    }

    public Image? GetImage(string SourceInput)
    {
        if(!this._cameraFrames.ContainsKey(SourceInput))
        {
            return null;
        }
        return this._cameraFrames[SourceInput].GetValue();
    }

    public void AddTOFReading(double distance, string SourceInput)
    {
        _tofFrames.Add(SourceInput, new TOFFrame(distance));
    }
}