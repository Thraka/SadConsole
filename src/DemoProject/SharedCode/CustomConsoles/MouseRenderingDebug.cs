using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using SadConsole.Surfaces;
using System;
using Console = SadConsole.Console;
using SadConsole.Input;
using SadConsole;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace StarterProject.CustomConsoles
{

    public class CellPalette : SadConsole.Cell
    {
        public int ForegroundIndex { get; set; }
        public int BackgroundIndex { get; set; }

        public CellPalette(int foregroundIndex, int backgroundIndex, int glyph)
        {
            Glyph = glyph;
            ForegroundIndex = foregroundIndex;
            BackgroundIndex = backgroundIndex;
        }
    }

    public class PaletteSurface: Console
    {
        private Palette palette;

        public Palette Palette
        {
            get { return palette; }
            set { palette = value;
                ((PaletteSurfaceRenderer) Renderer).palette = value; ValidateCells(); }
        }


        public PaletteSurface(int width, int height, Palette palette): base(width, height)
        {
            this.palette = palette;
            Renderer = new PaletteSurfaceRenderer() {palette = palette};

        }


        protected override void OnCellsReset()
        {
            Cells = new Cell[Width * Height];

            for (int i = 0; i < Cells.Length; i++)
                Cells[i] = new CellPalette(0, 1, 0);

            RenderCells = (Cell[])Cells.Clone();
            RenderRects = new Rectangle[Cells.Length];
        }

        protected void ValidateCells()
        {
            foreach (var cell in this.Cells.Cast<CellPalette>())
            {
                if (cell.ForegroundIndex >= palette.Length)
                    cell.ForegroundIndex = 0;

                if (cell.BackgroundIndex >= palette.Length)
                    cell.BackgroundIndex = 0;
            }
        }
    }

    public class PaletteSurfaceRenderer: SadConsole.Renderers.Basic
    {
        public Palette palette;

        public override void Render(ScreenObject surface, bool force = false)
        {
            base.Render(surface, true);
        }

        public override void RenderCells(ScreenObject surfacePreCast, bool force = false)
        {
            PaletteSurface surface = (PaletteSurface)surfacePreCast;
            if (surface.IsDirty || force)
            {
                if (surface.Tint.A != 255)
                {
                    CellPalette cell;
                    Color background;
                    Color foreground;


                    if (surface.DefaultBackground.A != 0)
                        Global.SpriteBatch.Draw(surface.Font.FontImage, surface.AbsoluteArea, surface.Font.GlyphRects[surface.Font.SolidGlyphIndex], surface.DefaultBackground, 0f, Vector2.Zero, SpriteEffects.None, 0.2f);

                    for (int i = 0; i < surface.RenderCells.Length; i++)
                    {
                        cell = (CellPalette)surface.RenderCells[i];

                        if (cell.IsVisible)
                        {
                            background = palette[cell.BackgroundIndex];
                            foreground = palette[cell.ForegroundIndex];

                            if (background != Color.Transparent && background != surface.DefaultBackground)
                                Global.SpriteBatch.Draw(surface.Font.FontImage, surface.RenderRects[i], surface.Font.GlyphRects[surface.Font.SolidGlyphIndex], background, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

                            if (foreground != Color.Transparent)
                                Global.SpriteBatch.Draw(surface.Font.FontImage, surface.RenderRects[i], surface.Font.GlyphRects[cell.Glyph], foreground, 0f, Vector2.Zero, cell.Mirror, 0.4f);
                        }
                    }
                }
            }
        }
    }


    // Using a ConsoleList which lets us group multiple consoles 
    // into a single processing entity
    class MouseRenderingDebug : Console
    {
        SadConsole.Instructions.DrawString typingInstruction;
        
        Palette pal;
        Timer timer;
        Timer timer2;
        Timer timer3;
        Timer timer4;
        Timer timer5;

        public MouseRenderingDebug(): base(80, 23)
        {
            pal = new Palette(new ColorGradient(Color.White, Color.Violet, Color.Black, Color.White).ToColorArray(25));
            PaletteSurface surfacePal = new PaletteSurface(5, 5, pal);

            for (int i = 0; i < 25; i++)
                ((CellPalette)surfacePal[i]).BackgroundIndex = i;

            Children.Add(surfacePal);

            surfacePal.Print(0, 0, "Hello from printing!");

            timer = new Timer(100, (t, a) => pal.ShiftRight(0, 5));
            timer5 = new Timer(2000, (t, a) => pal.ShiftLeft());
        }

        public override void Update(TimeSpan delta)
        {
            base.Update(delta);

            //pal.ShiftLeft(0, 5);
            timer.Update(delta.TotalMilliseconds);
            timer5.Update(delta.TotalMilliseconds);
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
