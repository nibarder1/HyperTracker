using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace HyperTracker.UI.UIBuilders;

public class ButtonBuilder
{
    public static Button CreateButton(int width = 200, int height = 30, int rootX = 0, int rootY = 0, string buttonName = "BUTTON", string buttonText = "SUBMIT", EventHandler<RoutedEventArgs>? onClick = null)
    {
        Button button = new Button();
        button.Name = buttonName;
        button.Content = buttonText;
        button.Width = width;
        button.Height = height;
        button.Background = Global.Theme.ButtonBackgroundBrush;
        button.Foreground = Global.Theme.ButtonForegroundBrush;
        button.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center;
        if(onClick != null)
        {
            button.Click += onClick;
        }
        Canvas.SetLeft(button, rootX);
        Canvas.SetTop(button, rootY);

        return button;
    }

    public static Border CreateButtonWithBorder(int width = 200, int height = 30, int rootX = 0, int rootY = 0, string buttonName = "BUTTON", string buttonText = "SUBMIT", EventHandler<RoutedEventArgs>? onClick = null)
    {
        Border border = BorderBuilder.CreateBorder(width, height, rootX, rootY, $"{buttonName}_BORDER");
        Button button = CreateButton(width, height, 0, 0, buttonName, buttonText, onClick);
        border.Child = button;
        return border;
    }
}