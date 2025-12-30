using System.Collections.Generic;

namespace HyperTracker.Config;

public class Settings
{
    public string ProfileName = "DEFAULT";
    public TrackMode TrackingMode = TrackMode.VERTICLE_DISTANCE;
    public int TargetCycleMs = 5;
    public List<CameraSettings> Cameras = new List<CameraSettings>();

    public Settings(string ProfileName,
                    TrackMode TrackingMode,
                    int TargetCycleMs,
                    List<CameraSettings> Cameras)
    {
        this.ProfileName = ProfileName;
        this.TrackingMode = TrackingMode;
        this.TargetCycleMs = TargetCycleMs;
        this.Cameras = Cameras;
    }

}