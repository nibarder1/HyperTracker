using System;
using System.Collections.Generic;
using System.Threading;
using HyperTracker.Datatypes;
using SixLabors.ImageSharp;

namespace HyperTracker.Threads;

public class CaptureThread
{
    public static void ThreadLoop()
    {
        while(true)
        {
            if(Global.IS_RECORDING)
            {
                Frame frame = new Frame(DateTime.Now);
                foreach(iInput i in Global.APPLICATION_INPUTS)
                {
                    if(i.GetInputType() == InputTypes.CAMERA)
                    {
                        Camera? c = i as Camera;
                        if(c != null)
                        {
                            Image? img = c.GetScan();
                            if(img != null)
                            {
                                frame.AddImageToFrame(img, c.GetParams().GetParameter<string>("InputName")!);
                            }                            
                        }
                    }
                }
                Global.RECORDING_FRAMES.Add(frame);
                if(Global.RECORDING_FRAMES.Count > Global.MAX_FRAMES)
                {
                    Global.RECORDING_FRAMES.RemoveAt(0);
                }
            }
            Thread.Sleep(Global.PROFILE_SETTINGS[Global.LOADED_PROFILE].TargetCycleMs);
        }
    }
}