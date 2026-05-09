using System;
using System.Collections.Generic;
using System.IO;
using Avalonia;
using HyperTracker.Core;
using HyperTracker.CV;
using HyperTracker.UI.Themes;

namespace HyperTracker
{
    public class Global
    {
        public static Config? Config = new Config();
        public static List<Camera> Cameras = new List<Camera>();
        public static Recording Recording = new Recording();
        public static ThemeBase Theme = new ThemeBase();
        public static int CurrentTab = 0;
        public static int CurrentFrame = 0;

        public static string ApplicationPath = AppDomain.CurrentDomain.BaseDirectory;
    }
}