using System;
using Avalonia.Controls.Embedding.Offscreen;

namespace HyperTracker
{
    public class GlobalEvents
    {
        /// <summary>
        /// Recording state.
        /// </summary>
        private static bool _isRecording = false;

        public static bool IsSaving = false;

        private static int _currentFrame = 0;

        /// <summary>
        /// Full UI rebuild event.
        /// </summary>
        public static event Action? OnRebuildUI;

        /// <summary>
        /// Update live UI feeds event.
        /// </summary>
        public static event Action? OnLiveUIUpdate;

        /// <summary>
        /// Start recording event.
        /// </summary>
        public static event Action? OnRecordStart;

        /// <summary>
        /// End recording event.
        /// </summary>
        public static event Action? OnRecordEnd;

        /// <summary>
        /// Load profile event.
        /// </summary>
        public static event Action<int>? OnLoadProfile;

        /// <summary>
        /// Settings change event.
        /// </summary>
        public static event Action? OnSettingsChange;

        public static event Action<int>? OnFrameChange;

        public static event Action? OnCaptureFrame;

        /// <summary>
        /// Invoke full UI rebuild.
        /// </summary>
        public static void InvokeRebuildUI()
        {
            OnRebuildUI?.Invoke();
        }

        /// <summary>
        /// Invoke live feed update.
        /// </summary>
        public static void InvokeOnLiveUIUpdate()
        {
            OnLiveUIUpdate?.Invoke();
        }

        /// <summary>
        /// Invoke start recording.
        /// </summary>
        public static void InvokeOnRecordStart()
        {
            if(_isRecording || IsSaving)
            {
                return;
            }
            OnRecordStart?.Invoke();
            OnCaptureFrame?.Invoke();
            _isRecording = true;
        }

        /// <summary>
        /// Invoke end recording.
        /// </summary>
        public static void InvokeOnRecordEnd()
        {
            if(!_isRecording || IsSaving)
            {
                return;
            }
            OnRecordEnd?.Invoke();
            _isRecording = false;
        }

        /// <summary>
        /// Invoke load profile.
        /// </summary>
        /// <param name="profileIndex">Profile index to load.</param>
        public static void InvokeLoadProfile(int profileIndex, bool forced = false)
        {
            if(profileIndex < 0)
            {
                return;
            }
            if(profileIndex == Global.LOADED_PROFILE && !forced)
            {
                return;
            }
            OnLoadProfile?.Invoke(profileIndex);
            Global.LOADED_PROFILE = profileIndex;
            OnRebuildUI?.Invoke();
        }

        /// <summary>
        /// Invoke settings change.
        /// </summary>
        public static void InvokeSettingsChange()
        {
            OnSettingsChange?.Invoke();
        }

        /// <summary>
        /// Invoke analysis frame change.
        /// </summary>
        /// <param name="frameIndex">Recording frame index.</param>
        public static void InvokeFrameChange(int frameIndex)
        {
            if(frameIndex == _currentFrame && frameIndex > 0)
            {
                return;
            }
            if(frameIndex >= Global.RECORDING_FRAMES.Count)
            {
                frameIndex = Global.RECORDING_FRAMES.Count - 1;
            }
            if(frameIndex < 0)
            {
                frameIndex = 0;
            }
            OnFrameChange?.Invoke(frameIndex);
            _currentFrame = frameIndex;
        }

        public static void InvokeNextFrame()
        {
            InvokeFrameChange(_currentFrame + 1);
        }

        public static void InvokePreviousFrame()
        {
            InvokeFrameChange(_currentFrame - 1);
        }

        public static void InvokeCaptureFrame()
        {
            OnCaptureFrame?.Invoke();
        }

    }
}