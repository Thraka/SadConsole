#if SFML
using Point = SFML.System.Vector2i;
using Vector2 = SFML.System.Vector2f;
using SFML.System;
using Matrix = SFML.Graphics.Transform;
using SFML.Graphics;
#elif MONOGAME
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endif

using System.Collections.Generic;
using System.Collections;
using SadConsole.Consoles;
using System.Runtime.Serialization;

namespace SadConsole.Game
{
    /// <summary>
    /// A collection of game objects with cached renderer
    /// </summary>
    [DataContract]
    public class GameObjectCollection : IList<GameObject>
    {
        [DataMember(Name = "UseRepositionRects")]
        private bool useRepositionRects;

        [DataMember(Name = "Objects")]
        List<GameObject> backingList = new List<GameObject>();

        /// <summary>
        /// Gets or sets a game object by index.
        /// </summary>
        /// <param name="index">Index in the collection.</param>
        /// <returns>A game object.</returns>
        public GameObject this[int index]
        {
            get { return backingList[index]; }
            set { backingList[index] = value; value.RepositionRects = true; }
        }

        public bool UseRepositionRects
        {
            get { return useRepositionRects; }
            set
            {
                useRepositionRects = value;

                for (int i = 0; i < backingList.Count; i++)
                    backingList[i].RepositionRects = value;
            }
        }

        /// <summary>
        /// The amount of items in the collection.
        /// </summary>
        public int Count
        {
            get { return backingList.Count; }
        }

        /// <summary>
        /// Always false.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Adds a game object to the collection and sets <see cref="GameObject.RepositionRects"/> to true.
        /// </summary>
        /// <param name="item">The game object.</param>
        public void Add(GameObject item)
        {
            item.RepositionRects = useRepositionRects;
            backingList.Add(item);
        }

        /// <summary>
        /// Removes all game objects from the collection.
        /// </summary>
        public void Clear()
        {
            backingList.Clear();
        }

        /// <summary>
        /// Returns true when the specified game object exists.
        /// </summary>
        /// <param name="item">The game object.</param>
        /// <returns>True or false.</returns>
        public bool Contains(GameObject item)
        {
            return backingList.Contains(item);
        }

        /// <summary>
        /// Copies the collection to an array, starting at the specified index.
        /// </summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The starting index.</param>
        public void CopyTo(GameObject[] array, int arrayIndex)
        {
            backingList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets an enumerator for the collection.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<GameObject> GetEnumerator()
        {
            return backingList.GetEnumerator();
        }

        /// <summary>
        /// Gets the index of a game object.
        /// </summary>
        /// <param name="item">The game object.</param>
        /// <returns></returns>
        public int IndexOf(GameObject item)
        {
            return backingList.IndexOf(item);
        }

        /// <summary>
        /// Inserts a game object at the specified index.
        /// </summary>
        /// <param name="index">The index of where the game object will be inserted at.</param>
        /// <param name="item">The game object.</param>
        public void Insert(int index, GameObject item)
        {
            backingList.Insert(index, item);
        }

        /// <summary>
        /// Removes the first occurrence of a game object.
        /// </summary>
        /// <param name="item">The game object.</param>
        /// <returns>Returns true if successful.</returns>
        public bool Remove(GameObject item)
        {
            return backingList.Remove(item);
        }

        /// <summary>
        /// Removes a game object at the specified index.
        /// </summary>
        /// <param name="index">The index to remove at.</param>
        public void RemoveAt(int index)
        {
            backingList.RemoveAt(index);
        }

        /// <summary>
        /// Gets an enumerator for the collection.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return backingList.GetEnumerator();
        }

        GameObjectRenderer renderer = new GameObjectRenderer();

        /// <summary>
        /// Creates a new empty game object collection.
        /// </summary>
        public GameObjectCollection() { }

        /// <summary>
        /// Links (by reference) an existing list of game objects to be managed by this collection object.
        /// </summary>
        /// <param name="managedList">The game object list.</param>
        public GameObjectCollection(ref List<GameObject> managedList)
        {
            backingList = managedList;

            foreach (var item in backingList)
                item.RepositionRects = true;
        }

        /// <summary>
        /// Creates a new game object collection with the specified objects.
        /// </summary>
        /// <param name="initialObjects">The objects to add to this game collection.</param>
        public GameObjectCollection(GameObject[] initialObjects)
        {
            backingList.AddRange(initialObjects);

            foreach (var item in backingList)
                item.RepositionRects = true;
        }

        /// <summary>
        /// Draws all of the game objects to the screen.
        /// </summary>
        public void Render()
        {
            renderer.Start();

            for (int i = 0; i < backingList.Count; i++)
                if (backingList[i].IsVisible)
                    renderer.Render(backingList[i].Animation, GameObject.NoMatrix);

            renderer.End();
        }

        /// <summary>
        /// Processes the updates for each game object.
        /// </summary>
        public void Update()
        {
            for (int i = 0; i < backingList.Count; i++)
                backingList[i].Update();
        }

        [DataContract]
        private class GameObjectRenderer : Consoles.TextSurfaceRenderer
        {
#if SFML
            public void Start()
            {
                Batch.Reset(Engine.Device, RenderStates.Default, Transform.Identity);
                BeforeRenderCallback?.Invoke(Batch);
            }

            public override void Render(ITextSurfaceRendered surface, Matrix renderingMatrix)
            {
                if (surface.Tint.A != 255)
                {
                    Cell cell;

                    if (surface.DefaultBackground.A != 0)
                        Batch.DrawQuad(surface.AbsoluteArea, surface.Font.SolidGlyphRectangle, surface.DefaultBackground, surface.Font.FontImage);

                    for (int i = 0; i < surface.RenderCells.Length; i++)
                    {
                        cell = surface.RenderCells[i];

                        if (cell.IsVisible)
                        {
                            Batch.DrawCell(cell, surface.RenderRects[i], surface.Font.SolidGlyphRectangle, surface.DefaultBackground, surface.Font);
                        }
                    }

                }

                if (surface.Tint.A != 0)
                    Batch.DrawQuad(surface.AbsoluteArea, surface.Font.SolidGlyphRectangle, surface.Tint, surface.Font.FontImage);
            }

#elif MONOGAME
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
#endif
            public void End()
            {
                AfterRenderCallback?.Invoke(Batch);
                Batch.End();
            }
        }
    }
}
