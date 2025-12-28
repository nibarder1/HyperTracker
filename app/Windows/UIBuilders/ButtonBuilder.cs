using Avalonia.Controls;
using Avalonia.Media;

namespace HyperTracker.Windows.UIBuilders;

public class ButtonBuilder
{
    public static Button CreateButton(int width, int height, int rootX, int rootY, string buttonName)
    {
        Button button = new Button();
        button.Name = buttonName;
        button.Width = width;
        button.Height = height;
        Canvas.SetLeft(button, rootX);
        Canvas.SetTop(button, rootY);

        return button;
    }
}