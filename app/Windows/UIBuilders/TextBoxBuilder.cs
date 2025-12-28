using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace HyperTracker.Windows.UIBuilders;

public class TextBoxBuilder
{
    public static TextBox CreateTextBox(int width, int height, int posX, int posY, string textBoxName, bool multiLine)
    {
        TextBox textBox = new TextBox();
        textBox.Name = textBoxName;
        textBox.Width = width;
        textBox.Height = height;
        textBox.AcceptsReturn = multiLine;
        Canvas.SetLeft(textBox, posX);
        Canvas.SetTop(textBox, posY);
        return textBox;
    }

    public static void UpdateTextBox(TextBox textBox, string value)
    {
        textBox.Text = value;
    }
}