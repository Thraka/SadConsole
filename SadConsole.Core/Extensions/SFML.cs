#if SFML
// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.


namespace SFML.Graphics
{
    using global::System;

    /// <summary>
    /// Defines sprite visual options for mirroring.
    /// </summary>
    [Flags]
    public enum SpriteEffects
    {
        /// <summary>
        /// No options specified.
        /// </summary>
		None = 0,
        /// <summary>
        /// Render the sprite reversed along the X axis.
        /// </summary>
        FlipHorizontally = 1,
        /// <summary>
        /// Render the sprite reversed along the Y axis.
        /// </summary>
        FlipVertically = 2
    }

    public static class ColorHelper
    {
        /// <summary>
        /// Performs linear interpolation of <see cref="Color"/>.
        /// </summary>
        /// <param name="value1">Source <see cref="Color"/>.</param>
        /// <param name="value2">Destination <see cref="Color"/>.</param>
        /// <param name="amount">Interpolation factor.</param>
        /// <returns>Interpolated <see cref="Color"/>.</returns>
        public static Color Lerp(Color value1, Color value2, float amount)
        {
            amount = SadConsole.MathHelper.Clamp(amount, 0, 1);
            return new Color(
                (byte)SadConsole.MathHelper.Lerp(value1.R, value2.R, amount),
                (byte)SadConsole.MathHelper.Lerp(value1.G, value2.G, amount),
                (byte)SadConsole.MathHelper.Lerp(value1.B, value2.B, amount),
                (byte)SadConsole.MathHelper.Lerp(value1.A, value2.A, amount));
        }

    }

    public static class RectHelper
    {
        public static bool Contains(this IntRect rect, IntRect value)
        {
            return ((((rect.Left <= value.Left) && ((value.Left + value.Width) <= (rect.Left + rect.Width))) && (rect.Top <= value.Top)) && ((value.Top + value.Height) <= (rect.Top + rect.Height)));
        }
    }


    public class SpriteBatch : Transformable, Drawable
    {
        Vertex[] m_verticies;
        SadConsole.Font font;

        public unsafe void Update(SadConsole.Consoles.ITextSurfaceRendered surface)
        {
            this.font = surface.Font;
            
            //    m_vertices.setPrimitiveType(sf::Quads);
            //    m_vertices.resize(width * height * 4);
            //m_verticies = new VertexArray(PrimitiveType.Quads, ((uint)(surface.RenderCells.Length * 4)));

            //    // populate the vertex array, with one quad per tile
            //    for (unsigned int i = 0; i < width; ++i)
            //        for (unsigned int j = 0; j < height; ++j)
            //        {
            uint counter = 0;

            fixed (Vertex* verts = m_verticies)
            {


                for (int y = 0; y < surface.RenderArea.Height; y++)
                {
                    for (int x = 0; x < surface.RenderArea.Width; x++)
                    {
                        uint quadIndex = counter * 4;

                        verts[counter].Position = new System.Vector2f(surface.RenderRects[counter].Left, surface.RenderRects[counter].Top);
                        verts[counter + 1].Position = new System.Vector2f(surface.RenderRects[counter].Left + surface.RenderRects[counter].Width, surface.RenderRects[counter].Top);
                        verts[counter + 2].Position = new System.Vector2f(surface.RenderRects[counter].Left + surface.RenderRects[counter].Width, surface.RenderRects[counter].Top + surface.RenderRects[counter].Height);
                        verts[counter + 3].Position = new System.Vector2f(surface.RenderRects[counter].Left, surface.RenderRects[counter].Top + surface.RenderRects[counter].Height);

                        verts[counter].TexCoords = new System.Vector2f(font.GlyphIndexRects[surface.RenderCells[counter].ActualGlyphIndex].Left / font.FontImage.Size.X,
                                                                       font.GlyphIndexRects[surface.RenderCells[counter].ActualGlyphIndex].Top / font.FontImage.Size.Y);
                        verts[counter + 1].TexCoords = new System.Vector2f((surface.RenderRects[counter].Left + surface.RenderRects[counter].Width) / font.FontImage.Size.X, 
                                                                           surface.RenderRects[counter].Top / font.FontImage.Size.Y);
                        verts[counter + 2].TexCoords = new System.Vector2f((surface.RenderRects[counter].Left + surface.RenderRects[counter].Width) / font.FontImage.Size.X, 
                                                                            (surface.RenderRects[counter].Top + surface.RenderRects[counter].Height) / font.FontImage.Size.Y);
                        verts[counter + 3].TexCoords = new System.Vector2f(surface.RenderRects[counter].Left, 
                                                                           (surface.RenderRects[counter].Top + surface.RenderRects[counter].Height) / font.FontImage.Size.Y);
                        counter++;
                    }
                }
            }
        }

        //public bool load(const std::string& tileset, sf::Vector2u tileSize, const int* tiles, unsigned int width, unsigned int height)
        //{
        //    // load the tileset texture
        //    if (!m_tileset.loadFromFile(tileset))
        //        return false;

        //    // resize the vertex array to fit the level size
        //    m_vertices.setPrimitiveType(sf::Quads);
        //    m_vertices.resize(width * height * 4);

        //    // populate the vertex array, with one quad per tile
        //    for (unsigned int i = 0; i < width; ++i)
        //        for (unsigned int j = 0; j < height; ++j)
        //        {
        //            // get the current tile number
        //            int tileNumber = tiles[i + j * width];

        //            // find its position in the tileset texture
        //            int tu = tileNumber % (m_tileset.getSize().x / tileSize.x);
        //            int tv = tileNumber / (m_tileset.getSize().x / tileSize.x);

        //            // get a pointer to the current tile's quad
        //            sf::Vertex* quad = &m_vertices[(i + j * width) * 4];

        //            // define its 4 corners
        //            quad[0].position = sf::Vector2f(i * tileSize.x, j * tileSize.y);
        //            quad[1].position = sf::Vector2f((i + 1) * tileSize.x, j * tileSize.y);
        //            quad[2].position = sf::Vector2f((i + 1) * tileSize.x, (j + 1) * tileSize.y);
        //            quad[3].position = sf::Vector2f(i * tileSize.x, (j + 1) * tileSize.y);

        //            // define its 4 texture coordinates
        //            quad[0].texCoords = sf::Vector2f(tu * tileSize.x, tv * tileSize.y);
        //            quad[1].texCoords = sf::Vector2f((tu + 1) * tileSize.x, tv * tileSize.y);
        //            quad[2].texCoords = sf::Vector2f((tu + 1) * tileSize.x, (tv + 1) * tileSize.y);
        //            quad[3].texCoords = sf::Vector2f(tu * tileSize.x, (tv + 1) * tileSize.y);
        //        }

        //    return true;
        //}

        public void Draw(RenderTarget target, RenderStates states)
        {

            // apply the transform
            //states.transform *= getTransform();
            states.Transform *= Transform;

            // apply the tileset texture
            //states.texture = &m_tileset;
            states.Texture = font.FontImage;

            // draw the vertex array
            //target.draw(m_vertices, states);
            //target.Draw(m_verticies, states);
            target.Draw(m_verticies, PrimitiveType.Quads);
        }

    }

   

//namespace SFML.Utils
//    {
//        using System;
//        using SFML.Graphics;
//        using SFML.Window;
//        /// <summary>
//        /// Functions that provides color/texture rectangle data from tile map (or other source)
//        /// </summary>
//        public delegate void TileProvider(int x, int y, int layer, out Color color, out IntRect rec);

//        /// <summary>
//        /// Fast and universal renderer of tilemaps
//        /// </summary>
//        class MapRenderer : Drawable
//        {
//            private readonly float TileSize;
//            public readonly int Layers;

//            private int height;
//            private int width;

//            /// <summary>
//            /// Points to the tile in the top left corner
//            /// </summary>
//            private Vector2i offset;
//            private Vertex[] vertices;

//            /// <summary>
//            /// Provides Color and Texture Source from tile map
//            /// </summary>
//            private TileProvider provider;

//            /// <summary>
//            /// Holds spritesheet
//            /// </summary>
//            private Texture texture;

//            /// <param name="texture">Spritesheet</param>
//            /// <param name="provider">Accesor to tilemap data</param>
//            /// <param name="tileSize">Size of one tile</param>
//            /// <param name="layers">Numbers of layers</param>
//            public MapRenderer(Texture texture, TileProvider provider, float tileSize = 16, int layers = 1)
//            {
//                if (provider == null || layers <= 0) throw new ArgumentException();
//                this.provider = provider;

//                TileSize = tileSize;
//                Layers = layers;

//                vertices = new Vertex[0];
//                this.texture = texture;

//            }

//            /// <summary>
//            /// Redraws whole screen
//            /// </summary>
//            public void Refresh()
//            {
//                RefreshLocal(0, 0, width, height);
//            }

//            private void RefreshLocal(int left, int top, int right, int bottom)
//            {
//                for (var y = top; y < bottom; y++)
//                    for (var x = left; x < right; x++)
//                    {
//                        Refresh(x + offset.X, y + offset.Y);
//                    }
//            }

//            /// <summary>
//            /// Ensures that vertex array has enough space
//            /// </summary>
//            /// <param name="v">Size of the visible area</param>
//            private void SetSize(Vector2f v)
//            {
//                var w = (int)(v.X / TileSize) + 2;
//                var h = (int)(v.Y / TileSize) + 2;
//                if (w == width && h == height) return;

//                width = w;
//                height = h;

//                vertices = new Vertex[width * height * 4 * Layers];
//                Refresh();
//            }

//            /// <summary>
//            /// Sets offset
//            /// </summary>
//            /// <param name="v">World position of top left corner of the screen</param>
//            private void SetCorner(Vector2f v)
//            {
//                var tile = GetTile(v);
//                var dif = tile - offset;
//                if (dif.X == 0 && dif.Y == 0) return;
//                offset = tile;

//                if (Math.Abs(dif.X) > width / 4 || Math.Abs(dif.Y) > height / 4)
//                {
//                    //Refresh everyting if difference is too big
//                    Refresh();
//                    return;
//                }

//                //Refresh only tiles that appeared since last update

//                if (dif.X > 0) RefreshLocal(width - dif.X, 0, width, height);
//                else RefreshLocal(0, 0, -dif.X, height);

//                if (dif.Y > 0) RefreshLocal(0, height - dif.Y, width, height);
//                else RefreshLocal(0, 0, width, -dif.Y);
//            }

//            /// <summary>
//            /// Transforms from world size to to tile that is under that position
//            /// </summary>
//            private Vector2i GetTile(Vector2f pos)
//            {
//                var x = (int)(pos.X / TileSize);
//                var y = (int)(pos.Y / TileSize);
//                if (pos.X < 0) x--;
//                if (pos.Y < 0) y--;
//                return new Vector2i(x, y);
//            }

//            /// <summary>
//            /// Redraws one tile
//            /// </summary>
//            /// <param name="x">X coord of the tile</param>
//            /// <param name="y">Y coord of the tile</param>
//            public void Refresh(int x, int y)
//            {
//                if (x < offset.X || x >= offset.X + width || y < offset.Y || y >= offset.Y + height)
//                    return; //check if tile is visible

//                //vertices works like 2d ring buffer
//                var vx = x % width;
//                var vy = y % height;
//                if (vx < 0) vx += width;
//                if (vy < 0) vy += height;

//                var index = (vx + vy * width) * 4 * Layers;
//                var rec = new FloatRect(x * TileSize, y * TileSize, TileSize, TileSize);

//                for (int i = 0; i < Layers; i++)
//                {
//                    Color color;
//                    IntRect src;
//                    provider(x, y, i, out color, out src);

//                    Draw(index, rec, src, color);
//                    index += 4;
//                }
//            }

//            /// <summary>
//            /// Inserts color and texture data into vertex array
//            /// </summary>
//            private unsafe void Draw(int index, FloatRect rec, IntRect src, Color color)
//            {
//                fixed (Vertex* fptr = vertices)
//                {
//                    //use pointers to avoid array bound checks (optimization)

//                    var ptr = fptr + index;

//                    ptr->Position.X = rec.Left;
//                    ptr->Position.Y = rec.Top;
//                    ptr->TexCoords.X = src.Left;
//                    ptr->TexCoords.Y = src.Top;
//                    ptr->Color = color;
//                    ptr++;

//                    ptr->Position.X = rec.Left + rec.Width;
//                    ptr->Position.Y = rec.Top;
//                    ptr->TexCoords.X = src.Left + src.Width;
//                    ptr->TexCoords.Y = src.Top;
//                    ptr->Color = color;
//                    ptr++;

//                    ptr->Position.X = rec.Left + rec.Width;
//                    ptr->Position.Y = rec.Top + rec.Height;
//                    ptr->TexCoords.X = src.Left + src.Width;
//                    ptr->TexCoords.Y = src.Top + src.Height;
//                    ptr->Color = color;
//                    ptr++;

//                    ptr->Position.X = rec.Left;
//                    ptr->Position.Y = rec.Top + rec.Height;
//                    ptr->TexCoords.X = src.Left;
//                    ptr->TexCoords.Y = src.Top + src.Height;
//                    ptr->Color = color;
//                }
//            }

//            /// <summary>
//            /// Update offset (based on Target's position) and draw it
//            /// </summary>
//            public void Draw(RenderTarget rt, RenderStates states)
//            {
//                var view = rt.GetView();
//                states.Texture = texture;
//                SetSize(view.Size);
//                SetCorner(rt.MapPixelToCoords(new Vector2i()));

//                rt.Draw(vertices, PrimitiveType.Quads, states);
//            }
//        }
//    }
}
    


namespace SadConsole
{
    using System;

    public class GameTime
    {
        public TimeSpan TotalGameTime { get; set; }

        public TimeSpan ElapsedGameTime { get; set; }

        public bool IsRunningSlowly { get; set; }

        public GameTime()
        {
            TotalGameTime = TimeSpan.Zero;
            ElapsedGameTime = TimeSpan.Zero;
            IsRunningSlowly = false;
        }

        public GameTime(TimeSpan totalGameTime, TimeSpan elapsedGameTime)
        {
            TotalGameTime = totalGameTime;
            ElapsedGameTime = elapsedGameTime;
            IsRunningSlowly = false;
        }

        public GameTime(TimeSpan totalRealTime, TimeSpan elapsedRealTime, bool isRunningSlowly)
        {
            TotalGameTime = totalRealTime;
            ElapsedGameTime = elapsedRealTime;
            IsRunningSlowly = isRunningSlowly;
        }
    }
}

#endif