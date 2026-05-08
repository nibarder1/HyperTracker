using Avalonia.Controls;
using Avalonia.Styling;

namespace HyperTracker.UI.UIBuilders
{
    public class TabControlBuilder
    {
        public static TabControl CreateTabControl(int width, int height, int rootX, int rootY, string elementName)
        {
            TabControl control = new TabControl();
            control.Name = elementName;
            control.Width = width;
            control.Height = height;
            control.Background = Global.Theme.TabControlBackgroundBrush;
            var style = new Avalonia.Styling.Style(x => x.OfType<TabControl>().Descendant().OfType<WrapPanel>());
            style.Setters.Add(new Setter(WrapPanel.MaxHeightProperty, Global.Theme.TabControlHeaderHeight));
            var activeTabStyle = new Style(x => x.OfType<TabItem>().Class(":selected"));
            activeTabStyle.Setters.Add(new Setter(TabItem.ForegroundProperty, Global.Theme.TabControlSelectedTextBrush));
            activeTabStyle.Setters.Add(new Setter(TabItem.BackgroundProperty, Global.Theme.TabControlSelectedBrush));

            control.Styles.Add(style);
            control.Styles.Add(activeTabStyle);
            Canvas.SetLeft(control, rootX);
            Canvas.SetTop(control, rootY);

            return control;
        }

        public static void AddTab(TabControl control, Control tabContent, string tabName)
        {
            TabItem item = new TabItem()
            {
                Header = tabName,
                Content = tabContent
            };
            control.Items.Add(item);
        }
    }
}