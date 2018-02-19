using Microsoft.Xna.Framework;
using SadConsole.Input;
using SadConsole.Surfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Editor.Tools
{
    internal class Box : ITool
    {
        private AnimatedSurface animSinglePoint;
        private SadConsole.Effects.Fade frameEffect;
        private Point? firstPoint;
        private Point secondPoint;
        private SadConsole.Shapes.Box boxShape;
        private bool cancelled;



        public string Name => "Box";

        public ToolBrush Brush { get; private set; } = new ToolBrush(1, 1);

        public System.Windows.Forms.Control GetUI()
        {
            return new System.Windows.Forms.Button() { Width = 100, Height = 25, Text = "Box", Name = "ToolPanel" };
        }

        public void OnUpdate(MouseConsoleState state)
        {
            //if (cancelled)
            //{
            //    // wait until left button is released...
            //    if (state.Mouse.LeftButtonDown)
            //        return;
            //    else
            //        cancelled = false;
            //}

            //if (state.Mouse.LeftButtonDown)
            //{
            //    if (!firstPoint.HasValue)
            //    {
            //        firstPoint = state.ConsolePosition;
            //        return;
            //    }
            //    else
            //    {
            //        // Check for right click cancel.
            //        if (state.Mouse.RightButtonDown)
            //        {
            //            cancelled = true;
            //            firstPoint = null;
            //            return;
            //        }


            //        secondPoint = state.ConsolePosition;

            //        // Draw the line (erase old) to where the mouse is
            //        // create the animation frame
            //        AnimatedSurface animation = new AnimatedSurface("line", Math.Max(firstPoint.Value.X, secondPoint.X) - Math.Min(firstPoint.Value.X, secondPoint.X) + 1,
            //                                                                        Math.Max(firstPoint.Value.Y, secondPoint.Y) - Math.Min(firstPoint.Value.Y, secondPoint.Y) + 1,
            //                                                                        DataContext.Instance.SelectedFont);

            //        var frame = animation.CreateFrame();

            //        Point p1;

            //        if (firstPoint.Value.X > secondPoint.X)
            //        {
            //            if (firstPoint.Value.Y > secondPoint.Y)
            //                p1 = Point.Zero;
            //            else
            //                p1 = new Point(0, frame.Height - 1);
            //        }
            //        else
            //        {
            //            if (firstPoint.Value.Y > secondPoint.Y)
            //                p1 = new Point(frame.Width - 1, 0);
            //            else
            //                p1 = new Point(frame.Width - 1, frame.Height - 1);
            //        }


            //        animation.Center = p1;

            //        SadConsoleEditor.Settings.QuickEditor.TextSurface = frame;
            //        boxShape = SadConsole.Shapes.Box.GetDefaultBox();

            //        if (_settingsPanel.UseCharacterBorder)
            //            boxShape.LeftSideCharacter = boxShape.RightSideCharacter =
            //            boxShape.TopLeftCharacter = boxShape.TopRightCharacter = boxShape.TopSideCharacter =
            //            boxShape.BottomLeftCharacter = boxShape.BottomRightCharacter = boxShape.BottomSideCharacter =
            //            _settingsPanel.BorderCharacter;

            //        boxShape.Foreground = _settingsPanel.LineForeColor;
            //        boxShape.FillColor = _settingsPanel.FillColor;
            //        boxShape.Fill = _settingsPanel.UseFill;
            //        boxShape.BorderBackground = _settingsPanel.LineBackColor;
            //        boxShape.Position = new Point(0, 0);
            //        boxShape.Width = frame.Width;
            //        boxShape.Height = frame.Height;
            //        boxShape.Draw(SadConsoleEditor.Settings.QuickEditor);

            //        Brush.Animation = animation;

            //    }
            //}
            //else if (firstPoint.HasValue)
            //{
            //    // We let go outside of bounds
            //    if (!isInBounds)
            //    {
            //        cancelled = true;
            //        return;
            //    }

            //    // Position the box shape and draw
            //    boxShape.Position = new Point(Math.Min(firstPoint.Value.X, secondPoint.X), Math.Min(firstPoint.Value.Y, secondPoint.Y))
            //                        + state.Console.TextSurface.RenderArea.Location;

            //    SadConsoleEditor.Settings.QuickEditor.TextSurface = surface;
            //    boxShape.Draw(SadConsoleEditor.Settings.QuickEditor);

            //    firstPoint = null;

            //    Brush.Animation = Brush.Animations["single"];
            //}
        }
    }
}
