using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using SadConsole.Consoles;
using Microsoft.Xna.Framework.Graphics;

namespace SadConsole.Game
{
    /// <summary>
    /// A collection of game objects with cached renderer
    /// </summary>
    public class GameObjectCollection : IList<GameObject>
    {
        List<GameObject> backingList = new List<GameObject>();

        public GameObject this[int index]
        {
            get { return backingList[index]; }
            set { backingList[index] = value; }
        }

        public int Count
        {
            get { return backingList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(GameObject item)
        {
            item.RepositionRects = true;
            backingList.Add(item);
        }

        public void Clear()
        {
            backingList.Clear();
        }

        public bool Contains(GameObject item)
        {
            return backingList.Contains(item);
        }

        public void CopyTo(GameObject[] array, int arrayIndex)
        {
            backingList.CopyTo(array, arrayIndex);
        }

        public IEnumerator<GameObject> GetEnumerator()
        {
            return backingList.GetEnumerator();
        }

        public int IndexOf(GameObject item)
        {
            return backingList.IndexOf(item);
        }

        public void Insert(int index, GameObject item)
        {
            backingList.Insert(index, item);
        }

        public bool Remove(GameObject item)
        {
            return backingList.Remove(item);
        }

        public void RemoveAt(int index)
        {
            backingList.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return backingList.GetEnumerator();
        }

        GameObjectRenderer renderer = new GameObjectRenderer();

        public void Render()
        {
            renderer.Start();

            for (int i = 0; i < backingList.Count; i++)
                renderer.Render(backingList[i].Animation, GameObject.NoMatrix);

            renderer.End();
        }

        public void Update()
        {
            for (int i = 0; i < backingList.Count; i++)
                backingList[i].Update();
        }

        private class GameObjectRenderer : Consoles.TextSurfaceRenderer
        {
            public void Start()
            {
                Batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);
                BeforeRenderCallback?.Invoke(Batch);
            }

            public override void Render(ITextSurfaceRendered surface, Matrix renderingMatrix)
            {
                if (surface.Tint.A != 255)
                {
                    Cell cell;

                    if (surface.DefaultBackground.A != 0)
                        Batch.Draw(surface.Font.FontImage, surface.AbsoluteArea, surface.Font.GlyphIndexRects[surface.Font.SolidGlyphIndex], surface.DefaultBackground, 0f, Vector2.Zero, SpriteEffects.None, 0.2f);

                    for (int i = 0; i < surface.RenderCells.Length; i++)
                    {
                        cell = surface.RenderCells[i];

                        if (cell.IsVisible)
                        {
                            if (cell.ActualBackground != Color.Transparent && cell.ActualBackground != surface.DefaultBackground)
                                Batch.Draw(surface.Font.FontImage, surface.RenderRects[i], surface.Font.GlyphIndexRects[surface.Font.SolidGlyphIndex], cell.ActualBackground, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

                            if (cell.ActualForeground != Color.Transparent)
                                Batch.Draw(surface.Font.FontImage, surface.RenderRects[i], surface.Font.GlyphIndexRects[cell.ActualGlyphIndex], cell.ActualForeground, 0f, Vector2.Zero, cell.ActualSpriteEffect, 0.4f);
                        }
                    }

                    if (surface.Tint.A != 0)
                        Batch.Draw(surface.Font.FontImage, surface.AbsoluteArea, surface.Font.GlyphIndexRects[surface.Font.SolidGlyphIndex], surface.Tint, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                }
                else
                {
                    Batch.Draw(surface.Font.FontImage, surface.AbsoluteArea, surface.Font.GlyphIndexRects[surface.Font.SolidGlyphIndex], surface.Tint, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                }
            }

            public void End()
            {
                AfterRenderCallback?.Invoke(Batch);
                Batch.End();
            }
        }
    }
}
