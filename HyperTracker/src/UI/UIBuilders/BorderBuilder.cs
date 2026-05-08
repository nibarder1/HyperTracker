using Avalonia.Controls;

namespace HyperTracker.UI.UIBuilders
{
    public class BorderBuilder
    {
        public static Border CreateBorder(int width, int height, int rootX, int rootY, string elementName)
        {
            Border border = new Border();
            border.Name = elementName;
            border.Width = width;
            border.Height = height;
            Canvas.SetLeft(border, rootX);
            Canvas.SetTop(border, rootY);
            border.BorderBrush = Global.Theme.BorderEdgeBrush;
            border.BorderThickness = Global.Theme.BorderEdgeThickness;
            border.Background = Global.Theme.BorderBackgroundBrush;
            return border;
        }
    }
}