using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if MACOS
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif

namespace StarterProject
{
	class Program
	{
		public static Game1 Game;
		static void Main(string[] args)
		{
			#if !MACOS
				Game = new Game1();
				Game.Run();
			#else
				NSApplication.Init ();
				using (var p = new NSAutoreleasePool ()) {
					NSApplication.SharedApplication.Delegate = new AppDelegate ();
					NSApplication.Main (args);
				}
			#endif
		}
	}

	#if MACOS
	class AppDelegate : NSApplicationDelegate
	{
		public override void FinishedLaunching (MonoMac.Foundation.NSObject notification)
		{
			Program.Game = new Game1();
			Program.Game.Run();
		}

		public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender)
		{
			return true;
		}
	}
	#endif
}