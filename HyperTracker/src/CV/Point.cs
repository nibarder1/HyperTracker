namespace HyperTracker.CV
{
    public class Point
    {
        public double x {get; set;}
        public double y {get; set;}

        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public OpenCvSharp.Point ToCVPoint(double scaleX = 100, double scaleY = 100)
        {
            return new OpenCvSharp.Point(x * scaleX / 100, y * scaleY / 100);
        }
    }
}