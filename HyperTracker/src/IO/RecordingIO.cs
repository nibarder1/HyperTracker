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
            if (!Directory.Exists(folderPath))
            {
                try
                {
                    Directory.CreateDirectory(folderPath);
                }
                catch
                {
                    return;
                }
            }
            string recordingPath = $"{folderPath}/recording_{DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss")}";
            try
            {
                Directory.CreateDirectory(recordingPath);
                Directory.CreateDirectory($"{recordingPath}/frames");
            }
            catch
            {
                return;
            }
            string filePath = $"{recordingPath}/recording.json";
            try
            {
                for (int i = 0; i < Global.Recording.Frames.Count; i++)
                {
                    string framePath = $"{recordingPath}/frames/frame{i}.json";
                    Frame frame = Global.Recording.Frames[i];
                    SerializableFrame sFrame = new SerializableFrame(frame.Timestamp,
                                                                    $"{recordingPath}/frames/frame{i + 1}.json",
                                                                    frame.FrameImages.ToDictionary(
                                                                        kvp => kvp.Key,
                                                                        kvp => kvp.Value != null ? MatToBytes(kvp.Value) : null
                                                                    ));
                    string json = JsonSerializer.Serialize(sFrame);
                    File.WriteAllText(framePath, json);
                }
                // Create a serializable version of the recording
                var serializableRecording = new SerializableRecording(Global.Recording.Properties, $"{recordingPath}/frames/frame0.json");

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
            try
            {
                string json = File.ReadAllText(filePath);
                var serializableRecording = JsonSerializer.Deserialize<SerializableRecording>(json);

                if (serializableRecording != null)
                {
                    Global.Recording.Properties = serializableRecording.Properties;
                    Global.Recording.Frames.Clear();
                    string frameFile = serializableRecording.FirstFramePath;
                    while (File.Exists(frameFile))
                    {
                        var frameJson = File.ReadAllText(frameFile);
                        var frameData = JsonSerializer.Deserialize<SerializableFrame>(frameJson);
                        if (frameData != null)
                        {
                            Frame frame = new Frame(frameData.Timestamp);
                            frame.FrameImages = frameData.FrameImages.ToDictionary(kvp => kvp.Key, kvp => kvp.Value != null ? BytesToMat(kvp.Value) : null);
                            Global.Recording.Frames.Add(frame);
                            frameFile = frameData.NextFramePath;
                        }
                        else
                        {
                            throw new Exception("Unable to load frames.");
                        }
                    }

                    Console.WriteLine("Recording loaded successfully.");
                    Global.CurrentFrame = 0;
                    Console.WriteLine(Global.Recording.Frames.Count);
                    AnalysisManager.Load();
                    GlobalEvents.ChangeFrame();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading recording: {ex.Message}");
            }
        }

        public static List<string> GetLastRecordings(int amount)
        {
            List<string> recordings = new List<string>();
            DirectoryInfo dir = new DirectoryInfo($"{Global.ApplicationPath}/recordings");
            var sortedDir = dir.GetDirectories().OrderByDescending(d => d.CreationTime).ToList();
            foreach(DirectoryInfo dirInfo in sortedDir)
            {
                if(recordings.Count > amount) break;
                recordings.Add(dirInfo.FullName);
            }

            return recordings;
        }

        private static byte[]? MatToBytes(Mat? mat)
        {
            if (mat == null) return null;
            return mat.ImEncode(".png");
        }

        private static Mat? BytesToMat(byte[]? bytes)
        {
            if (bytes == null) return null;
            return Cv2.ImDecode(bytes, ImreadModes.Unchanged);
        }

        private class SerializableRecording
        {
            public SerializableRecording(Dictionary<string, CameraProperties> properties, string firstFramePath)
            {
                Properties = properties;
                FirstFramePath = firstFramePath;
            }

            public Dictionary<string, CameraProperties> Properties { get; set; }
            public string FirstFramePath { get; set; }
        }

        private class SerializableFrame
        {
            public SerializableFrame(DateTime timestamp, string nextFramePath, Dictionary<string, byte[]?> frameImages)
            {
                Timestamp = timestamp;
                NextFramePath = nextFramePath;
                FrameImages = frameImages;
            }

            public DateTime Timestamp { get; set; }
            public string NextFramePath { get; set; }
            public Dictionary<string, byte[]?> FrameImages { get; set; }
            
            
        }
    }
}