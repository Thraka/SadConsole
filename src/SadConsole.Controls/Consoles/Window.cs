#if SFML
using Point = SFML.System.Vector2i;
using Keys = SFML.Window.Keyboard.Key;
#elif MONOGAME
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
#endif

using System;
using System.Windows;
using System.Runtime.Serialization;

namespace SadConsole.Consoles
{
    [DataContract]
    public partial class Window: ControlsConsole
    {
        /// <summary>
        /// Raised when the window is closed.
        /// </summary>
        public event EventHandler Closed;

        [DataMember(Name="IsModal")]
        private bool _isModal;
        [DataMember(Name="Title")]
        private string _title;
        [DataMember(Name="Theme")]
        private SadConsole.Themes.WindowTheme _theme;
        private bool _isDragging;
        [DataMember(Name = "TitleLocX")]
        private int _titleLocationX;
        [DataMember(Name = "TitleWidth")]
        private int _titleWidth;
        [DataMember(Name="TitleAlignment")]
        private HorizontalAlignment _titleAlignment;
        private SadConsole.Input.MouseInfo _previousMouseInfo = new Input.MouseInfo();
        private Point _consoleAtDragAbsPos;
        //private bool __isVisible;
        private bool _prevousMouseExclusiveDrag;
        private bool _addedToParent;
        private IConsole _previousActiveConsole;
        private SadConsole.Shapes.Box _border;

        /// <summary>
        /// Gets the whether or not the console is being shown as modal. 
        /// </summary>
        public bool IsModal { get { return _isModal; } }
        /// <summary>
        /// The <see cref="SadConsole.Shapes.Box"/> object used to draw the window border.
        /// </summary>
        [DataMember]
        public SadConsole.Shapes.Box Border { get { return _border; } set { _border = value; } }
        /// <summary>
        /// When true, indiciates that the window should be redrawn.
        /// </summary>
        public bool IsDirty { get; set; }
        /// <summary>
        /// Gets or sets whether or not this window can be moved with the mouse.
        /// </summary>
        [DataMember]
        public bool Dragable { get; set; }
        /// <summary>
        /// Gets or sets whether or not this window can be closed when the escape key is pressed.
        /// </summary>
        [DataMember]
        public bool CloseOnESC { get; set; }
        /// <summary>
        /// Gets or set the dialog result status of the window.
        /// </summary>
        [DataMember]
        public bool DialogResult { get; set; }
        
        /// <summary>
        /// Indicates that when this window is shown by the <see cref="Show()"/> method or by setting the <see cref="_isVisible"/> property to true, the window will be shown as modal.
        /// </summary>
        [DataMember]
        public bool ModalIsDefault { get; set; }

        /// <summary>
        /// Gets or sets the alignment of the window title.
        /// </summary>
        public HorizontalAlignment TitleAlignment
        {
            get { return _titleAlignment; }
            set { _titleAlignment = value; Redraw(); }
        }

        /// <summary>
        /// Gets or sets the title displayed on the window.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                Redraw();
            }
        }

        /// <summary>
        /// Gets or sets the theme of the window.
        /// </summary>
        public SadConsole.Themes.WindowTheme Theme
        {
            get
            {
                if (_theme == null)
                    return SadConsole.Themes.Library.Default.WindowTheme;
                else
                    return _theme;
            }
            set
            {
                _theme = value;

                ResetBox();
            }
        }

        private void ResetBox()
        {
            _border.Foreground = Theme.BorderStyle.Foreground;
            _border.BorderBackground = Theme.BorderStyle.Background;
        }

        #region Constructors
        public Window(int width, int height)
            : base(width, height)
        {
            updateKeyboardState = false;
            _border = Shapes.Box.GetDefaultBox();
            _border.Width = width;
            _border.Height = height;
            _isVisible = false;

            ProcessMouseWithoutFocus = true;
            CanFocus = true;
            MouseCanFocus = true;

            Renderer = new WindowRenderer();

            IsDirty = true;
            Redraw();
        }
        #endregion

        #region Base Methods
        protected override void OnVisibleChanged()
        {
            base.OnVisibleChanged();

            if (_isVisible)
                Show(ModalIsDefault);
            else
                Hide();
        }

        public override void Render()
        {
            //TODO: Perf - cache reference?
            ((WindowRenderer)_renderer).IsModal = _isModal;
            ((WindowRenderer)_renderer).ModalTint = Theme.ModalTint;

            base.Render();
        }

        /// <summary>
        /// Processes the mouse. If allowed, will move the console around with the mouse.
        /// </summary>
        /// <param name="info">The mouse state.</param>
        /// <returns></returns>
        public override bool ProcessMouse(Input.MouseInfo info)
        {
            if (_titleWidth != 0 && _isVisible)
            {
                if (!SkipMouseDataFill)
                    info.Fill(this);

                if (_isDragging && info.LeftButtonDown)
                {
                    if (base.UsePixelPositioning)
                        Position = new Point(info.ScreenLocation.X - (_previousMouseInfo.ScreenLocation.X - _consoleAtDragAbsPos.X), info.ScreenLocation.Y - (_previousMouseInfo.ScreenLocation.Y - _consoleAtDragAbsPos.Y));
                    else
                        Position = new Point(info.WorldLocation.X - _previousMouseInfo.ConsoleLocation.X, info.WorldLocation.Y - _previousMouseInfo.ConsoleLocation.Y);

                    return true;
                }

                // Stopped dragging
                if (_isDragging && !info.LeftButtonDown)
                {
                    _isDragging = false;
                    ExclusiveFocus = _prevousMouseExclusiveDrag;
                    return true;
                }

                // Left button freshly down and we're not already dragging, check to see if in title
                if (!_isDragging && !_previousMouseInfo.LeftButtonDown && info.LeftButtonDown)
                {
                    if (info.ConsoleLocation.Y == 0 && info.ConsoleLocation.X >= _titleLocationX && info.ConsoleLocation.X < _titleLocationX + _titleWidth)
                    {
                        _prevousMouseExclusiveDrag = ExclusiveFocus;

                        // Mouse is in the title bar
                        ExclusiveFocus = true;
                        _isDragging = true;
                        _consoleAtDragAbsPos = base.Position;

                        if (this.MouseCanFocus)
                        {
                            if (Engine.ActiveConsole != this)
                                Engine.ActiveConsole = this;

                            if (this.Parent != null && this.Parent.IndexOf(this) != this.Parent.Count - 1)
                                this.Parent.MoveToTop(this);
                        }
                    }
                }

                _previousMouseInfo = info.Clone();
            }

            return base.ProcessMouse(info);
        }

        /// <summary>
        /// Processes the keyboard looking for the ESC key press to close the console, if required. Otherwise the base ControlsConsole will process the keyboard.
        /// </summary>
        /// <param name="info">Keyboard state.</param>
        public override bool ProcessKeyboard(Input.KeyboardInfo info)
        {
            info = KeyboardState;
            info.ProcessKeys(Engine.GameTimeUpdate);

            if (CloseOnESC && info.IsKeyReleased(Keys.Escape))
            {
                this.Hide();
                return true;
            }
            else
                return base.ProcessKeyboard(info);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Displays this window using the modal value of the <see cref="ModalIsDefault"/> property.
        /// </summary>
        public void Show()
        {
            Show(ModalIsDefault);
        }

        /// <summary>
        /// Displays this window.
        /// </summary>
        /// <param name="modal">When true, the window will be displayed as modal; otherwise false.</param>
        public virtual void Show(bool modal)
        {
            if (Parent != null && _isVisible == true)
            {
                Parent.MoveToTop(this);
                return;
            }

            _isModal = modal;
            _isVisible = true;
            _addedToParent = false;

            if (Parent == null)
            {
                Engine.ConsoleRenderStack.Add(this);
                _addedToParent = true;
            }

            Parent.MoveToTop(this);

            if (modal)
            {
                _previousActiveConsole = Engine.ActiveConsole;
                Engine.ActiveConsole = this;
            }

            this.ExclusiveFocus = modal;
        }

        /// <summary>
        /// Hides the window.
        /// </summary>
        public virtual void Hide()
        {
            _isVisible = false;

            if (Engine.ActiveConsole == this)
            {
                if (_isModal)
                    Engine.ActiveConsole = _previousActiveConsole;
            }

            if (_addedToParent && Parent != null)
                Parent.Remove(this);

            if (Closed != null)
                Closed(this, new EventArgs());
        }

        /// <summary>
        /// Centers the window according to the <see cref="SadConsole.Engine.Device.Viewport"/> size.
        /// </summary>
        public void Center()
        {
#if SFML
            int screenWidth = (int)SadConsole.Engine.Device.Size.X;
            int screenHeight = (int)SadConsole.Engine.Device.Size.Y;
#elif MONOGAME
            int screenWidth = SadConsole.Engine.Device.PresentationParameters.BackBufferWidth;
            int screenHeight = SadConsole.Engine.Device.PresentationParameters.BackBufferHeight;
#endif
            if (UsePixelPositioning)
                this.Position = new Point((screenWidth / 2) - ((textSurface.Width * textSurface.Font.Size.X) / 2), (screenHeight / 2) - ((textSurface.Height * textSurface.Font.Size.Y) / 2));
            else
                this.Position = new Point(((screenWidth / textSurface.Font.Size.X) / 2) - (textSurface.Width / 2), ((screenHeight / textSurface.Font.Size.Y) / 2) - (textSurface.Height / 2));
            
        }

        /// <summary>
        /// Redraws the border and title of the window.
        /// </summary>
        public virtual void Redraw()
        {
            textSurface.DefaultForeground = Theme.FillStyle.Foreground;
            textSurface.DefaultBackground = Theme.FillStyle.Background;

            Clear();

            ResetBox();
            Border.Draw(this);

            // Draw title
            string adjustedText = "";
            _titleWidth = 0;
            _titleLocationX = 0;
            int adjustedWidth = textSurface.Width - 2;

            if (!string.IsNullOrEmpty(_title))
            {
                if (_title.Length > adjustedWidth)
                    adjustedText = _title.Substring(0, _title.Length - (_title.Length - adjustedWidth));
                else
                    adjustedText = _title;
            }

            if (!string.IsNullOrEmpty(adjustedText))
            {
                _titleWidth = adjustedText.Length;
                if (_titleAlignment == HorizontalAlignment.Left)
                {
                    Print(1, 0, adjustedText, Theme.TitleStyle);
                    _titleLocationX = 1;
                }
                else if (_titleAlignment == HorizontalAlignment.Center)
                {
                    _titleLocationX = ((adjustedWidth - adjustedText.Length) / 2) + 1;
                    Print(_titleLocationX, 0, adjustedText, Theme.TitleStyle);
                }
                else
                {
                    _titleLocationX = textSurface.Width - 1 - adjustedText.Length;
                    Print(_titleLocationX, 0, adjustedText, Theme.TitleStyle);
                }
            }
        }

        /*
        /// <summary>
        /// Determines if a console can become active instead of this one.
        /// </summary>
        /// <param name="askingConsole">The console making the request.</param>
        /// <returns>False when the window is currently shown as modal.</returns>
        public override bool CanActiveBeTaken(IConsole askingConsole)
        {
            // Should I do a request source?? 
            if (askingConsole != null)

            if (_isModal && _isVisible)
                return false;
            else
                return true;
        }
        */

        [OnDeserializedAttribute]
        private void AfterDeserialized(StreamingContext context)
        {
            _previousMouseInfo = new Input.MouseInfo();
            //Redraw();
        }
#endregion
    }
}
