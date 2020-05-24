using SadRogue.Primitives;
using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole.Renderers;
using SadConsole.Input;

namespace SadConsoleEditor.Consoles
{
    class BorderConsole: SadConsole.ScreenSurface
    {
        public void SetContent(SadConsole.Console console)
        {
            Rectangle newBounds = console.View;

            Children.Add(console);
            console.Position = new Point(1, 1);

            if (newBounds.Width > MainConsole.Instance.InnerEmptyBounds.Width)
                newBounds = new Rectangle(newBounds.X, newBounds.Y, MainConsole.Instance.InnerEmptyBounds.Width, newBounds.Height);
            if (newBounds.Height > MainConsole.Instance.InnerEmptyBounds.Height)
                newBounds = new Rectangle(newBounds.X, newBounds.Y, newBounds.Width, MainConsole.Instance.InnerEmptyBounds.Height);

            Surface.Resize(newBounds.Width + 2, newBounds.Height + 2, newBounds.Width + 2, newBounds.Height + 2,true);
            PrepBox();
        }

        public BorderConsole(int width, int height): base(width, height)
        {
            Font = Config.Program.ScreenFont;
            FontSize = Config.Program.ScreenFontSize;
        }

        // Todo: Tint console? Draw yellow dashes (line control?) for the console bounds
        public void PrepBox()
        {
            Surface.Clear();
            Surface.DrawBox(new Rectangle(0, 0, Surface.BufferWidth, Surface.BufferHeight), new ColoredGlyph(Surface.DefaultForeground, Color.Black, 177));
        }

        public override bool ProcessMouse(MouseScreenObjectState state)
        {
            var brush = MainConsole.Instance.Brush;

            if (brush != null)
            {
                //MouseScreenObjectState transformedState = new MouseScreenObjectState(contentContainer, state.Mouse);


            }

            return true;
        }

        public override void Draw(TimeSpan delta)
        {
            base.Draw(delta);

            MainConsole.Instance.ActiveEditor?.Draw();
        }
    }
}
