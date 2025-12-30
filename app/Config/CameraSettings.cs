namespace HyperTracker.Config;

public class CameraSettings
{
    public bool Enabled = true;
    public string CameraName = "";
    public int CameraIndex = 0;
    public int CameraWidth = 1280;
    public int CameraHeight = 720;
    public int FPS = 120;
    public double MEASURE_FLOOR_POSITION = 100;
    public double PIXELS_PER_MM = 10;
    public double MEASUREMENT_OFFSET_MM = 0;

    public CameraSettings(bool Enabled,
                            string CameraName,
                            int CameraIndex,
                            int CameraWidth,
                            int CameraHeight,
                            int FPS,
                            double MEASURE_FLOOR_POSITION,
                            double PIXELS_PER_MM,
                            double MEASUREMENT_OFFSET_MM)
    {
        this.Enabled = Enabled;
        this.CameraName = CameraName;
        this.CameraIndex = CameraIndex;
        this.CameraWidth = CameraWidth;
        this.CameraHeight = CameraHeight;
        this.FPS = FPS;
        this.MEASURE_FLOOR_POSITION = MEASURE_FLOOR_POSITION;
        this.PIXELS_PER_MM = PIXELS_PER_MM;
        this.MEASUREMENT_OFFSET_MM = MEASUREMENT_OFFSET_MM;
    }

}