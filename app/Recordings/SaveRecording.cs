using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using HyperTracker.Datatypes;
using SixLabors.ImageSharp;
using System.Threading.Tasks;

namespace HyperTracker.Recordings
{
    public class SaveRecording
    {
        
        private static void _buildFileSystem()
        {
            string rootPath = Global.GetProcessFolderPath();
            string folderPath = $"{rootPath}/recordings";
            if(!Directory.Exists(folderPath))
            {
                try
                {
                    Directory.CreateDirectory(folderPath);
                }
                catch
                {
                    
                }                
            }
        }
        public static async Task Save()
        {
            GlobalEvents.IsSaving = true;
            _buildFileSystem();
            string rootPath = Global.GetProcessFolderPath();
            string recordingsPath = $"{rootPath}/recordings";
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string recordingFolder = $"{recordingsPath}/{timestamp}";
            Directory.CreateDirectory(recordingFolder);

            List<SerializableFrame> serializableFrames = new List<SerializableFrame>();
            for (int i = 0; i < Global.RECORDING_FRAMES.Count; i++)
            {
                Frame frame = Global.RECORDING_FRAMES[i];
                SerializableFrame sFrame = new SerializableFrame
                {
                    TimeStamp = frame.TimeStamp
                };
                foreach (var kvp in frame.CameraFrames)
                {
                    string imagePath = $"{recordingFolder}/frame_{i}_camera_{kvp.Key}.jpg";
                    kvp.Value.GetValue().Save(imagePath);
                    sFrame.CameraFramePaths[kvp.Key] = Path.GetRelativePath(recordingFolder, imagePath);
                }
                foreach (var kvp in frame.TOFFrames)
                {
                    sFrame.TOFReadings[kvp.Key] = kvp.Value.GetValue();
                }
                serializableFrames.Add(sFrame);
            }
            string jsonPath = $"{recordingFolder}/recording.json";
            string json = JsonConvert.SerializeObject(serializableFrames, Formatting.Indented);
            File.WriteAllText(jsonPath, json);
            GlobalEvents.IsSaving = false;
        }

        public static void CleanUp()
        {
            
        }
    }
}