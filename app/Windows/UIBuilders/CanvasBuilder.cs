using Avalonia.Controls;
using Avalonia.Media;

namespace HyperTracker.Windows.UIBuilders;

public class CanvasBuilder
{
    public static Canvas CreateCanvas(int width, int height, int rootX, int rootY, string canvasName)
    {
        Canvas canvas = new Canvas();
        canvas.Name = canvasName;
        canvas.Width = width;
        canvas.Height = height;
        canvas.Background = new SolidColorBrush(Avalonia.Media.Color.FromRgb(0,0,0));
        Canvas.SetLeft(canvas, rootX);
        Canvas.SetTop(canvas, rootY);

        return canvas;
    }
}