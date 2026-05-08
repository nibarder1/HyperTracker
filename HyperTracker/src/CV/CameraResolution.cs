namespace HyperTracker.CV
{
    public class CameraResolution
    {
        public int Width {get; set;}
        public int Height {get; set;}
        public int FrameRate {get; set;}

        public CameraResolution(int Width,
                                int Height,
                                int FrameRate)
        {
            this.Width = Width;
            this.Height = Height;
            this.FrameRate = FrameRate;
        }

        public override string ToString()
        {
            return $"{Width}x{Height} @{FrameRate} FPS";
        }
    }
}