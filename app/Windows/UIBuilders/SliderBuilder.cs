using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace HyperTracker.Windows.UIBuilders;

public class SliderBuilder
{
    public static Slider CreateSlider(int width, int height, int posX, int posY, string sliderName)
    {
        Slider slider = new Slider();
        slider.Name = sliderName;
        slider.Width = width;
        slider.Height = height;
        Canvas.SetLeft(slider, posX);
        Canvas.SetTop(slider, posY);
        return slider;
    }

    public static void UpdateSlider(Slider slider, int min, int max, int value)
    {
        slider.Minimum = min;
        slider.Maximum = max;
        slider.Value = value;
    }
}