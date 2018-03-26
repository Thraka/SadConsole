using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole;

namespace MonoGame1
{
    public class Game1 : SadConsole.Game
    {
        SpriteBatch _spriteBatch;
        SadConsole.Console _console;
        SadConsole.GameHelpers.GameObject _gameObject;

        public Game1() : base("Fonts/IBM.font", 240, 68, null)
        {
            Content.RootDirectory = "Content";
            GraphicsDeviceManager.IsFullScreen = true;
            SadConsole.Game.OnDraw = DrawFrame;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            IsMouseVisible = true;
            base.Initialize();
            _console = new SadConsole.Console(120, 34);
            SadConsole.Global.CurrentScreen.Children.Add(_console);
            SadConsole.FontMaster fontMaster = SadConsole.Global.LoadFont("Fonts/IBM.font");
            _console.TextSurface.Font = fontMaster.GetFont(SadConsole.Font.FontSizes.Two);

            AddDebugEdgeNumbers();
            AddFill();
            AddBox();
            AddCircle();
            AddLine();
            AddGameObject();
        }

        void AddDebugEdgeNumbers()
        {
            for (int i = 0; i < _console.Width; i++)
            {
                int r1 = (i > 100) ? ((i - 100) / 10) : i / 10;
                int r2 = i % 10;
                _console.Print(i, 0, r1.ToString());
                _console.Print(i, 1, r2.ToString());
            }

            for (int i = 0; i < _console.Height; i++)
            {
                _console.Print(0, i, i.ToString());
            }
        }

        void AddFill()
        {
            _console.Fill(new Rectangle(2, 2, 20, 3), Color.Aqua, Color.OliveDrab, 0);
        }

        void AddGameObject()
        {
            _gameObject = new SadConsole.GameHelpers.GameObject(1,1);
            _gameObject.Animation.CurrentFrame[0].Glyph = '@';
            _gameObject.Animation.CurrentFrame[0].Background = Color.Red;
            _gameObject.Animation.CurrentFrame[0].Foreground = Color.White;
            _gameObject.Animation.IsDirty = true;
            _gameObject.IsVisible = true;
            _gameObject.Position = new Point(20, 20);
        }

        void AddBox()
        {
            SadConsole.Shapes.Box box = SadConsole.Shapes.Box.GetDefaultBox();
            box.Foreground = Color.Blue;
            box.BorderBackground = Color.Black;
            box.FillColor = Color.Black;
            box.Fill = true;
            box.Position = new Point(5, 5);
            box.Width = 10;
            box.Height = 10;
            box.Draw(_console);
        }

        void AddCircle()
        {
            SadConsole.Shapes.Circle circle = new SadConsole.Shapes.Circle();
            circle.BorderAppearance = new Cell(Color.Yellow, Color.Black, 'x');
            circle.Center = new Point(35, 15);
            circle.Radius = 10;
            circle.Draw(_console);
        }

        void AddLine()
        {
            SadConsole.Shapes.Line line = new SadConsole.Shapes.Line();
            line.StartingLocation = new Point(50, 30);
            line.EndingLocation = new Point(80, 20);
            line.UseEndingCell = false;
            line.UseStartingCell = false;
            line.Cell = new Cell { Foreground = Color.Purple, Background = Color.Black, Glyph = 'o' };
            line.Draw(_console);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            _gameObject.Update(gameTime.ElapsedGameTime);

            base.Update(gameTime);
        }

        private void DrawFrame(GameTime gameTime)
        {
            // TODO: Add your custom draw logic here
            _gameObject.Draw(gameTime.ElapsedGameTime);
        }

    }
}
