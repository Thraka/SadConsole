using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Components;
using SadRogue.Primitives;

namespace SadConsole
{
    public partial class Console : ScreenObjectSurface
    {
        /// <summary>
        /// How the console should handle becoming active.
        /// </summary>
        public ActiveBehavior FocusedMode { get; set; }

        private bool _isCursorDisabled;

        /// <summary>
        /// When <see langword="true"/>, indicates that the <see cref="Cursor"/> cannot be used on this console; otherwise, <see langword="false"/>.
        /// </summary>
        /// <remarks>
        /// This property should only be used to indicate that this object can never use the <see cref="Cursor"/>. To simply disable or enable the <see cref="Cursor"/>, use <see cref="Cursor.IsEnabled"/> and <see cref="Cursor.IsVisible"/>.
        /// </remarks>
        public bool IsCursorDisabled { get; set; }

        /// <summary>
        /// The private virtual cursor reference.
        /// </summary>
        public Cursor Cursor { get; }

        /// <summary>
        /// Toggles the cursor as visible\hidden when the console if focused\unfocused.
        /// </summary>
        public bool AutoCursorOnFocus { get; set; }

        public Console(int width, int height): base(width, height)
        {
            Cursor = new Cursor(Surface);
        }

        public Console(int width, int height, ColoredGlyph[] initialCells) : base(width, height, initialCells)
        {
            Cursor = new Cursor(Surface);
        }

        protected override void OnVisibleChanged()
        {
            //if (!value && IsMouseOver)
            {
                //OnMouseExit(new Input.MouseConsoleState(this, Global.MouseState));
            }

            base.OnVisibleChanged();
        }

        /// <summary>
        /// Gets or sets this console as the focused console for input.
        /// </summary>
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
                    }
                    else if (!value && Global.FocusedConsoles.Console == this)
                        Global.FocusedConsoles.Pop(this);
                }
                else
                {
                    if (value)
                    {
                        if (FocusedMode == ActiveBehavior.Push)
                            Global.FocusedConsoles.Push(this);
                        else
                            Global.FocusedConsoles.Set(this);
                    }
                }
            }
        }

        ///  <inheritdoc/>
        public override void Update()
        {
            if (!IsEnabled) return;

            base.Update();

            if (!IsCursorDisabled && Cursor.IsVisible)
                Cursor.Update(Global.UpdateFrameDelta);
        }

        /// <summary>
        /// Called when this console's focus has been lost. Hides the <see cref="Cursor"/> if <see cref="AutoCursorOnFocus"/> is <see langword="true"/>.
        /// </summary>
        public virtual void OnFocusLost()
        {
            if (AutoCursorOnFocus)
                Cursor.IsVisible = false;
        }

        /// <summary>
        /// Called when this console is focused. Shows the <see cref="Cursor"/> if <see cref="AutoCursorOnFocus"/> is <see langword="true"/>.
        /// </summary>
        public virtual void OnFocused()
        {
            if (AutoCursorOnFocus)
                Cursor.IsVisible = true;
        }
    }
}
