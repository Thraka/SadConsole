#if SFML
using Rectangle = SFML.Graphics.IntRect;
using Point = SFML.System.Vector2i;
using SFML.Graphics;
#elif MONOGAME
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endif

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SadConsole.Consoles
{
    /// <summary>
    /// The base class for a text surface. Provides code for the view port and basic cell access.
    /// </summary>
    [DataContract]
    public class TextSurface : TextSurfaceBasic, IEnumerable<Cell>, ITextSurfaceRendered
    {
        [DataMember(Name = "Font")]
        protected Font font;

        [DataMember(Name = "Area")]
        protected Rectangle area;
        
        /// <summary>
        /// The total cells for this surface.
        /// </summary>
        public int CellCount { get { return cells.Length; } }
        
        /// <summary>
        /// Font used with rendering.
        /// </summary>
        public Font Font { get { return font; } set { font = value; OnFontChanged(); } }

        #region ITextSurfaceView
        /// <summary>
        /// Pixel area of the render cells.
        /// </summary>
        public Rectangle AbsoluteArea { get; set; }

        /// <summary>
        /// Destination rectangles for rendering.
        /// </summary>
        public Rectangle[] RenderRects { get; set; }
        
        /// <summary>
        /// Cells that will be rendered.
        /// </summary>
        public Cell[] RenderCells { get; set; }

        /// <summary>
        /// A tint used in rendering.
        /// </summary>
        [DataMember]
        public Color Tint { get; set; } = Color.Transparent;

        /// <summary>
        /// Sets the area of the text surface that should be rendered.
        /// </summary>
        public Rectangle RenderArea
        {
            get { return area; }
            set
            {
                area = value;

                if (area == null)
                    area = new Rectangle(0, 0, width, height);
#if SFML
                if (area.Width > width)
                    area.Width = width;
                if (area.Height > height)
                    area.Height = height;

                if (area.Left < 0)
                    area.Left = 0;
                if (area.Top < 0)
                    area.Top = 0;

                if (area.Left + area.Width > width)
                    area.Left = width - area.Width;
                if (area.Top + area.Height > height)
                    area.Top = height - area.Height;
#elif MONOGAME
                if (area.Width > width)
                    area.Width = width;
                if (area.Height > height)
                    area.Height = height;

                if (area.X < 0)
                    area.X = 0;
                if (area.Y < 0)
                    area.Y = 0;

                if (area.X + area.Width > width)
                    area.X = width - area.Width;
                if (area.Y + area.Height > height)
                    area.Y = height - area.Height;
#endif

                ResetArea();
            }
        }
        #endregion

        /// <summary>
        /// Creates a new text surface with the specified width and height and <see cref="Engine.DefaultFont"/>.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        public TextSurface(int width, int height) : this(width, height, Engine.DefaultFont)
        {
        }

        /// <summary>
        /// Creates a new text surface with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="font">The font used with rendering.</param>
        public TextSurface(int width, int height, Font font): base(width, height)
        {
            area = new Rectangle(0, 0, width, height);
            Font = font;
        }

        /// <summary>
        /// Creates a new text surface with the specified width, height, and initial set of cell data. Uses <see cref="Engine.DefaultFont"/>.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="initialCells"></param>
        public TextSurface(int width, int height, Cell[] initialCells) : this(width, height, initialCells, Engine.DefaultFont)
        {
        }

        /// <summary>
        /// Creates a new text surface with the specified width, height, and initial set of cell data.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="font">The font used with rendering.</param>
        /// <param name="initialCells"></param>
        public TextSurface(int width, int height, Cell[] initialCells, Font font) : base(width, height, initialCells)
        {
            area = new Rectangle(0, 0, width, height);
            Font = font;
        }

        /// <summary>
        /// Sets <see cref="RenderCells"/> to <see cref="TextSurfaceBasic.cells"/>.
        /// </summary>
        protected override void InitializeCells()
        {
            base.InitializeCells();

            RenderCells = cells;
        }

        /// <summary>
        /// Keeps the text view data in sync with this surface.
        /// </summary>
        protected virtual void ResetArea()
        {
            RenderRects = new Rectangle[area.Width * area.Height];
            RenderCells = new Cell[area.Width * area.Height];

            int index = 0;

            for (int y = 0; y < area.Height; y++)
            {
                for (int x = 0; x < area.Width; x++)
                {
                    RenderRects[index] = font.GetRenderRect(x, y);
                    RenderCells[index] = cells[(y + area.Top) * width + (x + area.Left)];
                    index++;
                }
            }

            // TODO: Optimization by calculating AbsArea and seeing if it's diff from current, if so, don't create new RenderRects
            AbsoluteArea = new Rectangle(0, 0, area.Width * font.Size.X, area.Height * font.Size.Y);
        }

        protected virtual void OnFontChanged()
        {
            ResetArea();
        }


#region Static Methods


        public static int GetIndexFromPoint(Point location, int width)
        {
            return location.Y * width + location.X;
        }

        

        public static int GetIndexFromPoint(int x, int y, int width)
        {
            return y * width + x;
        }
        
        public static Point GetPointFromIndex(int index, int width)
        {
            return new Point(index % width, index / width);
        }
        
#endregion
        
        public IEnumerator<Cell> GetEnumerator()
        {
            return ((IEnumerable<Cell>)cells).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return cells.GetEnumerator();
        }

        /// <summary>
        /// Saves the <see cref="TextSurface"/> to a file.
        /// </summary>
        /// <param name="file">The destination file.</param>
        public void Save(string file)
        {
            Serializer.Save(this, file);
        }

        /// <summary>
        /// Loads a <see cref="TextSurface"/> from a file.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <returns></returns>
        public static TextSurface Load(string file)
        {
            return Serializer.Load<TextSurface>(file);
        }

        //#region Serialization
        //        /// <summary>
        //        /// Saves the <see cref="TextSurface"/> to a file.
        //        /// </summary>
        //        /// <param name="file">The destination file.</param>
        //        public void Save(string file)
        //        {
        //            Serializer.Save(this, file);
        //        }

        //        /// <summary>
        //        /// Loads a <see cref="TextSurface"/> from a file.
        //        /// </summary>
        //        /// <param name="file">The source file.</param>
        //        /// <returns></returns>
        //        public static TextSurface Load(string file)
        //        {
        //            return Serializer.Load<TextSurface>(file);
        //        }

        //        [OnSerializing]
        //        private void BeforeSerializing(StreamingContext context)
        //        {
        //            fontName = Font.Name;
        //            fontSize = Font.SizeMultiple;
        //        }

        //        [OnDeserialized]
        //        private void AfterDeserialized(StreamingContext context)
        //        {
        //            Font font;

        //            // Try to find font
        //            if (Engine.Fonts.ContainsKey(fontName))
        //                font = Engine.Fonts[fontName].GetFont(fontSize);
        //            else
        //                font = Engine.DefaultFont;

        //            Font = font;
        //        }
        //#endregion
    }
}
