using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScrollingConsole = SadConsole.ScrollingConsole;

namespace FeatureDemo.CustomConsoles
{
    // TODO: Something in here causes a lot of GC when this is toggled on.
    internal class SceneProjectionConsole : ScrollingConsole
    {
        private readonly RenderTarget2D _renderTexture;
        private Vector3 _boxPosition;
        private Vector3 _trianglePosition;
        private readonly VertexPositionColorNormal[] _vertices;
        private readonly VertexPositionColorNormal[] _triangle;
        private readonly BasicEffect _effect;
        private readonly RasterizerState rasterizerState = new RasterizerState();
        private float _angle = 0f;
        private bool toggle = true;
        private readonly bool blockMode = false;
        private readonly Color[] pixels;
        private readonly SadConsole.Readers.TextureToSurfaceReader reader1;

        public SceneProjectionConsole() : base(80, 25)
        {
            PresentationParameters pp = SadConsole.Global.GraphicsDevice.PresentationParameters;
            _renderTexture = new RenderTarget2D(SadConsole.Global.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, SadConsole.Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);

            _vertices = CreateBox();
            _triangle = CreateTriangle();

            _boxPosition = new Vector3(1.5f, 0.0f, -0.0f);
            _trianglePosition = new Vector3(-1.5f, 0.0f, -0.0f);

            _effect = new BasicEffect(SadConsole.Global.GraphicsDevice)
            {
                FogEnabled = false,
                TextureEnabled = false,
                LightingEnabled = false,
                VertexColorEnabled = true
            };
            //_effect.EnableDefaultLighting();
            pixels = new Color[_renderTexture.Width * _renderTexture.Height];
            rasterizerState.CullMode = CullMode.CullCounterClockwiseFace;

            reader1 = new SadConsole.Readers.TextureToSurfaceReader(_renderTexture.Width, _renderTexture.Height, SadConsole.Global.FontDefault);

            UseMouse = true;
            IsVisible = false;
            toggle = !toggle;
        }

        public override void Update(TimeSpan time)
        {

            base.Update(time);

            _angle -= MathHelper.ToRadians(0.5f);
            if (_angle < 0.0f)
            {
                _angle += MathHelper.TwoPi;
            }

            if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.T))
            {
                toggle = !toggle;
                Clear();
            }

            if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.B))
            {
                reader1.UseBlockMode = !reader1.UseBlockMode;
            }
        }


        public override void Draw(TimeSpan time)
        {
            if (IsVisible)
            {
                // Grab rendering
                SadConsole.Global.GraphicsDevice.SetRenderTarget(_renderTexture);

                SadConsole.Global.GraphicsDevice.RasterizerState = rasterizerState;

                Matrix worldMatrix = Matrix.CreateRotationY(_angle) * Matrix.CreateRotationX(-_angle) * Matrix.CreateTranslation(_boxPosition);

                _effect.World = worldMatrix;
                var camera = new Vector3(0, 0, 5);
                _effect.View = Matrix.CreateLookAt(camera, Vector3.Zero, Vector3.Up);
                _effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)_renderTexture.Width / _renderTexture.Height, 0.1f, 100.0f);

                // Calculate scene
                foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    // Render to texture
                    SadConsole.Global.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _vertices, 0, 12, VertexPositionColorNormal.VertexDeclaration);
                }

                worldMatrix = Matrix.CreateRotationY(_angle) * Matrix.CreateTranslation(_trianglePosition);
                _effect.World = worldMatrix;

                foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    // Render to texture
                    SadConsole.Global.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _triangle, 0, 2, VertexPositionColorNormal.VertexDeclaration);
                }

                if (toggle)
                {
                    reader1.GetSurface(_renderTexture).Copy(this);
                    base.Draw(time);
                }
                else
                {
                    SadConsole.Global.DrawCalls.Add(new SadConsole.DrawCalls.DrawCallTexture(_renderTexture, Position.ToVector2()));
                }
            }
        }

        private VertexPositionColorNormal[] CreateTriangle()
        {
            var vertices = new VertexPositionColorNormal[6];
            vertices[0].Position = new Vector3(-1f, -1f, 0f);
            vertices[0].Color = Color.Blue;
            vertices[0].Normal = new Vector3(1, 0, 1);
            vertices[1].Position = new Vector3(0, 1f, 0f);
            vertices[1].Color = Color.Green;
            vertices[1].Normal = new Vector3(1, 0, 1);
            vertices[2].Position = new Vector3(1f, -1f, 0f);
            vertices[2].Color = Color.White;
            vertices[2].Normal = new Vector3(1, 0, 1);
            vertices[3].Position = new Vector3(1f, -1f, 0f);
            vertices[3].Color = Color.White;
            vertices[3].Normal = new Vector3(1, 0, 1);
            vertices[4].Position = new Vector3(0, 1f, 0f);
            vertices[4].Color = Color.Green;
            vertices[4].Normal = new Vector3(1, 0, 1);
            vertices[5].Position = new Vector3(-1f, -1f, 0f);
            vertices[5].Color = Color.Blue;
            vertices[5].Normal = new Vector3(1, 0, 1);
            return vertices;
        }

        private VertexPositionColorNormal[] CreateBox()
        {
            var vertices = new VertexPositionColorNormal[]
            {
                
                // Front Surface
                new VertexPositionColorNormal(new Vector3(-1.0f, -1.0f, 1.0f), Color.Blue, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(-1.0f, 1.0f, 1.0f), Color.Red, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(1.0f, -1.0f, 1.0f), Color.Red, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(1.0f, -1.0f, 1.0f), Color.Red, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(-1.0f, 1.0f, 1.0f), Color.Red, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(1.0f, 1.0f, 1.0f), Color.Blue, new Vector3(1, 0, 1)),  

                // Back Surface
                new VertexPositionColorNormal(new Vector3(1.0f, -1.0f, -1.0f), Color.Yellow, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(1.0f, 1.0f, -1.0f), Color.Yellow, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(-1.0f, -1.0f, -1.0f), Color.Yellow, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(-1.0f, -1.0f, -1.0f), Color.Yellow, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(1.0f, 1.0f, -1.0f), Color.Yellow, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(-1.0f, 1.0f, -1.0f), Color.Yellow, new Vector3(1, 0, 1)), 

                
                // Left Surface
                new VertexPositionColorNormal(new Vector3(-1.0f, -1.0f, -1.0f), Color.Blue, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(-1.0f, 1.0f, -1.0f), Color.Blue, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(-1.0f, -1.0f, 1.0f), Color.Blue, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(-1.0f, -1.0f, 1.0f), Color.Blue, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(-1.0f, 1.0f, -1.0f), Color.Blue, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(-1.0f, 1.0f, 1.0f), Color.Blue, new Vector3(1, 0, 1)),

                
                // Right Surface
                new VertexPositionColorNormal(new Vector3(1.0f, -1.0f, 1.0f), Color.Violet, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(1.0f, 1.0f, 1.0f), Color.Violet, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(1.0f, -1.0f, -1.0f), Color.Violet, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(1.0f, -1.0f, -1.0f), Color.Violet, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(1.0f, 1.0f, 1.0f), Color.Violet, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(1.0f, 1.0f, -1.0f), Color.Violet, new Vector3(1, 0, 1)),
                

                // Top Surface
                new VertexPositionColorNormal(new Vector3(-1.0f, 1.0f, 1.0f), Color.Green, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(-1.0f, 1.0f, -1.0f), Color.Green, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(1.0f, 1.0f, 1.0f), Color.Green, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(1.0f, 1.0f, 1.0f), Color.Green, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(-1.0f, 1.0f, -1.0f), Color.Green, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(1.0f, 1.0f, -1.0f), Color.Green, new Vector3(1, 0, 1)),

                // Bottom Surface
                new VertexPositionColorNormal(new Vector3(-1.0f, -1.0f, -1.0f), Color.Orange, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(-1.0f, -1.0f, 1.0f), Color.Orange, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(1.0f, -1.0f, -1.0f), Color.Orange, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(1.0f, -1.0f, -1.0f), Color.Orange, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(-1.0f, -1.0f, 1.0f), Color.Orange, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(1.0f, -1.0f, 1.0f), Color.Orange, new Vector3(1, 0, 1)),
            };

            return vertices;
        }

        public struct VertexPositionColorNormal
        {
            public Vector3 Position;
            public Color Color;
            public Vector3 Normal;

            public VertexPositionColorNormal(Vector3 position, Color color, Vector3 normal)
            {
                Position = position;
                Color = color;
                Normal = normal;
            }

            public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
            );

        }
    }
}
