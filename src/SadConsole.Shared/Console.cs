using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Effects;
using SadConsole.Input;
using SadConsole.Surfaces;

namespace SadConsole
{
    [System.Diagnostics.DebuggerDisplay("Console")]
    public partial class Console: Surfaces.SurfaceBase, IConsole
    {
        /// <summary>
        /// How the console handles becoming <see cref="Global.InputTargets.Console"/>.
        /// </summary>
        [DataContract]
        public enum ActiveBehavior
        {
            /// <summary>
            /// Becomes the only active input object when focused.
            /// </summary>
            Set,

            /// <summary>
            /// Pushes to the top of the stack when it becomes the active input object.
            /// </summary>
            Push
        }

        /// <summary>
        /// The private virtual curser reference.
        /// </summary>
        public Cursor Cursor { get; private set; }

        /// <summary>
        /// Toggles the VirtualCursor as visible\hidden when the console if focused\unfocused.
        /// </summary>
        public bool AutoCursorOnFocus { get; set; }

        /// <summary>
        /// How the console should handle becoming active.
        /// </summary>
        public Console.ActiveBehavior FocusedMode { get; set; }

        /// <summary>
        /// Gets or sets whether or not this console has exclusive access to the mouse events.
        /// </summary>
        [DataMember]
        public bool IsExclusiveMouse { get; set; }

        /// <summary>
        /// Gets or sets this console as the focused console for input.
        /// </summary>
        [DataMember]
        public bool IsFocused
        {
            get => Global.FocusedConsoles.Console == this;
            set
            {
                if (Global.FocusedConsoles.Console != null)
                {
                    if (value && Global.FocusedConsoles.Console != this)
                    {
                        if (FocusedMode == ActiveBehavior.Push)
                            Global.FocusedConsoles.Push(this);
                        else
                            Global.FocusedConsoles.Set(this);

                        OnFocused();
                    }

                    else if (value && Global.FocusedConsoles.Console == this)
                        OnFocused();

                    else if (!value)
                    {
                        if (Global.FocusedConsoles.Console == this)
                            Global.FocusedConsoles.Pop(this);

                        OnFocusLost();
                    }
                }
                else
                {
                    if (value)
                    {
                        if (FocusedMode == ActiveBehavior.Push)
                            Global.FocusedConsoles.Push(this);
                        else
                            Global.FocusedConsoles.Set(this);

                        OnFocused();
                    }
                    else
                        OnFocusLost();
                }
            }
        }

        /// <inheritdoc />
        public new Cell this[int x, int y] => base[x, y];

        /// <inheritdoc />
        public new Cell this[int index] => base[index];

        /// <summary>
        /// Creates a new text surface with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">THe height of the surface.</param>
        public Console(int width, int height) : this(width, height, Global.FontDefault, new Rectangle(0, 0, width, height), null)
        {

        }

        /// <summary>
        /// Creates a new text surface with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="viewPort">Initial value for the <see cref="SurfaceBase.ViewPort"/> view.</param>
        public Console(int width, int height, Rectangle viewPort) : this(width, height, Global.FontDefault, viewPort, null)
        {

        }

        /// <summary>
        /// Creates a new text surface with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="font">The font used with rendering.</param>
        public Console(int width, int height, Font font) : this(width, height, font, new Rectangle(0, 0, width, height), null)
        {

        }

        /// <summary>
        /// Creates a new text surface with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="font">The font used with rendering.</param>
        /// <param name="viewPort">Initial value for the <see cref="SurfaceBase.ViewPort"/> view.</param>
        public Console(int width, int height, Font font, Rectangle viewPort) : this(width, height, font, viewPort, null)
        {

        }

        /// <summary>
        /// Creates a new text surface with the specified width, height, and initial set of cell data.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="font">The font used with rendering.</param>
        /// <param name="initialCells">Seeds the cells with existing values. Array size must match <paramref name="width"/> * <paramref name="height"/>.</param>
        /// <param name="viewPort">Initial value for the <see cref="SurfaceBase.ViewPort"/> view.</param>
        public Console(int width, int height, Font font, Rectangle viewPort, Cell[] initialCells): base (width, height, font, viewPort, initialCells)
        {
            Cursor = new Cursor(this);
            Renderer.BeforeRenderTintCallback = OnBeforeRender;
        }


        /// <summary>
        /// Called when the renderer renders the text view.
        /// </summary>
        /// <param name="batch">The batch used in renderering.</param>
        protected virtual void OnBeforeRender(SpriteBatch batch)
        {
            if (Cursor.IsVisible && ViewPort.Contains(Cursor.Position))
                Cursor.Render(batch, Font, Font.GetRenderRect(Cursor.Position.X - ViewPort.Location.X, Cursor.Position.Y - ViewPort.Location.Y));
        }

        /// <summary>
        /// Updates the <see cref="Cursor"/>.
        /// </summary>
        /// <param name="delta">Time difference for this frame (if update was called last frame).</param>
        public override void Update(TimeSpan delta)
        {
            if (IsPaused) return;

            if (Cursor.IsVisible)
                Cursor.Update(delta);

            base.Update(delta);
        }
        
        /// <summary>
        /// Called when this console's focus has been lost.
        /// </summary>
        protected virtual void OnFocusLost()
        {
            if (AutoCursorOnFocus)
                Cursor.IsVisible = false;
        }

        /// <summary>
        /// Called when this console is focused.
        /// </summary>
        protected virtual void OnFocused()
        {
            if (AutoCursorOnFocus)
                Cursor.IsVisible = true;
        }

        /// <summary>
        /// Creates a new console from an existing surface.
        /// </summary>
        /// <param name="surface"></param>
        /// <returns>A new console.</returns>
        public static Console FromSurface(SurfaceBase surface)
        {
            return new Console(surface.Width, surface.Height, surface.Font, surface.ViewPort, surface.Cells);
        }
    }
}
