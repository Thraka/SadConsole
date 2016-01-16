using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castle
{
    class Program
    {
        public static CastleGame Game;
        static void Main(string[] args)
        {
#if !MACOS
            Game = new CastleGame();
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
			Program.Game = new CastleGame();
			Program.Game.Run();
		}

		public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender)
		{
			return true;
		}
	}
#endif
}
