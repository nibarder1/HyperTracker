using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.VisualTree;
using HyperTracker.Config;
using HyperTracker.Datatypes;
using Newtonsoft.Json;

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

    /// <summary>
    /// Available profiles.
    /// </summary>
    public static List<Settings> PROFILE_SETTINGS = new List<Settings>();

    /// <summary>
    /// Currently loaded profile index.
    /// </summary>
    public static int LOADED_PROFILE = 0;

    /// <summary>
    /// Current index of the recorded frames.
    /// </summary>
    public static int CURRENT_FRAME_INDEX  = 0;

    /// <summary>
    /// Flag to rebuild current UI.
    /// </summary>
    public static bool REBUILD_UI = true;

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

    /// <summary>
    /// Load profile using name.
    /// </summary>
    /// <param name="profileName">Name of the profile to load.</param>
    public static void LoadProfile(string profileName)
    {
        for(int i = 0; i < PROFILE_SETTINGS.Count; i++)
        {
            if(PROFILE_SETTINGS[i].ProfileName.Equals(profileName))
            {
                LoadProfile(i);
                return;
            }
        }
    }

    /// <summary>
    /// Load profile using index.
    /// </summary>
    /// <param name="index">Index of profile to learn.</param>
    public static void LoadProfile(int index)
    {
        if(APPLICATION_INPUTS.Count > 0)
        {
            foreach(iInput iInput in APPLICATION_INPUTS)
            {
                iInput.Release();
            }
        }
        Thread.Sleep(1000);
        Console.WriteLine("Creating inputs.");
        APPLICATION_INPUTS = new List<iInput>();
        foreach(CameraSettings camSettings in PROFILE_SETTINGS[index].Cameras)
        {
            InputParameters ip = new InputParameters();
            ip.AddParam("CameraIndex", camSettings.CameraIndex);
            ip.AddParam("CameraWidth", camSettings.CameraWidth);
            ip.AddParam("CameraHeight", camSettings.CameraHeight);
            ip.AddParam("CameraFPS", camSettings.FPS);
            ip.AddParam("IsStereo", false);
            ip.AddParam("IsEnabled", true);
            ip.AddParam("InputName", camSettings.CameraName);

            Camera cam = new Camera(ip);
            cam.Initialize();
            if(cam.IsInitialized())
            {
                Console.WriteLine($"Starting camera: {camSettings.CameraName}");
                Task.Run(cam.Start);
                Console.WriteLine("Camera started");
            }

            APPLICATION_INPUTS.Add(cam);
        }

        REBUILD_UI = true;

    }

    /// <summary>
    /// Load settings from file.
    /// </summary>
    public static void LoadSettings()
    {
        BuildFS();
        string root = GetProcessFolderPath();
        if(!File.Exists($"{root}/data/profiles.json"))
        {
            LoadDefaultSettings();
            SaveSettings();
        }else
        {
            string json = File.ReadAllText($"{root}/data/profiles.json");
            var settings = JsonConvert.DeserializeObject<List<Settings>>(json);
            if(settings != null)
            {
                PROFILE_SETTINGS = settings;
            }
            else
            {
                LoadDefaultSettings();
            }
        }
    }   

    /// <summary>
    /// Load default profile settings.
    /// </summary>
    public static void LoadDefaultSettings()
    {
        CameraSettings camera1Settings = new CameraSettings(true, "Floor", 0, 1280, 720, 120, 100, 90, 10, 0);
        CameraSettings camera2Settings = new CameraSettings(true, "Elevated", 1, 1280, 720, 120, 100, 90, 10, 1000);
        List<CameraSettings> cameras = [camera1Settings, camera2Settings];
        Settings defaultSettings = new Settings("DEFAULT", TrackMode.VERTICLE_DISTANCE, 5, cameras);
        PROFILE_SETTINGS = [defaultSettings];
        LOADED_PROFILE = 0;
    }

    /// <summary>
    /// Save settings to file.
    /// </summary>
    public static void SaveSettings()
    {
        BuildFS();
        string json = JsonConvert.SerializeObject(PROFILE_SETTINGS);
        string root = GetProcessFolderPath();
        File.WriteAllText($"{root}/data/profiles.json", json);
    } 

    /// <summary>
    /// Build file system.
    /// </summary>
    private static void BuildFS()
    {
        string root = GetProcessFolderPath();
        if(!Directory.Exists($"{root}/data"))
        {
            Directory.CreateDirectory($"{root}/data");
        }
    }

    /// <summary>
    /// Get folder path of the process.
    /// </summary>
    /// <returns>Process folder path.</returns>
    public static string GetProcessFolderPath()
    {
        return System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName) ?? ".";
    }
}