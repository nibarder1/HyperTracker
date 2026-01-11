using System;
using System.IO;
using System.Runtime.ConstrainedExecution;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace HyperTracker.Windows.UIBuilders;

public class ImageBuilder
{
    public static Avalonia.Controls.Image CreateImage(int width, int height, int posX, int posY, string imageName)
    {
        Avalonia.Controls.Image image = new Avalonia.Controls.Image();
        image.Name = imageName;
        image.Width = width;
        image.Height = height;
        Canvas.SetLeft(image, posX);
        Canvas.SetTop(image, posY);
        return image;
    }

    public static void UpdateImage(Avalonia.Controls.Image image, SixLabors.ImageSharp.Image source)
    {
        using var ms = new MemoryStream();
        //source.Save(ms, new PngEncoder());
        source.SaveAsBmp(ms);
        ms.Position = 0;
        var bitmap = new Bitmap(ms);
        image.Source = bitmap.CreateScaledBitmap(new Avalonia.PixelSize(1280, 720)); 
    }
}