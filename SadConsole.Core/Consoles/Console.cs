using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SadConsole.Consoles
{
    /// <summary>
    /// A basic console ready to render and use. Implements it's own <see cref="Parent"/> and <see cref="IsFocused"/> logic. By default uses the <see cref="CustomConsole"/> and <see cref="TextSurfaceRenderer"/> types for console management and rendering.
    /// </summary>
    public class Console: TextSurface, IConsole, ITextSurfaceRenderer
    {
        protected IConsole console;
        protected ITextSurfaceRenderer renderer;
        protected IConsoleList _parentConsole;

        #region Constructors

        /// <summary>
        /// Creates a console with the specified width and height. Uses <see cref="Engine.DefaultFont"/> as the font and the <see cref="CustomConsole"/> and <see cref="TextSurfaceRenderer"/> types for console management and rendering.
        /// </summary>
        /// <param name="width">Width of the console.</param>
        /// <param name="height">Height of the console.</param>
        public Console(int width, int height) : this(width, height, Engine.DefaultFont) { }

        /// <summary>
        /// Creates a console with the specified width, height, and font. Uses the <see cref="CustomConsole"/> and <see cref="TextSurfaceRenderer"/> types for console management and rendering.
        /// </summary>
        /// <param name="width">Width of the console.</param>
        /// <param name="height">Height of the console.</param>
        /// <param name="font">The font used in rendering.</param>
        public Console(int width, int height, Font font) : base(width, height, font)
        {
            console = new CustomConsole(this);
            renderer = new TextSurfaceRenderer();
            ((CustomConsole)console).Renderer = renderer;
        }

        /// <summary>
        /// Creates the console with the specified width, height, backing console and renderer. If the console uses a customer renderer, the <paramref name="supportingRenderer"/> parameter should match.
        /// </summary>
        /// <param name="width">Width of the console.</param>
        /// <param name="height">Height of the console.</param>
        /// <param name="font">The font used in rendering.</param>
        /// <param name="supportingConsole">Custom console.</param>
        /// <param name="supportingRenderer">Custom renderer</param>
        public Console(int width, int height, Font font, IConsole supportingConsole, ITextSurfaceRenderer supportingRenderer) : base(width, height, font)
        {
            console = supportingConsole;
            this.renderer = supportingRenderer;
        }

        #endregion

        #region IConsole
        public bool CanFocus
        {
            get
            {
                return console.CanFocus;
            }

            set
            {
                console.CanFocus = value;
            }
        }

        public bool CanUseKeyboard
        {
            get
            {
                return console.CanUseKeyboard;
            }

            set
            {
                console.CanUseKeyboard = value;
            }
        }

        public bool CanUseMouse
        {
            get
            {
                return console.CanUseMouse;
            }

            set
            {
                console.CanUseMouse = value;
            }
        }

        TextSurface IConsole.Data
        {
            get
            {
                return this;
            }
        }

        Rectangle IConsole.ViewArea
        {
            get
            {
                return this.ViewArea;
            }

            set
            {
                this.ViewArea = value;
            }
        }

        public bool DoUpdate
        {
            get
            {
                return console.DoUpdate;
            }

            set
            {
                console.DoUpdate = value;
            }
        }

        public bool ExclusiveFocus
        {
            get
            {
                return console.ExclusiveFocus;
            }

            set
            {
                console.ExclusiveFocus = value;
            }
        }

        /// <summary>
        /// Gets or sets this console as the <see cref="Engine.ActiveConsole"/> value.
        /// </summary>
        /// <remarks>If the <see cref="Engine.ActiveConsole"/> has the <see cref="Console.ExclusiveFocus"/> property set to true, you cannot use this property to set this console to focused.</remarks>
        public bool IsFocused
        {
            get { return Engine.ActiveConsole == this; }
            set
            {
                if (Engine.ActiveConsole != null)
                {
                    if (value && Engine.ActiveConsole != this && !Engine.ActiveConsole.ExclusiveFocus)
                    {
                        Engine.ActiveConsole = this;
                        OnFocused();
                    }

                    else if (value && Engine.ActiveConsole == this)
                        OnFocused();

                    else if (!value)
                    {
                        if (Engine.ActiveConsole == this)
                            Engine.ActiveConsole = null;

                        OnFocusLost();
                    }
                }
                else
                {
                    if (value)
                    {
                        Engine.ActiveConsole = this;
                        OnFocused();
                    }
                    else
                        OnFocusLost();
                }
            }
        }
        public bool IsVisible
        {
            get
            {
                return console.IsVisible;
            }

            set
            {
                console.IsVisible = value;
            }
        }

        public IConsoleList Parent
        {
            get { return _parentConsole; }
            set
            {
                if (_parentConsole != value)
                {
                    if (_parentConsole == null)
                    {
                        _parentConsole = value;
                        _parentConsole.Add(this);
                        OnParentConsoleChanged(null, _parentConsole);
                    }
                    else
                    {
                        var oldParent = _parentConsole;
                        _parentConsole = value;

                        oldParent.Remove(this);

                        if (_parentConsole != null)
                            _parentConsole.Add(this);

                        OnParentConsoleChanged(oldParent, _parentConsole);
                    }
                }

            }
        }
        public Point Position
        {
            get
            {
                return console.Position;
            }

            set
            {
                console.Position = value;
            }
        }

        public bool UsePixelPositioning
        {
            get
            {
                return console.UsePixelPositioning;
            }

            set
            {
                console.UsePixelPositioning = value;
            }
        }

        public Cursor VirtualCursor
        {
            get
            {
                return console.VirtualCursor;
            }
        }

        /// <summary>
        /// Toggles the VirtualCursor as visible\hidden when the console if focused\unfocused.
        /// </summary>
        public bool AutoCursorOnFocus { get { return console.AutoCursorOnFocus; } set { console.AutoCursorOnFocus = value; } }

        public virtual bool ProcessKeyboard(KeyboardInfo info)
        {
            return console.ProcessKeyboard(info);
        }

        public virtual bool ProcessMouse(MouseInfo info)
        {
            return console.ProcessMouse(info);
        }

        public virtual void Render()
        {
            console.Render();
        }

        public virtual void Update()
        {
            console.Update();
        }

        #endregion

        #region ITextSurfaceRenderer
        Action<SpriteBatch> ITextSurfaceRenderer.AfterRenderCallback
        {
            get
            {
                return renderer.AfterRenderCallback;
            }

            set
            {
                renderer.AfterRenderCallback = value;
            }
        }

        SpriteBatch ITextSurfaceRenderer.Batch
        {
            get
            {
                return renderer.Batch;
            }
        }

        Action<SpriteBatch> ITextSurfaceRenderer.BeforeRenderCallback
        {
            get
            {
                return renderer.BeforeRenderCallback;
            }

            set
            {
                renderer.BeforeRenderCallback = value;
            }
        }

        void ITextSurfaceRenderer.Render(ITextSurface cells, Matrix renderingMatrix)
        {
            renderer.Render(cells, renderingMatrix);
        }

        void ITextSurfaceRenderer.Render(ITextSurface cells, Point position, bool usePixelPositioning)
        {
            renderer.Render(cells, position, usePixelPositioning);
        }

        #endregion  


        /// <summary>
        /// Called when the parent console changes for this console.
        /// </summary>
        /// <param name="oldParent">The previous parent.</param>
        /// <param name="newParent">The new parent.</param>
        protected virtual void OnParentConsoleChanged(IConsoleList oldParent, IConsoleList newParent) { }



        /// <summary>
        /// Called when this console's focus has been lost.
        /// </summary>
        protected virtual void OnFocusLost()
        {
            if (AutoCursorOnFocus == true)
                VirtualCursor.IsVisible = false;
        }

        /// <summary>
        /// Called when this console is focused.
        /// </summary>
        protected virtual void OnFocused()
        {
            if (AutoCursorOnFocus == true)
                VirtualCursor.IsVisible = true;
        }
    }
}
