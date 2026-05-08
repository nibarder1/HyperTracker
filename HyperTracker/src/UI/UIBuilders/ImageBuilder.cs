using System;
using System.IO;
using System.Runtime.ConstrainedExecution;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using OpenCvSharp;

namespace HyperTracker.UI.UIBuilders;

public class ImageBuilder
{
    public static Avalonia.Controls.Image CreateImage(int width, int height, int posX, int posY, string imageName)
    {
        Avalonia.Controls.Image image = new Avalonia.Controls.Image();
        image.Name = imageName;
        image.Width = width;
        image.Height = height;
        image.Stretch = Stretch.Uniform;   
             
        Canvas.SetLeft(image, posX);
        Canvas.SetTop(image, posY);
        return image;
    }

    public static void UpdateImage(Avalonia.Controls.Image image, Mat source)
    {
        try
        {           
            Cv2.ImEncode(".png", source, out var buffer);
            MemoryStream stream = new MemoryStream(buffer);
            var bitmap = new Bitmap(stream);
            image.Source = bitmap; 
        }catch{}
        
    }
}