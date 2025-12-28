using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace HyperTracker.Windows.UIBuilders;

public class TextBlockBuilder
{
    public static TextBlock CreateTextBlock(int width, int height, int posX, int posY, string textBlockName, string textContent)
    {
        TextBlock text = new TextBlock();
        text.Name = textBlockName;
        Canvas.SetLeft(text, posX);
        Canvas.SetTop(text, posY);
        text.Text = textContent;
        text.FontSize = 16;
        return text;
    }

    public static void UpdateText(TextBlock textBlock, string textContent)
    {
        textBlock.Text = textContent; 
    }
}