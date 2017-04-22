using Microsoft.Xna.Framework;
using System.Collections.Generic;

using System.Runtime.Serialization;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace SadConsole.Surfaces
{
    /// <summary>
    /// A sub-view of an existing surface. Treated as it's own surface though, it shares the cell data with the original surface.
    /// </summary>
    [DataContract]
    public class SurfaceView : ISurface
    {
        protected Cell[] cells;

        [DataMember(Name = "Font")]
        protected Font font;
        protected Rectangle renderArea;
        [DataMember(Name = "ViewArea")]
        protected Rectangle viewArea;
        protected Rectangle[] renderRects;
        
        protected ISurface data;

        protected bool isDirty = true;

        /// <summary>
        /// Gets a cell by index.
        /// </summary>
        /// <param name="index">The index of the cell.</param>
        /// <returns>The indicated cell.</returns>
        public Cell this[int index]
        {
            get
            {
                return cells[index];
            }
        }

        /// <summary>
        /// Gets a cell based on its coordinates on the surface.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <returns>The indicated cell.</returns>
        public Cell this[int x, int y]
        {
            get
            {
                return cells[this.GetIndexFromPoint(x, y)];
            }
        }

        /// <summary>
        /// Total area in pixels of this surface view.
        /// </summary>
        public Rectangle AbsoluteArea { get; set; }

        /// <summary>
        /// All cells of the view.
        /// </summary>
        public Cell[] Cells
        {
            get
            {
                return cells;
            }
        }

        /// <summary>
        /// The default background color.
        /// </summary>
        [DataMember]
        public Color DefaultBackground { get; set; }

        /// <summary>
        /// The default foreground color.
        /// </summary>
        [DataMember]
        public Color DefaultForeground { get; set; }

        /// <summary>
        /// The font used for rendering.
        /// </summary>
        public Font Font
        {
            get
            {
                return font;
            }

            set
            {
                font = value;
                ResetArea();
            }
        }

        /// <summary>
        /// The height of the view.
        /// </summary>
        public int Height
        {
            get
            {
                return viewArea.Height;
            }
        }

        /// <summary>
        /// Internal only.
        /// </summary>
        public Rectangle RenderArea
        {
            get
            {
                return renderArea;
            }

            set
            {
            }
        }

        /// <summary>
        /// Cells that will be rendered.
        /// </summary>
        public Cell[] RenderCells
        {
            get
            {
                return cells;
            }
        }


        /// <summary>
        /// Destination rectangles for rendering.
        /// </summary>
        public Rectangle[] RenderRects
        {
            get
            {
                return renderRects;
            }
        }

        /// <summary>
        /// A tint used in rendering.
        /// </summary>
        [DataMember]
        public Color Tint { get; set; }

        /// <summary>
        /// The width of the view.
        /// </summary>
        public int Width
        {
            get
            {
                return viewArea.Width;
            }
        }

        /// <summary>
        /// The area of the original surface to use in this view.
        /// </summary>
        public Rectangle ViewArea
        {
            get { return viewArea; }
            set { viewArea = value; ResetArea(); }
        }

        /// <summary>
        /// Indicates the surface has changed and needs to be rendered.
        /// </summary>
        public bool IsDirty
        {
            get { return isDirty; }
            set
            {
                isDirty = value;

                if (value)
                {
                    OnIsDirty?.Invoke(this);

                    if (data != null)
                        data.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// The last texture render pass for this surface.
        /// </summary>
        public RenderTarget2D LastRenderResult { get; set; }

        /// <summary>
        /// A callback that happens when <see cref="IsDirty"/> is set to true.
        /// </summary>
        public Action<ISurface> OnIsDirty { get; set; }

        /// <summary>
        /// Creates a new surface view from an existing surface.
        /// </summary>
        /// <param name="surface">The source cell data.</param>
        /// <param name="area">The area of the text surface.</param>
        public SurfaceView(ISurface surface, Rectangle area)
        {
            data = surface;
            DefaultBackground = surface.DefaultBackground;
            DefaultForeground = surface.DefaultForeground;
            font = surface.Font;

            LastRenderResult = new RenderTarget2D(Global.GraphicsDevice, area.Width * font.Size.X, area.Height * font.Size.Y);
            ViewArea = area;
        }

        /// <summary>
        /// Disposes <see cref="LastRenderResult"/>.
        /// </summary>
        ~SurfaceView()
        {
            if (LastRenderResult != null)
                LastRenderResult.Dispose();
        }

        /// <summary>
        /// Keeps the text view data in sync with this surface.
        /// </summary>
        protected virtual void ResetArea()
        {
            if (data == null)
                return;

            if (viewArea.Left + viewArea.Width > data.Width || viewArea.Top + viewArea.Height > data.Height)
                throw new ArgumentOutOfRangeException("RenderArea", "Area is out of range of the surface");

            renderArea = new Rectangle(0, 0, viewArea.Width, viewArea.Height);
            renderRects = new Rectangle[viewArea.Width * viewArea.Height];
            cells = new Cell[viewArea.Width * viewArea.Height];

            int index = 0;

            for (int y = 0; y < viewArea.Height; y++)
            {
                for (int x = 0; x < viewArea.Width; x++)
                {
                    renderRects[index] = font.GetRenderRect(x, y);
                    RenderCells[index] = data[(y + viewArea.Top) * data.Width + (x + viewArea.Left)];
                    index++;
                }
            }

            cells = RenderCells;
            AbsoluteArea = new Rectangle(0, 0, viewArea.Width * font.Size.X, viewArea.Height * font.Size.Y);

            if (LastRenderResult.Bounds.Size != AbsoluteArea.Size)
            {
                LastRenderResult.Dispose();
                LastRenderResult = new RenderTarget2D(Global.GraphicsDevice, AbsoluteArea.Width, AbsoluteArea.Height, false, Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
            }
        }

        /// <summary>
        /// Call after the <see cref="TextSurfaceView"/> is deserialized to hook it back up to the original surface.
        /// </summary>
        /// <param name="surface">The surface to associate with the view.</param>
        public void Hydrate(ISurface surface)
        {
            data = surface;

            ResetArea();
        }

        /// <summary>
        /// Call after the <see cref="TextSurfaceView"/> is deserialized to hook it back up to the original surface.
        /// </summary>
        /// <param name="surface">The surface to associate with the view.</param>
        /// <param name="view">The sub view of the <paramref name="surface"/>.</param>
        public void Hydrate(ISurface surface, Rectangle view)
        {
            data = surface;
            viewArea = view;
            ResetArea();
        }
        /// <summary>
        /// Saves the serialized <see cref="TextSurfaceView"/> to a file.
        /// </summary>
        /// <param name="file">The destination file.</param>
        public void Save(string file)
        {
            SadConsole.Serializer.Save((SerializedTypes.SurfaceViewSerialized)this, file);
        }

        /// <summary>
        /// Loads a <see cref="TextSurfaceView"/> from a file and existing <see cref="ITextSurfaceRendered"/>.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <param name="surfaceHydrate">The surface this view was originally from.</param>
        /// <returns>A surface view.</returns>
        public static SurfaceView Load(string file, ISurface surfaceHydrate)
        {
            return Serializer.Load<SerializedTypes.SurfaceViewSerialized>(file).ToSurfaceView(surfaceHydrate);
        }
    }
}
