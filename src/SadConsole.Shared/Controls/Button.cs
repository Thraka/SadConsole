using Microsoft.Xna.Framework.Input;

using SadConsole.Themes;
using System;
using System.Runtime.Serialization;

namespace SadConsole.Controls
{
    /// <summary>
    /// Base class for creating a button type control.
    /// </summary>
    /// <typeparam name="TTheme"></typeparam>
    [DataContract]
    public abstract class ButtonBase<TTheme>: ControlBase
        where TTheme : ButtonTheme
    {
        /// <summary>
        /// The theme override for the button.
        /// </summary>
        [DataMember(Name = "Theme")]
        protected TTheme theme;

        /// <summary>
        /// The default theme if a theme is not set for the button.
        /// </summary>
        protected TTheme defaultTheme;

        /// <summary>
        /// Raised when the button is clicked.
        /// </summary>
        public event EventHandler Click;

        /// <summary>
        /// True when the mouse is down.
        /// </summary>
        protected bool isMouseDown;

        /// <summary>
        /// The display text of the button.
        /// </summary>
        [DataMember(Name = "Text")]
        protected string text;

        /// <summary>
        /// The alignment of the <see cref="text"/>.
        /// </summary>
        [DataMember(Name = "TextAlignment")]
        protected System.Windows.HorizontalAlignment textAlignment = System.Windows.HorizontalAlignment.Center;

        /// <summary>
        /// Selected part of the theme based on the state of the control.
        /// </summary>
        protected Cell currentAppearance;

        /// <summary>
        /// The text displayed on the control.
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; Compose(true); }
        }

        /// <summary>
        /// The alignment of the text, left, center, or right.
        /// </summary>
        public System.Windows.HorizontalAlignment TextAlignment
        {
            get { return textAlignment; }
            set { textAlignment = value; Compose(true); }
        }

        /// <summary>
        /// The theme of this control. If the theme is not explicitly set, the theme is taken from the library.
        /// </summary>
        public virtual TTheme Theme
        {
            get
            {
                if (theme == null)
                    return defaultTheme;
                else
                    return theme;
            }
            set
            {
                theme = value;
            }
        }

        /// <summary>
        /// Creates a new button control.
        /// </summary>
        /// <param name="width">Width of the button.</param>
        /// <param name="height">Height of the button.</param>
        /// <param name="defaultTheme">The default theme, cannot be null.</param>
        public ButtonBase(int width, int height, TTheme defaultTheme): base(width, height) { this.defaultTheme = defaultTheme; }

        /// <summary>
        /// Raises the <see cref="Click"/> event.
        /// </summary>
        public virtual void DoClick()
        {
            Click?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Sets the appearance of the control depending on the current state of the control.
        /// </summary>
        public override void DetermineAppearance()
        {
            Cell currentappearance = currentAppearance;

            if (!isEnabled)
                currentAppearance = Theme.Disabled;

            else if (!isMouseDown && isMouseOver)
                currentAppearance = Theme.MouseOver;

            else if (!isMouseDown && !isMouseOver && IsFocused && Global.FocusedConsoles.Console == parent)
                currentAppearance = Theme.Focused;

            else if (isMouseDown && isMouseOver)
                currentAppearance = Theme.MouseClicking;

            else
                currentAppearance = Theme.Normal;

            if (currentappearance != currentAppearance)
                IsDirty = true;
        }

        /// <summary>
        /// Detects if the SPACE and ENTER keys are pressed and calls the <see cref="Click"/> method.
        /// </summary>
        /// <param name="info"></param>
        public override bool ProcessKeyboard(Input.Keyboard info)
        {
            if (info.IsKeyReleased(Keys.Space) || info.IsKeyReleased(Keys.Enter))
            {
                DoClick();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Called when the mouse is in the control area.
        /// </summary>
        /// <param name="state">The mouse state.</param>
        protected override void OnMouseIn(Input.MouseConsoleState state)
        {
            isMouseDown = state.Mouse.LeftButtonDown;

            base.OnMouseIn(state);
        }

        /// <summary>
        /// Called when the mouse leaves the control area.
        /// </summary>
        /// <param name="state">The mouse state.</param>
        protected override void OnMouseExit(Input.MouseConsoleState state)
        {
            isMouseDown = false;

            base.OnMouseExit(state);
        }

        /// <summary>
        /// Called when the left-mouse button is clicked.
        /// </summary>
        /// <param name="state">The mouse state.</param>
        protected override void OnLeftMouseClicked(Input.MouseConsoleState state)
        {
            base.OnLeftMouseClicked(state);

            if (isEnabled)
                DoClick();
        }
    }

    /// <summary>
    /// Simple button control with a height of 1.
    /// </summary>
    [DataContract]
    public class Button: ButtonBase<ButtonTheme>
    {
        /// <summary>
        /// When true, renders the <see cref="EndCharacterLeft"/> and <see cref="EndCharacterRight"/> on the button.
        /// </summary>
        [DataMember]
        public bool ShowEnds { get; set; } = true;

        /// <summary>
        /// The character on the left side of the button. Defaults to '&lt;'.
        /// </summary>
        [DataMember]
        public int EndCharacterLeft { get; set; } = (int)'<';

        /// <summary>
        /// The character on the right side of the button. Defaults to '>'.
        /// </summary>
        [DataMember]
        public int EndCharacterRight { get; set; } = (int)'>';

        /// <summary>
        /// Creates an instance of the button control with the specified width.
        /// </summary>
        /// <param name="width">Width of the control.</param>
        public Button(int width)
            : base(width, 1, Themes.Library.Default.ButtonTheme)
        {
            DetermineAppearance();
        }

        /// <summary>
        /// Creates an instance of the button control with the specified width.
        /// </summary>
        /// <param name="width">Width of the control.</param>
        /// <param name="height">Height of the control.</param>
        public Button(int width, int height)
            :base(width, height, Themes.Library.Default.ButtonTheme)
        {
            DetermineAppearance();
        }

        /// <summary>
        /// Draws the control.
        /// </summary>
        public override void Compose()
        {
            if (this.IsDirty)
            {
                // Redraw the control
                this.Fill(currentAppearance.Foreground, currentAppearance.Background, currentAppearance.Glyph, null);

                if (ShowEnds)
                {
                    this.Print(1, 0, (Text).Align(TextAlignment, this.TextSurface.Width - 2));
                    SetGlyph(0, 0, EndCharacterLeft);
                    SetGlyph(this.TextSurface.Width - 1, 0, EndCharacterRight);
                }
                else
                    this.Print(0, 0, (Text).Align(TextAlignment, this.TextSurface.Width));

                OnComposed?.Invoke(this);
                this.IsDirty = false;
            }
        }

        [OnDeserialized]
        private void AfterDeserialized(StreamingContext context)
        {
            DetermineAppearance();
            Compose(true);
        }
    }
}
