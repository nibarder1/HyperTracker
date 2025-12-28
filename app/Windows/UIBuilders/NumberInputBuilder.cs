using Avalonia.Controls;

namespace HyperTracker.Windows.UIBuilders;

public class NumberInputBuilder
{
    public static NumericUpDown CreateNumberInput(int width, int height, int posX, int posY, string numberInputName)
    {
        NumericUpDown numberInput = new NumericUpDown();
        numberInput.Name = numberInputName;
        numberInput.Width = width;
        numberInput.Height = height;
        Canvas.SetLeft(numberInput, posX);
        Canvas.SetTop(numberInput, posY);
        return numberInput;
    }
}