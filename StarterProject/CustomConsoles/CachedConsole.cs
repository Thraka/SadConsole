namespace StarterProject.CustomConsoles
{
    using System;
    using SadConsole;
    using SadConsole.Consoles;
    using Console = SadConsole.Consoles.Console;
    using Microsoft.Xna.Framework;
    using SadConsole.Input;

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
            if (info.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                Renderer = _renderer == oldRenderer ? cachedRenderer : oldRenderer;
                TextSurface.Tint = _renderer == oldRenderer ? Color.Transparent : Color.Black * 0.25f;
            }

            return false;
        }
    }
}
