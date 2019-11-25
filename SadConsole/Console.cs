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
    public class Console: ScreenObjectSurface
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
        /// The buffer width of the console. Forwards <see cref="CellSurface.BufferWidth"/>.
        /// </summary>
        public int Width =>
            base.BufferWidth;

        /// <summary>
        /// The buffer height of the console. Forwards <see cref="CellSurface.BufferHeight"/>.
        /// </summary>
        public int Height =>
            base.BufferHeight;

        /// <summary>
        /// The private virtual cursor reference.
        /// </summary>
        public Cursor Cursor { get; }

        /// <summary>
        /// Toggles the cursor as visible\hidden when the console if focused\unfocused.
        /// </summary>
        public bool AutoCursorOnFocus { get; set; }

        /// <summary>
        /// Creates a new console with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the console.</param>
        /// <param name="height">The height of the console.</param>
        public Console(int width, int height): base(width, height)
        {
            Cursor = new Cursor(this);
            UseKeyboard = Settings.DefaultConsoleUseKeyboard;
        }

        /// <summary>
        /// Creates a new console with the specified width and height and an initial set of cells.
        /// </summary>
        /// <param name="width">The width of the console.</param>
        /// <param name="height">The height of the console.</param>
        /// <param name="initialCells">The cells to seed the cosnole.</param>
        public Console(int width, int height, ColoredGlyph[] initialCells) : base(width, height, initialCells)
        {
            Cursor = new Cursor(this);
            UseKeyboard = Settings.DefaultConsoleUseKeyboard;
        }

        /// <summary>
        /// Creates a new console with the specified <see cref="CellSurface"/> type.
        /// </summary>
        /// <param name="surface">The surface to use for this console.</param>
        public Console(CellSurface surface) : base(surface)
        {
            Cursor = new Cursor(this);
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
