using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using HyperTracker.Core;
using HyperTracker.CV;
using OpenCvSharp;

namespace HyperTracker.IO
{
    public class RecordingIO
    {
        public static void SaveRecording(string folderPath, Action cb)
        {
            if(!Directory.Exists(folderPath))
            {
                try{
                    Directory.CreateDirectory(folderPath);
                }catch
                {
                    return;
                }
            }
            string recordingPath = $"{folderPath}/recording_{DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss")}";
            try
            {
                Directory.CreateDirectory(recordingPath);
                Directory.CreateDirectory($"{recordingPath}/frames");
            }catch
            {
                return;
            }
            string filePath = $"{recordingPath}/recording.json";
            try
            {
                for(int i = 0; i < Global.Recording.Frames.Count; i++)
                {
                    string framePath = $"{recordingPath}/frames/frame{i}.frame";
                    Frame frame = Global.Recording.Frames[i];
                    SerializableFrame sFrame = new SerializableFrame {
                        Timestamp = frame.Timestamp,
                        FrameImages = frame.FrameImages.ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value != null ? MatToBytes(kvp.Value) : null
                        ),
                        NextFramePath = $"{recordingPath}/frames/frame{i+1}.frame"
                    };
                    string json = JsonSerializer.Serialize(sFrame);
                    File.WriteAllText(framePath, json);
                }
                // Create a serializable version of the recording
                var serializableRecording = new SerializableRecording
                {
                    Properties = Global.Recording.Properties,
                    FirstFramePath = $"{recordingPath}/frames/frame0.frame"
                };

                string jsonR = JsonSerializer.Serialize(serializableRecording);
                File.WriteAllText(filePath, jsonR);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving recording: {ex.Message}");
            }
            cb();
        }

        public static void LoadRecording(string filePath)
        {
            // try
            // {
            //     string json = File.ReadAllText(filePath);
            //     var serializableRecording = JsonSerializer.Deserialize<SerializableRecording>(json);
                
            //     if (serializableRecording != null)
            //     {
            //         Global.Recording.Properties = serializableRecording.Properties;
            //         Global.Recording.Frames = serializableRecording.Frames.Select(sf => new Frame(sf.Timestamp)
            //         {
            //             FrameImages = sf.FrameImages.ToDictionary(
            //                 kvp => kvp.Key,
            //                 kvp => kvp.Value != null ? BytesToMat(kvp.Value) : null
            //             )
            //         }).ToList();
                    
            //         Console.WriteLine("Recording loaded successfully.");
            //     }
            // }
            // catch (Exception ex)
            // {
            //     Console.WriteLine($"Error loading recording: {ex.Message}");
            // }
        }

        public static List<string> GetLastRecordings(string path, int amount)
        {
            List<string> recordings = new List<string>();

            return recordings;
        }

        private static byte[]? MatToBytes(Mat? mat)
        {
            if(mat == null) return null;
            return mat.ImEncode(".png");
        }

        private static Mat? BytesToMat(byte[]? bytes)
        {
            if (bytes == null) return null;
            return Cv2.ImDecode(bytes, ImreadModes.Unchanged);
        }

        private class SerializableRecording
        {
            public Dictionary<string, CameraProperties> Properties { get; set; } = new();
            public string FirstFramePath {get; set;} = "";
        }

        private class SerializableFrame
        {
            public DateTime Timestamp { get; set; }
            public Dictionary<string, byte[]?> FrameImages { get; set; } = new();
            public string NextFramePath {get; set;} = "";
        }
    }
}