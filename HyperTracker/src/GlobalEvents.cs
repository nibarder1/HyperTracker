using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Threading;
using HyperTracker.Core;
using HyperTracker.CV;
using HyperTracker.IO;

namespace HyperTracker
{
    public class GlobalEvents
    {
        private static System.Timers.Timer? _captureTimer;
        private static System.Timers.Timer? _liveTimer;
        private static RecordingStatus _recordingStatus = RecordingStatus.IDLE;
        public static event Action? OnCaptureFrame;
        public static event Action? OnStartRecording;
        public static event Action? OnStopRecording;
        public static event Action? OnCancelRecording;
        public static event Action? OnRebuildUI;
        public static event Action? OnUpdateLive;
        public static event Action? OnReady;
        public static event Action? OnFrameChange;

        public static event Action? OnExit;

        public static int LiveUpdateSubscriptions => OnUpdateLive?.GetInvocationList().Length ?? 0;
        public static RecordingStatus RecordingStatus => _recordingStatus;

        public static void Ready()
        {
            OnReady?.Invoke();
        }

        public static void Init()
        {
            _liveTimer = new System.Timers.Timer(200);
            _liveTimer.Elapsed += _liveTick;
            _liveTimer.AutoReset = true;
            _liveTimer.Start();

            _captureTimer = new System.Timers.Timer(Global.Config!.RecordingCycleMs);
            _captureTimer.Elapsed += _captureEvent;
            _captureTimer.AutoReset = true;
            _captureTimer.Start();

            if(!Directory.Exists($"{Global.ApplicationPath}/recordings"))
            {
                Directory.CreateDirectory($"{Global.ApplicationPath}/recordings");
            }
            if(!Directory.Exists($"{Global.ApplicationPath}/programs"))
            {
                Directory.CreateDirectory($"{Global.ApplicationPath}/programs");
            }
        }

        public static void UpdateRecordingCycle(int ms)
        {
            if(_captureTimer != null)
            {
                _captureTimer.Interval = ms;
            }            
        }

        private static void _liveTick(object? sender, ElapsedEventArgs e)
        {
            UpdateLive();
        }
       

        private static void _captureEvent(object? sender, ElapsedEventArgs e)
        {
            if(_recordingStatus == RecordingStatus.RECORDING)
            {
                OnCaptureFrame?.Invoke();
            }            
        }

        public static void ChangeFrame()
        {
            OnFrameChange?.Invoke();
        }

        public static void StartRecording()
        {
            if(_recordingStatus == RecordingStatus.IDLE)
            {
                _recordingStatus = RecordingStatus.RECORDING;
                Global.Recording.Frames.Clear();
                Global.Recording.Properties.Clear();
                foreach(Camera camera in Global.Cameras)
                {
                    if(camera.Properties != null) Global.Recording.Properties.Add(camera.CameraName, camera.Properties);
                }
                OnStartRecording?.Invoke();
            }
        }

        public static void StopRecording(Action cb)
        {
            if(_recordingStatus == RecordingStatus.RECORDING)
            {
                _recordingStatus = RecordingStatus.SAVING;
                OnStopRecording?.Invoke();
                Task.Run(() => {RecordingIO.SaveRecording($"{Global.ApplicationPath}/recordings", cb);});                
            }
        }
        public static void CancelRecording()
        {
            if(_recordingStatus == RecordingStatus.RECORDING || _recordingStatus == RecordingStatus.SAVING)
            {
                _recordingStatus = RecordingStatus.IDLE;
                OnCancelRecording?.Invoke();
            }
        }

        public static void UpdateLive()
        {
            Dispatcher.UIThread.Post(() =>
            {
                try
                {
                    OnUpdateLive?.Invoke();
                }catch(Exception e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine(e.StackTrace);
                }
            });           
            
        }

        public static void RebuildUI()
        {
            try
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    OnRebuildUI?.Invoke();
                });
            }catch
            {
                
            }
            
            
        }

        public static void Exit()
        {
            _liveTimer?.Stop();
            _captureTimer?.Stop();
            OnExit?.Invoke();
            Environment.Exit(0);
        }
    }
}