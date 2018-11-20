using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Surfaces;

namespace SadConsole.Debug
{
    static class CurrentScreen
    {
        private class DebugSurface: SurfaceBase
        {
            public override void Draw(TimeSpan timeElapsed)
            {
                base.Draw(timeElapsed);
            }
        }

        public static void Show()
        {

        }

    }
}
