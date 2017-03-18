using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using SadConsole.Surfaces;
using System;
using Console = SadConsole.Console;
using SadConsole.Input;

namespace StarterProject.CustomConsoles
{

    // Using a ConsoleList which lets us group multiple consoles 
    // into a single processing entity
    class MouseRenderingDebug : Console, IConsoleMetadata
    {
        SadConsole.Instructions.DrawString typingInstruction;

        public ConsoleMetadata Metadata
        {
            get
            {
                return new ConsoleMetadata() { Title = "SadConsole.Instructions", Summary = "Automatic typing to a console." };
            }
        }

        public MouseRenderingDebug(): base(80, 23)
        {
        }

        public override void Update(TimeSpan elapsed)
        {
           
        }

        public override bool ProcessKeyboard(SadConsole.Input.Keyboard info)
        {
            return true;
        }

        public override bool ProcessMouse(SadConsole.Input.MouseConsoleState state)
        {
            Clear();
            Print(0, 0, $"mouse:           {state.Mouse.ScreenPosition}");
            Print(0, 1, $"adapter:         {SadConsole.Global.GraphicsDevice.Adapter.CurrentDisplayMode.Width},{SadConsole.Global.GraphicsDevice.Adapter.CurrentDisplayMode.Height}");
            Print(0, 2, $"window:          {SadConsole.Game.Instance.Window.ClientBounds}");
            Print(0, 3, $"pref:            {SadConsole.Global.GraphicsDeviceManager.PreferredBackBufferWidth},{SadConsole.Global.GraphicsDeviceManager.PreferredBackBufferHeight}");
            Print(0, 4, $"pparams:         {SadConsole.Global.GraphicsDevice.PresentationParameters.BackBufferWidth},{SadConsole.Global.GraphicsDevice.PresentationParameters.BackBufferHeight}");
            Print(0, 5, $"viewport:        {SadConsole.Global.GraphicsDevice.Viewport}");
            Print(0, 6, $"viewport.bounds: {SadConsole.Global.GraphicsDevice.Viewport.Bounds}");
            Print(0, 7, $"scale:           {SadConsole.Global.RenderScale}");
            Print(0, 8, $"renderrect:      {SadConsole.Global.RenderRect}");
            Print(0, 9, $"");

            return base.ProcessMouse(state);
        }
    }
}
