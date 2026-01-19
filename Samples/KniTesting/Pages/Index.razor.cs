using System;
using Microsoft.JSInterop;
using SadConsole;
using SadConsole.Configuration;
using SadConsole.Examples;
using SadRogue.Primitives;

namespace KniTesting.Pages
{
    public partial class Index
    {
        Game _game;

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                JsRuntime.InvokeAsync<object>("initRenderJS", DotNetObjectReference.Create(this));
            }
        }

        [JSInvokable]
        public void TickDotNet()
        {
            // init game
            if (_game == null)
            {
                //_game = new KniTestingGame();
                //_game.Run();

                Builder startup = new Builder()
                    .SetWindowSizeInCells(90, 30)
                    .UseDefaultConsole()
                    //.OnStart(Game_Started)
                    .ConfigureFonts(true)

                    .SetStartingScreen<RootScreen>()
    .IsStartingScreenFocused(false) // Don't want RootScreen to be focused because RootScreen automatically focuses the selected demo console
    .SetWindowSizeInCells(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT)


                    //.SkipMonoGameGameCreation()
                    ;

                Game.Create(startup);
                Game.Instance.Run();
                _game = Game.Instance;

                void Game_Started(object? sender, GameHost host)
                {
                    ColoredGlyph boxBorder = new(Color.White, Color.Black, 178);
                    ColoredGlyph boxFill = new(Color.White, Color.Black);

                    Game.Instance.StartingConsole.Surface.DefaultBackground = Color.Transparent;
                    Game.Instance.StartingConsole.FillWithRandomGarbage(255);
                    //Game.Instance.StartingConsole.Surface.Clear();

                    SadConsole.Settings.ClearColor = Color.Transparent;

                    Game.Instance.StartingConsole.DrawBox(new Rectangle(2, 2, 26, 5), ShapeParameters.CreateFilled(boxBorder, boxFill));
                    Game.Instance.StartingConsole.Print(4, 4, "Welcome to SadConsole!");
                }

            }

            // run gameloop
            _game.Tick();
        }

    }
}
