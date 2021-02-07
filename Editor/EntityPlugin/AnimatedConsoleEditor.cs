using System;
using System.Collections.Generic;
using System.Text;
using SadConsole;
using SadRogue.Primitives;

namespace EntityPlugin
{
    public class AnimatedConsoleEditor : SadConsole.AnimatedScreenSurface
    {
        public AnimatedConsoleEditor(string name, int width, int height, Font font, Point fontSize) : base(name, width, height, font, fontSize)
        {
        }

        public AnimatedConsoleEditor(AnimatedScreenSurface baseConsole): base(baseConsole.Name, baseConsole.Width, baseConsole.Height)
        {
            FramesList = new List<ICellSurface>(baseConsole.Frames);
            
        }

        public void InsertFrame(int index, CellSurface frame)
        {
            FramesList.Insert(index, frame);
        }

        public void RemoveFrame(CellSurface frame)
        {
            if (FramesList.Contains(frame))
                FramesList.Remove(frame);
        }
    }
}
