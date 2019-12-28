using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Surfaces;
using SadConsole.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole.Renderers;
using SadConsole.Input;

namespace SadConsoleEditor.Consoles
{
    class BorderConsole: SadConsole.Console
    {
        private SadConsole.Console contentContainer;

        public void SetContent(SurfaceBase surface, IRenderer renderer)
        {
            Rectangle newBounds = MainScreen.Instance.InnerEmptyBounds;

            contentContainer.TextSurface = surface;
            contentContainer.TextSurface.Font = Settings.Config.ScreenFont;
            contentContainer.Renderer = renderer;
            TextSurface = new SadConsole.Surfaces.Basic(surface.RenderArea.Width + 2, surface.RenderArea.Height + 2, Settings.Config.ScreenFont);
            PrepBox();
        }

        public BorderConsole(int width, int height): base(width, height, Settings.Config.ScreenFont)
        {
            textSurface.Font = Settings.Config.ScreenFont;
            
            PrepBox();

            contentContainer = new SadConsole.Console(1, 1);
            contentContainer.Position = new Point(1, 1);
            contentContainer.MouseHandler = (c, m) => { return false; };
            Children.Add(contentContainer);
        }

        // Todo: Tint console? Draw yellow dashes (line control?) for the console bounds
        public void PrepBox()
        {
            Clear();

            var box = Box.GetDefaultBox();
            box.Width = TextSurface.Width;
            box.Height = TextSurface.Height;
            box.Fill = false;
            box.TopLeftCharacter = box.TopSideCharacter = box.TopRightCharacter = box.LeftSideCharacter = box.RightSideCharacter = box.BottomLeftCharacter = box.BottomSideCharacter = box.BottomRightCharacter = 177;
            box.Draw(this);

            //List<Cell> newAreaCells = new List<Cell>(TextSurface.Width * 2 + TextSurface.Height * 2);
            //List<Rectangle> newAreaRects = new List<Rectangle>(TextSurface.Width * 2 + TextSurface.Height * 2);

            //if (TextSurface.Width - 2 > MainScreen.Instance.Instance.SelectedEditor.Settings.BoundsWidth)
            //{
            //    for (int x = 1; x <= (TextSurface.Width - 2) / MainScreen.Instance.Instance.SelectedEditor.Settings.BoundsWidth; x++)
            //    {
            //        Line line = new Line();
            //        line.Cell = new Cell() { Foreground = Color.Yellow * 0.5f, Background = Color.Transparent, GlyphIndex = 124 };
            //        line.UseStartingCell = false;
            //        line.UseEndingCell = false;
            //        line.StartingLocation = new Point((x * MainScreen.Instance.Instance.SelectedEditor.Settings.BoundsWidth), 1);
            //        line.EndingLocation = new Point((x * MainScreen.Instance.Instance.SelectedEditor.Settings.BoundsWidth), TextSurface.Height - 2);
            //        line.Draw(this);
            //    }
            //}

            //if (TextSurface.Height - 2 > MainScreen.Instance.Instance.SelectedEditor.Settings.BoundsHeight)
            //{
            //    for (int y = 1; y <= (TextSurface.Height - 2) / MainScreen.Instance.Instance.SelectedEditor.Settings.BoundsHeight; y++)
            //    {
            //        Line line = new Line();
            //        line.Cell = new Cell() { Foreground = Color.Yellow * 0.5f, Background = Color.Transparent, GlyphIndex = 45 };
            //        line.UseStartingCell = false;
            //        line.UseEndingCell = false;
            //        line.StartingLocation = new Point(1, (y * MainScreen.Instance.Instance.SelectedEditor.Settings.BoundsHeight));
            //        line.EndingLocation = new Point(TextSurface.Width - 2, (y * MainScreen.Instance.Instance.SelectedEditor.Settings.BoundsHeight));
            //        line.Draw(this);
            //    }
            //}
        }

        public override bool ProcessMouse(MouseConsoleState state)
        {
            var brush = MainScreen.Instance.Brush;

            if (brush != null)
            {
                MouseConsoleState transformedState = new MouseConsoleState(contentContainer, state.Mouse);

                if (MainScreen.Instance.InnerEmptyBounds.Contains(state.WorldPosition))
                {
                    brush.IsVisible = true;
                    brush.Position = state.ConsolePosition;

                    MainScreen.Instance.ActiveEditor?.ProcessMouse(transformedState, true);
                }
                else
                {
                    brush.IsVisible = false;
                    MainScreen.Instance.ActiveEditor?.ProcessMouse(transformedState, false);
                }
            }

            return false;
        }

        public override void Draw(TimeSpan delta)
        {
            base.Draw(delta);

            MainScreen.Instance.ActiveEditor?.Draw();
        }
    }
}
