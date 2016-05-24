using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Consoles;
using System;
using System.Collections.Generic;
using System.Text;
using Console = SadConsole.Consoles.Console;
using SadConsole.Input;

namespace StarterProject.CustomConsoles
{
    public class SceneProjectionConsole : Console
    {
        private RenderTarget2D _renderTexture;
        private Vector3 _boxPosition;
        private Vector3 _trianglePosition;
        private VertexPositionColorNormal[] _vertices;
        private VertexPositionColorNormal[] _triangle;
        private BasicEffect _effect;
        private RasterizerState rasterizerState = new RasterizerState();
        private float _angle = 0f;
        private bool toggle = true;
        private bool blockMode = false;
        Color[] pixels;
        
        public SceneProjectionConsole(int width, int height) : base(width, height)
        {
            PresentationParameters pp = SadConsole.Engine.Device.PresentationParameters;
            _renderTexture = new RenderTarget2D(SadConsole.Engine.Device, pp.BackBufferWidth, pp.BackBufferHeight, false, SadConsole.Engine.Device.DisplayMode.Format, DepthFormat.Depth24);

            _vertices = CreateBox();
            _triangle = CreateTriangle();

            _boxPosition = new Vector3(1.5f, 0.0f, -0.0f);
            _trianglePosition = new Vector3(-1.5f, 0.0f, -0.0f);

            _effect = new BasicEffect(SadConsole.Engine.Device);
            _effect.FogEnabled = false;
            _effect.TextureEnabled = false;
            _effect.LightingEnabled = false;
            _effect.VertexColorEnabled = true;
            //_effect.EnableDefaultLighting();
            pixels = new Color[_renderTexture.Width * _renderTexture.Height];
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.CullCounterClockwiseFace;

            CanUseMouse = true;
        }
        
        public override void Update()
        {

            base.Update();

            _angle -= MathHelper.ToRadians(0.5f);
            if (_angle < 0.0f)
                _angle += MathHelper.TwoPi;

            if (SadConsole.Engine.Keyboard.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.T))
            {
                toggle = !toggle;
                _textSurface.Clear();
            }

            if (SadConsole.Engine.Keyboard.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.B))
            {
                blockMode = !blockMode;
            }
        }
        public override bool ProcessMouse(MouseInfo info)
        {
            base.ProcessMouse(info);

            if (info.LeftClicked)
            {
                var test = info.ConsoleLocation;
            }

            return true;
        }

        public override void Render()
        {
            if (IsVisible)
            {
                // Grab rendering
                if (toggle)
                    SadConsole.Engine.Device.SetRenderTarget(_renderTexture);

                SadConsole.Engine.Device.RasterizerState = rasterizerState;

                var worldMatrix = Matrix.CreateRotationY(_angle) * Matrix.CreateRotationX(-_angle) * Matrix.CreateTranslation(_boxPosition);

                _effect.World = worldMatrix;
                var camera = new Vector3(0, 0, 5);
                _effect.View = Matrix.CreateLookAt(camera, Vector3.Zero, Vector3.Up);
                _effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)_renderTexture.Width / _renderTexture.Height, 0.1f, 100.0f);

                // Calculate scene
                foreach (var pass in _effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    // Render to texture
                    SadConsole.Engine.Device.DrawUserPrimitives(PrimitiveType.TriangleList, _vertices, 0, 12, VertexPositionColorNormal.VertexDeclaration);
                }

                worldMatrix = Matrix.CreateRotationY(_angle) * Matrix.CreateTranslation(_trianglePosition);
                _effect.World = worldMatrix;

                foreach (var pass in _effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    // Render to texture
                    SadConsole.Engine.Device.DrawUserPrimitives(PrimitiveType.TriangleList, _triangle, 0, 2, VertexPositionColorNormal.VertexDeclaration);
                }

                if (toggle)
                {
                    SadConsole.Engine.Device.SetRenderTarget(null);
                    _renderTexture.ToSurface(_textSurface, pixels, blockMode);
                }

                base.Render();
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

            public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
            );

        }
    }
}
