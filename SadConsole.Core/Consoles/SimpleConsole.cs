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
    public class SimpleConsole: TextSurface, IConsole, ITextSurfaceRenderer
    {
        protected IConsole console;
        protected ITextSurfaceRenderer renderer;

        #region Constructors

        /// <summary>
        /// Creates a console with the specified width and height. Uses <see cref="Engine.DefaultFont"/> as the font and the <see cref="Console"/> and <see cref="TextSurfaceRenderer"/> types for console management and rendering.
        /// </summary>
        /// <param name="width">Width of the console.</param>
        /// <param name="height">Height of the console.</param>
        public SimpleConsole(int width, int height) : this(width, height, Engine.DefaultFont) { }

        /// <summary>
        /// Creates a console with the specified width, height, and font. Uses the <see cref="Console"/> and <see cref="TextSurfaceRenderer"/> types for console management and rendering.
        /// </summary>
        /// <param name="width">Width of the console.</param>
        /// <param name="height">Height of the console.</param>
        /// <param name="font">The font used in rendering.</param>
        public SimpleConsole(int width, int height, Font font) : base(width, height, font)
        {
            console = new Console(this);
            renderer = new TextSurfaceRenderer();
            ((Console)console).Renderer = renderer;
        }

        /// <summary>
        /// Creates the console with the specified width, height, backing console and renderer. If the console uses a customer renderer, the <paramref name="supportingRenderer"/> parameter should match.
        /// </summary>
        /// <param name="width">Width of the console.</param>
        /// <param name="height">Height of the console.</param>
        /// <param name="font">The font used in rendering.</param>
        /// <param name="supportingConsole">Custom console.</param>
        /// <param name="supportingRenderer">Custom renderer</param>
        public SimpleConsole(int width, int height, Font font, IConsole supportingConsole, ITextSurfaceRenderer supportingRenderer) : base(width, height, font)
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

        public bool IsFocused
        {
            get
            {
                return console.IsFocused;
            }

            set
            {
                console.IsFocused = value;
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
            get
            {
                return console.Parent;
            }

            set
            {
                console.Parent = value;
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
    }
}
