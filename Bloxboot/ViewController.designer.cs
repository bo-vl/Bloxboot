// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Bloxboot
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSTabView tabs { get; set; }

		[Action ("Framerate:")]
		partial void Framerate (AppKit.NSTextField sender);

		[Action ("Save:")]
		partial void Save (AppKit.NSButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (tabs != null) {
				tabs.Dispose ();
				tabs = null;
			}
		}
	}
}
