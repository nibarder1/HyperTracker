using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using HyperTracker.CV;

namespace HyperTracker.Core
{
    public class Config
    {
        public string ProgramName {get; set;}
        public ProgramType ProgramMode {get; set;}
        public int RecordingTime {get; set;}
        public int RecordingCycleMs {get; set;}

        public List<CameraProperties> Cameras {get; set;}

        public Config(string ProgramName = "DEFAULT",
                        ProgramType ProgramMode = ProgramType.MEASURE,
                        int RecordingTime = 5,
                        int RecordingCycleMs = 10,
                        List<CameraProperties>? Cameras = null)
        {
            this.ProgramName = ProgramName;
            this.ProgramMode = ProgramMode;
            this.RecordingCycleMs = RecordingCycleMs;
            this.RecordingTime = RecordingTime;
            this.Cameras = Cameras == null? new List<CameraProperties>(): Cameras;
        }

        public static Config? LoadConfig(string file)
        {
            try
            {
                string json = File.ReadAllText(file);
                Config? config = JsonSerializer.Deserialize<Config>(json);
                if(config != null)
                {
                    Console.WriteLine("Config loaded.");
                    Console.WriteLine($"{config.Cameras.Count}");
                    return config;
                }
            }
            catch
            {
                Console.WriteLine("Error loading config.");
            }
            return new Config();
        }

        public static void SaveConfig(Config config, string file)
        {
            try
            {
                string json = JsonSerializer.Serialize(config);
                File.WriteAllText(file, json);
            }
            catch
            {
                
            }           
        }
    }

    
}