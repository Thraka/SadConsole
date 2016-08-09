#if MONOGAME
using Microsoft.Xna.Framework;
using System;

namespace SadConsole
{

    /// <summary>
    /// A game component to handle the SadConsole engine initialization, update, and drawing.
    /// </summary>
    public class EngineGameComponent : DrawableGameComponent
    {
        private Action initializationCallback;
        private string font;
        private int screenWidth;
        private int screenHeight;
        private GraphicsDeviceManager manager;

        public EngineGameComponent(Game game, GraphicsDeviceManager manager, string font, int screenWidth, int screenHeight, Action initializeCallback) : base(game)
        {
            this.initializationCallback = initializeCallback;
            this.font = font;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.manager = manager;
        }

        public override void Initialize()
        {
            Engine.Initialize(manager, font, screenWidth, screenHeight);

            manager = null; // no need to hang on to this.

            initializationCallback?.Invoke();
        }

        public override void Update(GameTime gameTime)
        {
            Engine.Update(gameTime, this.Game.IsActive);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Engine.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
#endif