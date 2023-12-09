using System;
using AppKit;
using Bloxboot.Utilities;
using Foundation;

namespace Bloxboot
{
    public partial class ViewController : NSViewController
    {
        JsonManager jsonManager = new JsonManager();
        public int fps;
        private const string JsonKey = "DFIntTaskSchedulerTargetFps";
        private const string disablemetal = "FFlagDebugGraphicsDisableMetal";

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        public override NSObject RepresentedObject
        {
            get { return base.RepresentedObject; }
            set { base.RepresentedObject = value; }
        }

        partial void Framerate(NSTextField sender)
        {
            Console.WriteLine(sender.IntValue);
            fps = sender.IntValue;
        }

        partial void Save(NSButton sender)
        {
            jsonManager.Add(JsonKey, fps);
            jsonManager.Add(disablemetal, true);
            jsonManager.WriteFile();
        }
    }
}

