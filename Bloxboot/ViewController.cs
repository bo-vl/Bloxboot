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
            jsonManager.Add("DFIntTaskSchedulerTargetFps", fps);
            jsonManager.Add("FFlagDebugGraphicsDisableMetal", true);
            jsonManager.WriteFile();
        }
    }
}

