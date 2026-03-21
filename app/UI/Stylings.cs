using Avalonia.Media;

namespace HyperTracker.UI
{
    public class Stylings
    {

        public static Color GREEN = new Color(255, 0, 150, 0);

        public static Color RED = new Color(255, 150, 0, 0);

        public static Color BLACK = new Color(255, 0, 0, 0);

        public static Color YELLOW = new Color(255, 255, 238, 0);


        public static  LinearGradientBrush CreateGradient(Color startColor, Color endColor)
        {
            LinearGradientBrush gradientBrush = new LinearGradientBrush();
            gradientBrush.StartPoint = new Avalonia.RelativePoint(0,0, Avalonia.RelativeUnit.Relative);
            gradientBrush.EndPoint = new Avalonia.RelativePoint(0,1, Avalonia.RelativeUnit.Relative);

            gradientBrush.GradientStops.Add(new GradientStop(startColor, 0.0));
            gradientBrush.GradientStops.Add(new GradientStop(endColor, 1));

            return gradientBrush;
        }
    }
}