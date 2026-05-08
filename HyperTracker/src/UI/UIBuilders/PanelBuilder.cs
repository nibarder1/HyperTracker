using Avalonia.Controls;

namespace HyperTracker.UI.UIBuilders
{
    public class PanelBuilder
    {
        public static Border CreateStackPanelWithBorder(int width, int height, int rootX, int rootY, string elementName)
        {
            Border border = BorderBuilder.CreateBorder(width, height, rootX, rootY, $"{elementName}_BORDER");
            StackPanel panel = new StackPanel();
            panel.Name = elementName;
            panel.Width = width;
            panel.Height = height;
            Canvas.SetLeft(panel, 0);
            Canvas.SetTop(panel, 0);
            border.Child = panel;
            
            return border;
        }

        public static StackPanel CreateStackPanel(int width, int height, int rootX, int rootY, string elementName)
        {
            StackPanel panel = new StackPanel();
            panel.Name = elementName;
            panel.Width = width;
            panel.Height = height;
            Canvas.SetLeft(panel, rootX);
            Canvas.SetTop(panel, rootY);
            
            return panel;
        }

        public static void AddElement(Border control, Control content)
        {
            if(control.Child == null)
            {
                return;
            }
            StackPanel? panel = control.Child as StackPanel;
            panel?.Children.Add(content);
        }
    }
}