using System;
using AppKit;
using Bloxboot.Utilities;
using Foundation;
using System.Timers;

namespace Bloxboot
{
    public partial class ViewController : NSViewController
    {

        JsonManager jsonManager = new JsonManager();
        Server server = new Server();
        public int fps;

        private const string searchString = "[FLog::Output] ! Joining game";
        private const int checkIntervalMilliseconds = 5000; 
        private Timer logCheckTimer;

        public ViewController(IntPtr handle) : base(handle)
        {
            logCheckTimer = new Timer(checkIntervalMilliseconds);
            logCheckTimer.Elapsed += LogCheckTimer_Elapsed;
            logCheckTimer.AutoReset = true;
            logCheckTimer.Start();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

        }

        private void LogCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            string latestLogEntry = server.GetLatestLog(searchString);

            if (latestLogEntry != null)
            {
                Console.WriteLine($"Latest log entry containing '{searchString}':");
                Console.WriteLine(latestLogEntry);
            }
            else
            {
                Console.WriteLine($"No log entry containing '{searchString}' found.");
            }
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
    }
}
