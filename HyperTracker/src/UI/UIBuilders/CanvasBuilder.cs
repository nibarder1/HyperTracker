using Avalonia.Controls;
using Avalonia.Media;

namespace HyperTracker.UI.UIBuilders;

public class CanvasBuilder
{
    public static Canvas CreateCanvas(int width, int height, int rootX, int rootY, string canvasName)
    {
        Canvas canvas = new Canvas();
        canvas.Name = canvasName;
        canvas.Width = width;
        canvas.Height = height;
        canvas.Background = Global.Theme.CanvasBackgroundBrush;
        Canvas.SetLeft(canvas, rootX);
        Canvas.SetTop(canvas, rootY);

        return canvas;
    }

    public static Border CreateCanvasWithBorder(int width, int height, int rootX, int rootY, string canvasName)
    {
        Border border = BorderBuilder.CreateBorder(width, height, rootX, rootY, $"{canvasName}_BORDER");
        Canvas canvas = new Canvas();
        canvas.Name = canvasName;
        canvas.Width = width;
        canvas.Height = height;
        Canvas.SetLeft(canvas, 0);
        Canvas.SetTop(canvas, 0);
        border.Child = canvas;
        return border;
    }

    public static void AddElement(Border control, Control content)
    {
        if(control.Child == null)
        {
            return;
        }
        Canvas? panel = control.Child as Canvas;
        panel?.Children.Add(content);
    }
}