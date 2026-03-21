using System.Linq;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace HyperTracker.UI
{
    public class UIControl
    {
        /// <summary>
    /// Get Avalonia control by name.
    /// </summary>
    /// <typeparam name="T">Control type.</typeparam>
    /// <param name="root">Root control.</param>
    /// <param name="name">Name of control to find.</param>
    /// <returns>Control or null.</returns>
    public static T? FindAvaloniaControl<T>(Control root, string name) where T: Control
    {
        return root.GetVisualDescendants().OfType<T>().FirstOrDefault(c => c.Name == name);
    }
    }
}