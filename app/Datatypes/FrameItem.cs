using SixLabors.ImageSharp;

namespace HyperTracker.Datatypes;

public interface iFrameItem<T>
{
    public T GetValue();
}