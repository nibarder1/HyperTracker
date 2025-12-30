using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.VisualTree;
using HyperTracker.Config;
using HyperTracker.Datatypes;
using SixLabors.ImageSharp;
namespace HyperTracker;

public class Global
{
    /// <summary>
    /// List of configured inputs.
    /// </summary>
    public static List<iInput> APPLICATION_INPUTS = new List<iInput>();
    /// <summary>
    /// Currently selected tab index.
    /// </summary>
    public static int CURRENT_TAB = 0;
    /// <summary>
    /// Maximum recording frames.
    /// </summary>
    public static int MAX_FRAMES = 1500;

    /// <summary>
    /// Recording frames from all inputs.
    /// </summary>
    public static bool IS_RECORDING = false;
    /// <summary>
    /// Recording frames.
    /// </summary>
    public static List<Frame> RECORDING_FRAMES = new List<Frame>();

    public static List<Settings> PROFILE_SETTINGS = new List<Settings>();

    public static int LOADED_PROFILE = 0;

    public static int CURRENT_FRAME_INDEX  = 0;
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

    public static void LoadSettings()
    {
        
    }   

    public static void SaveSettings()
    {
        
    } 
}