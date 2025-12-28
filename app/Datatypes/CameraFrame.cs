using SixLabors.ImageSharp;

namespace HyperTracker.Datatypes;

public class CameraFrame : iFrameItem<Image>
{
    private Image _image;

    public CameraFrame(Image image)
    {
        this._image = image;
    }
    public Image GetValue()
    {
        return this._image;
    }
}