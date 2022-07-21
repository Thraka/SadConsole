using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Assets;
using Myra.Graphics2D.UI;
using SadConsole.MyraUI;

namespace SadConsole.MyraUI
{
    public class MyraGameComponent : DrawableGameComponent
    {
        private readonly Microsoft.Xna.Framework.Game _game;
        private GraphicsDeviceManager _graphics;

        public event EventHandler HostClosed;

        private Desktop _desktop;

        public MyraGameComponent(GraphicsDeviceManager graphics, Microsoft.Xna.Framework.Game game) : base(game)
        {
            // Run after (and thus draw on top of) the normal SadConsole MonoGame component
            DrawOrder = 7;

            // Run before the normal SadConsole MonoGame component
            UpdateOrder = 3;

            _graphics = graphics;
            _game = game;
            Myra.MyraEnvironment.Game = game;
            _desktop = new Desktop();

            _desktop.Root = new DebuggerPage().Root;
        }

        public override void Draw(GameTime gameTime)
        {
            _desktop.Render();
            //if (ImGuiRenderer.HideRequested)
            //{
            //    ImGuiRenderer.HideRequested = false;

            //    Enabled = false;
            //    Visible = false;

            //    HostClosed?.Invoke(this, EventArgs.Empty);
            //}
        }
    }
}
