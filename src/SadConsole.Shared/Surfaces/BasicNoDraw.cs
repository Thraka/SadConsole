using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace SadConsole.Surfaces
{
    /// <summary>
    /// A <see cref="SurfaceBase"/> surface that does not create a backing texture for rendering.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Basic Surface (No Draw)")]
    public class BasicNoDraw : SurfaceBase
    {
        /// <summary>
        /// Creates a new surface from an existing surface.
        /// </summary>
        /// <param name="surface">The original surface</param>
        /// <returns></returns>
        public static BasicNoDraw FromSurface(SurfaceBase surface)
        {
            return new BasicNoDraw(surface.Width, surface.Height, surface.ViewPort, surface.Cells);
        }

        /// <summary>
        /// Creates a new text surface with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        public BasicNoDraw(int width, int height) : this(width, height, new Rectangle(0, 0, width, height), null)
        {

        }

        /// <summary>
        /// Creates a new text surface with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="renderArea">Initial value for the <see cref="BasicSurface.ViewPort"/> view.</param>
        public BasicNoDraw(int width, int height, Rectangle renderArea) : this(width, height, renderArea, null)
        {

        }

        /// <summary>
        /// Creates a new text surface with the specified width, height, and initial set of cell data.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="initialCells">Seeds the cells with existing values. Array size must match <paramref name="width"/> * <paramref name="height"/>.</param>
        /// <param name="renderArea">Initial value for the <see cref="SurfaceBase.ViewPort"/> view.</param>
        public BasicNoDraw(int width, int height, Rectangle renderArea, Cell[] initialCells): base(width, height, null, renderArea, initialCells)
        {
            Renderer = null;
            SetRenderCells();
        }

        /// <inheritdoc />
        public override void SetRenderCells()
        {
            if (RenderCells.Length != ViewPort.Width * ViewPort.Height)
            {
                //RenderRects = new Rectangle[ViewPort.Width * ViewPort.Height];
                RenderCells = new Cell[ViewPort.Width * ViewPort.Height];
            }

            int index = 0;

            for (int y = 0; y < ViewPort.Height; y++)
            {
                for (int x = 0; x < ViewPort.Width; x++)
                {
                    //RenderRects[index] = Font.GetRenderRect(x, y);
                    RenderCells[index] = Cells[(y + ViewPort.Top) * Width + (x + ViewPort.Left)];
                    index++;
                }
            }

            //AbsoluteArea = new Rectangle(0, 0, ViewPort.Width * Font.Size.X, ViewPort.Height * Font.Size.Y);
        }

        /// <summary>
        /// Does nothing for this class.
        /// </summary>
        /// <param name="timeElapsed"></param>
        public override void Draw(TimeSpan timeElapsed)
        {
            // Do nothing but draw the children.
            if (IsVisible)
            {
                var copyList = new List<ScreenObject>(Children);

                foreach (var child in copyList)
                    child.Draw(timeElapsed);
            }
        }

        /// <summary>
        /// Saves the <see cref="SurfaceBase"/> to a file.
        /// </summary>
        /// <param name="file">The destination file.</param>
        public void Save(string file) => Serializer.Save((SerializedTypes.BasicSurfaceSerialized)this, file, Settings.SerializationIsCompressed);

        /// <summary>
        /// Loads a <see cref="SurfaceBase"/> from a file.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <returns></returns>
        public static BasicNoDraw Load(string file) => Serializer.Load<SerializedTypes.BasicSurfaceSerialized>(file, Settings.SerializationIsCompressed);
    }
}
