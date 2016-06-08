using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Consoles
{
    public class LayeredTextSurface: TextSurface
    {
        public class Layer
        {
            public Cell[] Cells;
            public Cell[] RenderCells;
            public bool IsVisible;
        }
        protected Layer[] Layers;

        public int LayerCount { get; private set; }

        public int ActiveLayerIndex { get; private set; }

        public LayeredTextSurface(int width, int height, int layers) : this(width, height, layers, Engine.DefaultFont) { }

        public LayeredTextSurface(int width, int height, int layers, Font font): base(width, height, font)
        {
            Layers = new Layer[layers];
            LayerCount = layers;

            for (int i = 0; i < layers; i++)
                Layers[i] = new Layer();
        }

        public void SetActiveLayer(int index)
        {
            if (ActiveLayerIndex <= 0 || ActiveLayerIndex >= LayerCount)
                throw new ArgumentOutOfRangeException("index");

            base.cells = Layers[index].Cells;
            base.RenderCells = Layers[index].RenderCells;
        }

        protected override void InitializeCells()
        {
            for (int i = 0; i < LayerCount; i++)
            {
                Layers[i].Cells = new Cell[width * height];

                for (int c = 0; c < base.cells.Length; c++)
                {
                    Layers[i].Cells[c] = new Cell();
                    Layers[i].Cells[c].Foreground = this.DefaultForeground;
                    Layers[i].Cells[c].Background = this.DefaultBackground;
                    Layers[i].Cells[c].OnCreated();
                }

                Layers[i].RenderCells = Layers[i].Cells;
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
