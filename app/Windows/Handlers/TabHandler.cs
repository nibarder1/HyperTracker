using System;
using Avalonia.Controls;

namespace HyperTracker.Windows.Handlers;

/// <summary>
/// Tab control event handlers.
/// </summary>
public class TabHandler
{
    /// <summary>
    /// Change tab event handler.
    /// </summary>
    /// <param name="sender">Avalonia UI control.</param>
    /// <param name="e">Event arguments.</param>
    public static void ChangeTab(object sender, SelectionChangedEventArgs e)
    {
        var tabControl = (TabControl)sender;
        int tabIndex = tabControl.SelectedIndex;
        if(tabIndex == 1 && Global.CURRENT_FRAME >= Global.RECORDING_FRAMES.Count)
        {
            GlobalEvents.InvokeFrameChange(0);
        }
        if(tabIndex == 2)
        {
            //Settings

        }
    }
}