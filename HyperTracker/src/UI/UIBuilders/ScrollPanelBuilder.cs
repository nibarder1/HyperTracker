using Avalonia.Controls;

namespace HyperTracker.UI.UIBuilders
{
    public class ScrollPanelBuilder
    {
        public static Border CreateScrollPanelWithBorder(int width, int height, int rootX, int rootY, string elementName)
        {
            Border border = BorderBuilder.CreateBorder(width, height, rootX, rootY, $"{elementName}_BORDER");
            ScrollViewer panel = new ScrollViewer();
            panel.Name = elementName;
            panel.Width = width;
            panel.Height = height;
            panel.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
            Canvas.SetLeft(panel, 0);
            Canvas.SetTop(panel, 0);

            border.Child = panel;
            return border;
        }

        public static void AddElement(Border control, Control content)
        {
            if(control.Child == null)
            {
                return;
            }
            ScrollViewer? panel = control.Child as ScrollViewer;
            if(panel != null)
            {
                panel.Content = content;
            }
        }
    }
}