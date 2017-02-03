using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using SadConsole.Consoles;
using System;
using Console = SadConsole.Consoles.Console;
using SadConsole.Input;

namespace StarterProject.CustomConsoles
{

    // Using a ConsoleList which lets us group multiple consoles 
    // into a single processing entity
    class FadingExample : SadConsole.Consoles.ConsoleList
    {
        Console backgroundConsole;
        Console foregroundConsole;

        bool startFade;
        SadConsole.Instructions.FadeTextSurfaceTint tintFade;

        public FadingExample()
        {
            // Simulate two consoles
            backgroundConsole = new Console(80, 25);
            foregroundConsole = new Console(80, 25);

            // To work with our sample project, all consoles default to not visible. If you're using this
            // on your own, outside of the starter project, delete this line.
            IsVisible = false;
            
            // Setup main view
            backgroundConsole.FillWithRandomGarbage();

            // Setup sub view
            // Normally the text surface uses a transparent background so you can easily layer. We don't want that.
            foregroundConsole.TextSurface.DefaultBackground = Color.Black;
            foregroundConsole.Clear();

            // Some visuals on the surface so we can see something fade.
            var border = SadConsole.Shapes.Box.GetDefaultBox();
            border.Location = new Point(0, 0);
            border.Width = 80;
            border.Height = 25;
            border.Draw(foregroundConsole);

            for (int y = 2; y < 23; y += 2)
                foregroundConsole.Print(9, y, "This is a console that will fade, showing whatever is beind it", Color.Black.GetRandomColor(SadConsole.Engine.Random));

            foregroundConsole.Print(2, 0, "[c:r f:Black][c:r b:White]  PRESS SPACE TO START FADE  ");

            // Console has been drawn on and is ready to be cached for the fade.
            foregroundConsole.Renderer = new SadConsole.Consoles.CachedTextSurfaceRenderer(foregroundConsole.TextSurface);

            // Because of the way the cached renderer handles tinting, we need to set tinting to white
            // This means "render the cahced surface exactly how it was originally drawn."
            foregroundConsole.TextSurface.Tint = Color.White;

            // Ad the consoles to the list.
            Add(backgroundConsole);
            Add(foregroundConsole);

            // Setup the fade animation to go from white to transparent in 2 seconds.
            tintFade = new SadConsole.Instructions.FadeTextSurfaceTint(foregroundConsole.TextSurface, new ColorGradient(Color.White, Color.Transparent), TimeSpan.FromSeconds(2));
        }

        public override void Update()
        {
            // Check if we have started the fade and that it has not yet finished.
            if (startFade && !tintFade.IsFinished)
                tintFade.Run();
        }

        public override bool ProcessKeyboard(KeyboardInfo info)
        {
            // If the space key is pressed, run the fade
            if (info.IsKeyReleased(Keys.Space))
                startFade = true;

            // Do not pass the keyboard input to the child consoles, eat it.
            return true;
        }
    }
}
