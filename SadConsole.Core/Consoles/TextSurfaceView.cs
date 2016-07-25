#if SFML
using Rectangle = SFML.Graphics.IntRect;
using SFML.Graphics;
#else
using Microsoft.Xna.Framework;
#endif

using System.Runtime.Serialization;

namespace SadConsole.Consoles
{
    /// <summary>
    /// A text surface created from an existing text surface.
    /// </summary>
    public class TextSurfaceView : TextSurface
    {
        private ITextSurfaceRendered data;
        protected Rectangle originalArea;
        
        /// <summary>
        /// Creates a new surface view from an existing surface.
        /// </summary>
        /// <param name="surface">The source cell data.</param>
        /// <param name="area">The area of the text surface.</param>
        public TextSurfaceView(ITextSurfaceRendered surface, Rectangle area): base(area.Width, area.Height, surface.Font)
        {
            data = surface;
            DefaultBackground = surface.DefaultBackground;
            DefaultForeground = surface.DefaultForeground;
            base.font = surface.Font;
            base.width = surface.Width;
            base.height = surface.Height;

            this.originalArea = area;
            this.cells = data.Cells;

            base.width = area.Width;
            base.height = area.Height;

            RenderRects = new Rectangle[area.Width * area.Height];
            RenderCells = new Cell[area.Width * area.Height];

            int index = 0;

            for (int y = 0; y < area.Height; y++)
            {
                for (int x = 0; x < area.Width; x++)
                {
                    RenderRects[index] = new Rectangle(x * Font.Size.X, y * Font.Size.Y, Font.Size.X, Font.Size.Y);
                    RenderCells[index] = base.cells[(y + area.Top) * surface.Width + (x + area.Left)];
                    index++;
                }
            }

            cells = RenderCells;

            AbsoluteArea = new Rectangle(0, 0, area.Width * Font.Size.X, area.Height * Font.Size.Y);
        }

        #region Serialization
        /// <summary>
        /// Saves the <see cref="TextSurfaceView"/> to a file.
        /// </summary>
        /// <param name="file">The destination file.</param>
        public new void Save(string file)
        {
            new Serialized(this).Save(file);
        }

        /// <summary>
        /// Loads a <see cref="TextSurfaceView"/> from a file.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <param name="surfaceBase">The surface this view was created from.</param>
        /// <returns></returns>
        public static TextSurfaceView Load(string file, ITextSurfaceRendered surfaceBase)
        {
            return Serialized.Load(file, surfaceBase);
        }

        /// <summary>
        /// Serialized instance of a <see cref="TextSurface"/>.
        /// </summary>
        [DataContract]
        public new class Serialized
        {
            [DataMember]
            public Rectangle Area;

            [DataMember]
            public string FontName;

            [DataMember]
            public Font.FontSizes FontMultiple;

            [DataMember]
            public Color DefaultBackground;

            [DataMember]
            public Color DefaultForeground;

            [DataMember]
            public Color Tint;

            /// <summary>
            /// Creates a serialized object from an existing <see cref="TextSurfaceView"/>.
            /// </summary>
            /// <param name="surface">The surface to serialize.</param>
            public Serialized(TextSurfaceView surfaceView)
            {
                Area = surfaceView.originalArea;
                FontName = surfaceView.font.Name;
                FontMultiple = surfaceView.font.SizeMultiple;
                DefaultBackground = surfaceView.DefaultBackground;
                DefaultForeground = surfaceView.DefaultForeground;
                Tint = surfaceView.Tint;
            }

            protected Serialized() { }

            /// <summary>
            /// Saves the serialized <see cref="TextSurfaceView"/> to a file.
            /// </summary>
            /// <param name="file">The destination file.</param>
            public void Save(string file)
            {
                SadConsole.Serializer.Save(this, file);
            }

            /// <summary>
            /// Loads a <see cref="TextSurfaceView"/> from a file and existing <see cref="ITextSurfaceRendered"/>.
            /// </summary>
            /// <param name="file">The source file.</param>
            /// <param name="surfaceHydrate">The surface this view was originally from.</param>
            /// <returns>A surface view.</returns>
            public static TextSurfaceView Load(string file, ITextSurfaceRendered surfaceHydrate)
            {
                Serialized data = Serializer.Load<Serialized>(file);
                Font font;
                // Try to find font
                if (Engine.Fonts.ContainsKey(data.FontName))
                    font = Engine.Fonts[data.FontName].GetFont(data.FontMultiple);
                else
                    font = Engine.DefaultFont;

                TextSurfaceView newSurface = new TextSurfaceView(surfaceHydrate, data.Area);
                newSurface.Font = font;
                newSurface.DefaultBackground = data.DefaultBackground;
                newSurface.DefaultForeground = data.DefaultForeground;
                newSurface.Tint = data.Tint;

                return newSurface;
            }
        }
        #endregion
    }
}
