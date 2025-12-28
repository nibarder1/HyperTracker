using System;

namespace HyperTracker.Windows.Handlers;

/// <summary>
/// Application event handlers.
/// </summary>
public class ApplicationHandler
{
    /// <summary>
    /// Quit application event handler.
    /// </summary>
    public static void QuitApplication()
    {
        Environment.Exit(0);
    }
}