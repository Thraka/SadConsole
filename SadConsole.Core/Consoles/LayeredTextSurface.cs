using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Consoles
{
    public class LayeredTextSurface: TextSurface
    {
        protected Cell[][] renderCellsLayers;
        protected Cell[][] cellsLayers;

        public int Layers { get; private set; }

        public int ActiveLayerIndex { get; private set; }

        public LayeredTextSurface(int width, int height, int layers) : this(width, height, layers, Engine.DefaultFont) { }

        public LayeredTextSurface(int width, int height, int layers, Font font): base(width, height, font)
        {
            Layers = layers;

            for (int i = 0; i < layers; i++)
            {

            }
        }

        public void SetActiveLayer(int index)
        {
            if (ActiveLayerIndex <= 0 || ActiveLayerIndex >= Layers)
                throw new ArgumentOutOfRangeException("index");

            base.cells = cellsLayers[index];
            base.RenderCells = renderCellsLayers[index];
        }

        protected override void InitializeCells()
        {
            cellsLayers = new Cell[Layers][];
            renderCellsLayers = new Cell[Layers][];

            for (int i = 0; i < Layers; i++)
            {
                cellsLayers[i] = new Cell[width * height];

                for (int c = 0; c < base.cells.Length; c++)
                {
                    cellsLayers[i][c] = new Cell();
                    cellsLayers[i][c].Foreground = this.DefaultForeground;
                    cellsLayers[i][c].Background = this.DefaultBackground;
                    cellsLayers[i][c].OnCreated();
                }

                renderCellsLayers[i] = cellsLayers[i];
            }
            
            // Setup the new render area
            //ResetArea();
        }

        //protected override void ResetArea()
        //{
        //    RenderRects = new Rectangle[area.Width * area.Height];
        //    renderCellsLayers = new Cell[Layers][];

        //    int index = 0;

        //    for (int y = 0; y < area.Height; y++)
        //    {
        //        for (int x = 0; x < area.Width; x++)
        //        {
        //            for (int i = 0; i < Layers; i++)
        //            {
        //                renderCellsLayers[i][index] = cellsLayers[i][(y + area.Top) * width + (x + area.Left)];
        //            }

        //            RenderRects[index] = new Rectangle(x * Font.Size.X, y * Font.Size.Y, Font.Size.X, Font.Size.Y);
        //            index++;
        //        }
        //    }

        //    // TODO: Optimization by calculating AbsArea and seeing if it's diff from current, if so, don't create new RenderRects
        //    AbsoluteArea = new Rectangle(0, 0, area.Width * Font.Size.X, area.Height * Font.Size.Y);
        //}
    }
}
