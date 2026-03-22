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
    /// Profile settings.
    /// </summary>
    public static List<Settings> PROFILE_SETTINGS = new List<Settings>();

    /// <summary>
    /// Loaded profile application input modules.
    /// </summary>
    public static List<iModule> APPLICATION_INPUTS = new List<iModule>();

    /// <summary>
    /// Recording frame buffer.
    /// </summary>
    public static List<Frame> RECORDING_FRAMES = new List<Frame>();

    /// <summary>
    /// Current loaded frame.
    /// </summary>
    public static int CURRENT_FRAME = 0;

    public static int LOADED_PROFILE = 0;

    public static int MAX_FRAMES = 1500;

    

    /// <summary>
    /// Load profile using index.
    /// </summary>
    /// <param name="index">Index of profile to learn.</param>
    public static void LoadProfile(int index)
    {
        if(APPLICATION_INPUTS.Count > 0)
        {
            foreach(iModule iInput in APPLICATION_INPUTS)
            {
                iInput.Release();
            }
        }
        Console.WriteLine("Creating inputs.");
        APPLICATION_INPUTS = new List<iModule>();
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

            CameraModule cam = new CameraModule(ip);
            cam.Initialize();
            if(cam.IsInitialized())
            {
                Console.WriteLine($"Starting camera: {camSettings.CameraName}");
                Task.Run(cam.Start);
                Console.WriteLine("Camera started");
            }

            APPLICATION_INPUTS.Add(cam);
        }
        MAX_FRAMES = PROFILE_SETTINGS[index].RecordingSeconds * 1000 / PROFILE_SETTINGS[index].TargetCycleMs;
        Console.WriteLine($"MAX RECORDING FRAMES: {MAX_FRAMES}");
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
            GlobalEvents.InvokeSettingsChange();
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
        Settings defaultSettings = new Settings("DEFAULT", TrackMode.VERTICLE_DISTANCE, 15, 5, cameras);
        PROFILE_SETTINGS = [defaultSettings];
        GlobalEvents.InvokeLoadProfile(0);
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