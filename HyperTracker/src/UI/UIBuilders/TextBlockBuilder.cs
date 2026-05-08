using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace HyperTracker.UI.UIBuilders;

public class TextBlockBuilder
{
    public static TextBlock CreateTextBlock(int width, int height, int posX, int posY, string textBlockName, string textContent)
    {
        TextBlock text = new TextBlock();
        text.Name = textBlockName;
        text.Foreground = Global.Theme.PrimaryForegroundBrush;
        text.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
        Canvas.SetLeft(text, posX);
        Canvas.SetTop(text, posY);
        text.Text = textContent;
        text.FontSize = 16;
        return text;
    }

    public static Border CreateTextBlockWithBox(int width, int height, int posX, int posY, string textBlockName, string textContent)
    {
        Border border = BorderBuilder.CreateBorder(width, height, posX, posY, $"{textBlockName}_BORDER");
        border.Background = Global.Theme.PrimaryBackgroundBrush;
        border.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
        border.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
        TextBlock text = CreateTextBlock(width, height, 0, 0, textBlockName, textContent);
        text.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
        text.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
        text.TextAlignment = TextAlignment.Center;
        border.Child = text;
        return border;
    }

    public static void UpdateText(TextBlock textBlock, string textContent)
    {
        textBlock.Text = textContent; 
    }
}