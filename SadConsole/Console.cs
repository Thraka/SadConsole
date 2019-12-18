using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Components;
using SadConsole.Input;
using SadRogue.Primitives;

namespace SadConsole
{
    /// <summary>
    /// A surface that has a cursor.
    /// </summary>
    public partial class Console: ScreenSurface
    {
        private bool _isCursorDisabled;

        /// <summary>
        /// When <see langword="true"/>, indicates that the <see cref="Cursor"/> cannot be used on this console; otherwise, <see langword="false"/>.
        /// </summary>
        /// <remarks>
        /// This property should only be used to indicate that this object can never use the <see cref="Cursor"/>. To simply disable or enable the <see cref="Cursor"/>, use <see cref="Cursor.IsEnabled"/> and <see cref="Cursor.IsVisible"/>.
        /// </remarks>
        public bool IsCursorDisabled
        {
            get => _isCursorDisabled;
            set { _isCursorDisabled = value; IsDirty = true; }
        }

        /// <summary>
        /// The entire width of the console. Forwards <see cref="ICellSurface.BufferWidth"/>.
        /// </summary>
        public int Width =>
            BufferWidth;

        /// <summary>
        /// The entire height of the console. Forwards <see cref="ICellSurface.BufferHeight"/>.
        /// </summary>
        public int Height =>
            BufferHeight;

        /// <summary>
        /// The private virtual cursor reference.
        /// </summary>
        public Cursor Cursor { get; }

        /// <summary>
        /// Toggles the cursor as visible\hidden when the console if focused\unfocused.
        /// </summary>
        public bool AutoCursorOnFocus { get; set; }

        /// <summary>
        /// Creates a new console.
        /// </summary>
        /// <param name="width">The width in cells of the surface.</param>
        /// <param name="height">The height in cells of the surface.</param>
        public Console(int width, int height) : this(width, height, width, height, null) { }

        /// <summary>
        /// Creates a new screen object that can render a surface. Uses the specified cells to generate the surface.
        /// </summary>
        /// <param name="width">The width in cells of the surface.</param>
        /// <param name="height">The height in cells of the surface.</param>
        /// <param name="initialCells">The initial cells to seed the surface.</param>
        public Console(int width, int height, ColoredGlyph[] initialCells) : this(width, height, width, height, initialCells) { }

        /// <summary>
        /// Creates a new console with the specified width and height, with <see cref="Color.Transparent"/> for the background and <see cref="Color.White"/> for the foreground.
        /// </summary>
        /// <param name="width">The visible width of the console in cells.</param>
        /// <param name="height">The visible height of the console in cells.</param>
        /// <param name="bufferWidth">The total width of the console in cells.</param>
        /// <param name="bufferHeight">The total height of the console in cells.</param>
        public Console(int width, int height, int bufferWidth, int bufferHeight) : this(width, height, bufferWidth, bufferHeight, null) { }

        /// <summary>
        /// Creates a new console using the specified surface's cells.
        /// </summary>
        /// <param name="surface">The surface.</param>
        public Console(ICellSurface surface) : this(surface.View.Width, surface.View.Height, surface.BufferWidth, surface.BufferHeight, surface.Cells) { }

        /// <summary>
        /// Creates a console with the specified width and height, with <see cref="Color.Transparent"/> for the background and <see cref="Color.White"/> for the foreground.
        /// </summary>
        /// <param name="width">The width of the console in cells.</param>
        /// <param name="height">The height of the console in cells.</param>
        /// <param name="bufferWidth">The total width of the console in cells.</param>
        /// <param name="bufferHeight">The total height of the console in cells.</param>
        /// <param name="initialCells">The cells to seed the console with. If <see langword="null"/>, creates the cells for you.</param>
        public Console(int width, int height, int bufferWidth, int bufferHeight, ColoredGlyph[] initialCells) : base(width, height, bufferWidth, bufferHeight, initialCells)
        {
            Cursor = new Cursor(Surface);
            UseKeyboard = Settings.DefaultConsoleUseKeyboard;
        }
        
        /// <inheritdoc/>
        protected override void OnVisibleChanged()
        {
            if (!IsVisible && IsMouseOver)
                OnMouseExit(new Input.MouseScreenObjectState(this, SadConsole.Global.Mouse));

            base.OnVisibleChanged();
        }

        ///  <inheritdoc/>
        public override void Update()
        {
            if (!IsEnabled) return;

            base.Update();

            if (!IsCursorDisabled && Cursor.IsVisible)
                Cursor.Update(Global.UpdateFrameDelta);
        }

        /// <inheritdoc />
        public override bool ProcessKeyboard(Keyboard keyboard)
        {
            if (!UseKeyboard) return false;
            else if (base.ProcessKeyboard(keyboard)) return true;

            return !IsCursorDisabled && Cursor.IsEnabled && Cursor.ProcessKeyboard(keyboard);
        }

        /// <summary>
        /// Called when this console's focus has been lost. Hides the <see cref="Cursor"/> if <see cref="AutoCursorOnFocus"/> is <see langword="true"/>.
        /// </summary>
        public override void OnFocusLost()
        {
            if (AutoCursorOnFocus)
                Cursor.IsVisible = false;
        }

        /// <summary>
        /// Called when this console is focused. Shows the <see cref="Cursor"/> if <see cref="AutoCursorOnFocus"/> is <see langword="true"/>.
        /// </summary>
        public override void OnFocused()
        {
            if (AutoCursorOnFocus)
                Cursor.IsVisible = true;
        }

        
    }
}
