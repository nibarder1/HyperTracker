using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using HyperTracker.Datatypes;
using SixLabors.ImageSharp;

namespace HyperTracker.Recordings
{
    public class LoadRecording
    {

        public static void OpenRecording(string recordingPath)
        {
            
            string jsonPath = recordingPath;
            string rootPath = recordingPath;
            if(!jsonPath.EndsWith(".json")){
                jsonPath = Path.Combine(recordingPath, "recording.json");
            } else
            {
                rootPath = Path.GetDirectoryName(jsonPath);
            }
            if (!File.Exists(jsonPath))
            {
                throw new FileNotFoundException("Recording JSON file not found.", jsonPath);
            }

            string json = File.ReadAllText(jsonPath);
            var serializableFrames = JsonConvert.DeserializeObject<List<SerializableFrame>>(json);
            if (serializableFrames == null)
            {
                throw new InvalidDataException("Failed to deserialize recording data.");
            }

            List<Frame> frames = new List<Frame>();
            foreach (var sFrame in serializableFrames)
            {
                Frame frame = new Frame(sFrame.TimeStamp);
                foreach (var kvp in sFrame.CameraFramePaths)
                {
                    string imagePath = Path.Combine(rootPath, kvp.Value);
                    if (File.Exists(imagePath))
                    {
                        var image = Image.Load(imagePath);
                        frame.AddImageToFrame(image, kvp.Key);
                    }
                }
                foreach (var kvp in sFrame.TOFReadings)
                {
                    frame.AddTOFReading(kvp.Value, kvp.Key);
                }
                frames.Add(frame);
            }

            Global.RECORDING_FRAMES = frames;
            Global.CURRENT_FRAME = 0;
            Console.WriteLine("Recording opened.");
            GlobalEvents.InvokeFrameChange(0);
        }
    }
}