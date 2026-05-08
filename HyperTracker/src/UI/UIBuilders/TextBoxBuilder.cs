using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;

namespace HyperTracker.UI.UIBuilders;

public class TextBoxBuilder
{
    public static TextBox CreateTextBox(int width, int height, int posX, int posY, string textBoxName, bool multiLine)
    {
        TextBox textBox = new TextBox();
        textBox.Name = textBoxName;
        textBox.Width = width;
        textBox.Height = height;
        textBox.Background = Global.Theme.PrimaryBackgroundBrush;
        textBox.Foreground = Global.Theme.PrimaryForegroundBrush;
        textBox.CaretBrush = Global.Theme.PrimaryForegroundBrush;
        var editStyle = new Style(s => s.OfType<TextBox>().Class(":focus").Template().OfType<Border>().Name("PART_BorderElement"));
        editStyle.Setters.Add(new Setter(Border.BackgroundProperty, Global.Theme.PrimaryBackgroundBrush));

        // Apply to a specific TextBox instance
        textBox.Styles.Add(editStyle);
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