using System;
using SadConsole;
using SadConsole.Components;
using SadConsole.Input;
using SadRogue.Primitives;

namespace FeatureDemo.CustomConsoles
{
    // Using a ConsoleList which lets us group multiple consoles 
    // into a single processing entity
    internal class MouseRenderingDebug : ScreenSurface
    {
        private readonly SadConsole.Instructions.DrawString typingInstruction;
        private readonly Palette pal;
        private readonly Timer timer;
        private readonly Timer timerLeftReset;
        private readonly Timer timerRightReset;
        private readonly Timer timer4;
        private readonly Timer timer5;

        public MouseRenderingDebug() : base(80, 23)
        {
            pal = new Palette(new ColorGradient(Color.White, Color.Violet, Color.Black, Color.White).ToColorArray(25));
            PaletteSurface surfacePal = new PaletteSurface(5, 5, pal);

            for (int i = 0; i < 25; i++)
            {
                ((CellPalette)surfacePal[i]).BackgroundIndex = i;
            }

            //Children.Add(new ScreenSurface(surfacePal));

            surfacePal.Print(0, 0, "Hello from printing!");

            timer = new Timer(TimeSpan.FromMilliseconds(100));
            timer.TimerElapsed += (t, e) => pal.ShiftRight(0, 5);

            timer5 = new Timer(TimeSpan.FromMilliseconds(2000));
            timer5.TimerElapsed += (t, e) => pal.ShiftLeft();

            timerLeftReset = new Timer(TimeSpan.FromMilliseconds(2000));
            timerLeftReset.TimerElapsed += timerLeftReset_TimerElapsed;
            timerLeftReset.IsPaused = true;
            timerLeftReset.Repeat = false;

            timerRightReset = new Timer(TimeSpan.FromMilliseconds(2000));
            timerRightReset.TimerElapsed += TimerRightReset_TimerElapsed;
            timerRightReset.IsPaused = true;
            timerRightReset.Repeat = false;

            //SadComponents.Add(timer);
            //SadComponents.Add(timer5);

            SadComponents.Add(timerLeftReset);
            SadComponents.Add(timerRightReset);
        }

        private void TimerRightReset_TimerElapsed(object sender, EventArgs e)
        {
            rightClicked = false;
        }

        private void timerLeftReset_TimerElapsed(object sender, EventArgs e)
        {
            leftClicked = false;
        }

        public override void Update(TimeSpan delta) => base.Update(delta);//pal.ShiftLeft(0, 5);

        private bool showMouse = false;
        private bool DoClear = false;

        public override bool ProcessKeyboard(SadConsole.Input.Keyboard info)
        {
            if (info.IsKeyReleased(Keys.Space))
            {
                showMouse = !showMouse;
            }

            if (info.IsKeyReleased(Keys.Enter))
            {
                //Cursor.UsePrintProcessor = !UsePrintProcessor;
            }

            if (info.IsKeyReleased(Keys.C))
            {
                DoClear = !DoClear;
            }

            return true;
        }

        bool leftClicked = false;
        bool rightClicked = false;

        public override bool ProcessMouse(SadConsole.Input.MouseScreenObjectState state)
        {
            //if (showMouse)
            {
                if (DoClear)
                {
                    Surface.Clear();
                }

                if (state.Mouse.LeftClicked)
                {
                    leftClicked = true;
                    timerLeftReset.Restart();
                }

                if (state.Mouse.RightClicked)
                {
                    rightClicked = true;
                    timerRightReset.Restart();
                }

                Surface.Clear(0, 0, Surface.Width);
                Surface.Print(0, 0, $"mouse:           Pos:{state.Mouse.ScreenPosition} L:{state.Mouse.LeftButtonDown} LClk:{leftClicked} R:{state.Mouse.RightButtonDown} RClk:{rightClicked}");
#if MONOGAME
                Surface.Print(0, 1, $"adapter:         {SadConsole.Host.Global.GraphicsDevice.Adapter.CurrentDisplayMode.Width},{SadConsole.Host.Global.GraphicsDevice.Adapter.CurrentDisplayMode.Height}");
                Surface.Print(0, 2, $"window:          {SadConsole.Game.Instance.MonoGameInstance.Window.ClientBounds}");
                Surface.Print(0, 3, $"pref:            {SadConsole.Host.Global.GraphicsDeviceManager.PreferredBackBufferWidth},{SadConsole.Host.Global.GraphicsDeviceManager.PreferredBackBufferHeight}");
                Surface.Print(0, 4, $"pparams:         {SadConsole.Host.Global.GraphicsDevice.PresentationParameters.BackBufferWidth},{SadConsole.Host.Global.GraphicsDevice.PresentationParameters.BackBufferHeight}");
                Surface.Print(0, 5, $"viewport:        {SadConsole.Host.Global.GraphicsDevice.Viewport}");
                Surface.Print(0, 6, $"viewport.bounds: {SadConsole.Host.Global.GraphicsDevice.Viewport.Bounds}");

                //Surface.Print(0, 9, $"UsePrintProcessor: {UsePrintProcessor}");
#endif
                Surface.Print(0, 7, $"scale:           {SadConsole.Settings.Rendering.RenderScale}");
                Surface.Print(0, 8, $"renderrect:      {SadConsole.Settings.Rendering.RenderRect}");
                Surface.Print(0, 10, $"DoClear: {DoClear}");
            }

            return base.ProcessMouse(state);
        }

        protected override void OnMouseLeftClicked(MouseScreenObjectState state) =>
            base.OnMouseLeftClicked(state);
    }
}
