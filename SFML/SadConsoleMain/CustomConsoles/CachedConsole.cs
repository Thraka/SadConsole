namespace StarterProject.CustomConsoles
{
    using System;
    using SadConsole;
    using SadConsole.Consoles;
    using Console = SadConsole.Consoles.Console;
    using SadConsole.Input;
    using static SFML.Window.Keyboard;
    using SFML.Graphics;

    class CachedConsoleConsole : Console
    {
        CachedTextSurfaceRenderer cachedRenderer;
        ITextSurfaceRenderer oldRenderer;

        public CachedConsoleConsole()
            : base(80, 25)
        {
            IsVisible = false;
            FillWithRandomGarbage();

            cachedRenderer = new CachedTextSurfaceRenderer(TextSurface);
            oldRenderer = _renderer;
        }

        public override bool ProcessKeyboard(KeyboardInfo info)
        {
            if (info.IsKeyReleased(Key.Space))
            {
                Renderer = _renderer == oldRenderer ? cachedRenderer : oldRenderer;
                TextSurface.Tint = _renderer == oldRenderer ? Color.Transparent : new Color(0, 0, 0, 70);
            }

            return false;
        }
    }
}
