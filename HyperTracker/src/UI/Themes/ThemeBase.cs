using Avalonia;
using Avalonia.Animation;
using Avalonia.Media;

namespace HyperTracker.UI.Themes
{
    public class ThemeBase
    {
        public string ThemeName = "Basic";

        public Brush PrimaryBackgroundBrush = new SolidColorBrush(Brushes.Black.Color);
        public Brush PrimaryForegroundBrush = new SolidColorBrush(Brushes.White.Color);

        public Brush RecordingBrush = new SolidColorBrush(Brushes.Green.Color);
        public Brush IdleBrush = new SolidColorBrush(Brushes.Red.Color);
        public Brush SavingBrush = new SolidColorBrush(Brushes.Yellow.Color);

        public Brush BorderEdgeBrush = new SolidColorBrush(Brushes.White.Color);
        public Thickness BorderEdgeThickness = new Thickness(1);
        public Brush BorderBackgroundBrush = new SolidColorBrush(Brushes.Gray.Color);

        public Brush ButtonBackgroundBrush = new SolidColorBrush(Brushes.Black.Color);
        public Brush ButtonForegroundBrush = new SolidColorBrush(Brushes.White.Color);

        public Brush CanvasBackgroundBrush = new SolidColorBrush(Brushes.Gray.Color);

        public Brush ComboBoxBackgroundBrush = new SolidColorBrush(Brushes.Black.Color);
        public Brush ComboBoxForegroundBrush = new SolidColorBrush(Brushes.White.Color);

        public Brush TabControlBackgroundBrush = new SolidColorBrush(Brushes.Gray.Color);
        public Brush TabControlSelectedBrush = new SolidColorBrush(Brushes.Black.Color);
        public Brush TabControlSelectedTextBrush = new SolidColorBrush(Brushes.White.Color);
        public double TabControlHeaderHeight = 40;
    }
}