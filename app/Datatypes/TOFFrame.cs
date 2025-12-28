namespace HyperTracker.Datatypes;

public class TOFFrame : iFrameItem<double>
{
    private double _distance;

    public TOFFrame(double distance)
    {
        this._distance = distance;
    }
    public double GetValue()
    {
        return this._distance;
    }
}