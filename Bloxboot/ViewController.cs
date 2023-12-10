using System;
using AppKit;
using Bloxboot.Utilities;
using Foundation;

namespace Bloxboot
{
    public partial class ViewController : NSViewController
    {
        JsonManager jsonManager = new JsonManager();
        RobloxLogger robloxLogger = new RobloxLogger();
        public int fps;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            CheckAndDisplayActivity();
        }

        public override NSObject RepresentedObject
        {
            get { return base.RepresentedObject; }
            set { base.RepresentedObject = value; }
        }

        partial void lighting(NSComboBoxCell sender)
        {
            Console.WriteLine(sender);
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

        private void CheckAndDisplayActivity()
        {
            string activityResult = RobloxLogger.CheckActivity();
            Console.WriteLine(activityResult);

        }
    }
}
