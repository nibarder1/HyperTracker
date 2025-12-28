using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace HyperTracker.Windows.UIBuilders;

public class ComboBoxBuilder
{
    public static ComboBox CreateComboBox(int width, int height, int posX, int posY, string comboBoxName)
    {
        ComboBox comboBox = new ComboBox();
        comboBox.Name = comboBoxName;
        Canvas.SetLeft(comboBox, posX);
        Canvas.SetTop(comboBox, posY);
        return comboBox;
    }

    public static void UpdateComboBox(ComboBox comboBox, List<string> items)
    {
        comboBox.ItemsSource = items; 
    }
}