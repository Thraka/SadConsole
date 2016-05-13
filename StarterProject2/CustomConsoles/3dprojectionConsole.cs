using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Consoles;
using System;
using System.Collections.Generic;
using System.Text;
using Console = SadConsole.Consoles.Console;

namespace StarterProject.CustomConsoles
{
    public class _3dprojectionConsole : Console
    {
        private RenderTarget2D _renderTexture;
        private VertexPositionColorNormal[] _vertices;
        private BasicEffect _effect;

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


        public _3dprojectionConsole(int width, int height) : base(width, height)
        {
            PresentationParameters pp = SadConsole.Engine.Device.PresentationParameters;
            _renderTexture = new RenderTarget2D(SadConsole.Engine.Device, pp.BackBufferWidth, pp.BackBufferHeight, false, SadConsole.Engine.Device.DisplayMode.Format, DepthFormat.Depth24);
            _vertices = CreateBox();
            _effect = new BasicEffect(SadConsole.Engine.Device);
            _effect.FogEnabled = false;
            _effect.TextureEnabled = false;
            _effect.LightingEnabled = false;
            _effect.VertexColorEnabled = true;
            //_effect.EnableDefaultLighting();

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.CullClockwiseFace;
            SadConsole.Engine.Device.RasterizerState = rasterizerState;
            SadConsole.Engine.Device.DepthStencilState = DepthStencilState.DepthRead;

        }

        private float _angle = 0f;

        public override void Update()
        {

            base.Update();

            _angle -= MathHelper.ToRadians(0.5f);
            if (_angle < 0.0f)
                _angle += MathHelper.TwoPi;
        }

        public override void Render()
        {
            if (IsVisible)
            {
                // Grab rendering
                //SadConsole.Engine.Device.SetRenderTarget(_renderTexture);

                var worldMatrix = Matrix.CreateRotationY(_angle) * Matrix.CreateRotationX(-_angle) * Matrix.CreateTranslation(new Vector3(1.5f, 0.0f, -7.0f));
                worldMatrix = Matrix.CreateRotationX(-_angle) * Matrix.CreateTranslation(new Vector3(1.5f, 0.0f, -7.0f));

                _effect.World = worldMatrix;
                _effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)_renderTexture.Width / _renderTexture.Height, 0.1f, 100.0f);


                // Calculate scene
                foreach (var pass in _effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    // Render to texture
                    SadConsole.Engine.Device.DrawUserPrimitives(PrimitiveType.TriangleList, _vertices, 0, 4, VertexPositionColorNormal.VertexDeclaration);
                }

                // restore
                SadConsole.Engine.Device.SetRenderTarget(null);

                // Process texture to cells
                TranslateImageToTextSurface((Texture2D)_renderTexture, _textSurface);

                base.Render();

            }
        }

        private VertexPositionColorNormal[] CreateTriangle()
        {
            var vertices = new VertexPositionColorNormal[3];
            vertices[0].Position = new Vector3(-0.5f, -0.5f, 0f);
            vertices[0].Color = Color.Red;
            vertices[0].Normal = new Vector3(1, 0, 1);
            vertices[1].Position = new Vector3(0, 0.5f, 0f);
            vertices[1].Color = Color.Green;
            vertices[1].Normal = new Vector3(1, 0, 1);
            vertices[2].Position = new Vector3(0.5f, -0.5f, 0f);
            vertices[2].Color = Color.Yellow;
            vertices[2].Normal = new Vector3(1, 0, 1);
            return vertices;
        }

        private VertexPositionColorNormal[] CreateBox()
        {
            var vertices = new VertexPositionColorNormal[]
            {
                
                // Front Surface
                new VertexPositionColorNormal(new Vector3(-1.0f, -1.0f, 1.0f), Color.Red, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(-1.0f, 1.0f, 1.0f), Color.Red, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(1.0f, -1.0f, 1.0f), Color.Red, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(1.0f, -1.0f, 1.0f), Color.Red, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(-1.0f, 1.0f, 1.0f), Color.Red, new Vector3(1, 0, 1)),
                new VertexPositionColorNormal(new Vector3(1.0f, 1.0f, 1.0f), Color.Red, new Vector3(1, 0, 1)),  

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

        private void TranslateImageToTextSurface(Texture2D image, TextSurface surface)
        {
            Color[] pixels = new Color[image.Width * image.Height];
            image.GetData<Color>(pixels);

            for (int h = 0; h < image.Height / surface.Font.Size.Y; h++)
            {
                int startY = (h * surface.Font.Size.Y);
                for (int w = 0; w < image.Width / surface.Font.Size.X; w++)
                {
                    int startX = (w * surface.Font.Size.X);

                    float allR = 0;
                    float allG = 0;
                    float allB = 0;
                    float allBri = 0;

                    for (int y = 0; y < surface.Font.Size.Y; y++)
                    {
                        for (int x = 0; x < surface.Font.Size.X; x++)
                        {
                            int cY = y + startY;
                            int cX = x + startX;

                            Color color = pixels[cY * image.Width + cX];

                            allR += color.R;
                            allG += color.G;
                            allB += color.B;
                            allBri += color.GetBrightness();
                        }
                    }

                    // This only works for square fonts... ack??
                    float sr = (allR / (surface.Font.Size.X * surface.Font.Size.Y));
                    float sg = (allG / (surface.Font.Size.X * surface.Font.Size.Y));
                    float sb = (allB / (surface.Font.Size.X * surface.Font.Size.Y));

                    var newColor = new Color(sr, sg, sb);

                    float sbri = ((allBri * 255) / (surface.Font.Size.X * surface.Font.Size.Y));

                    if (sbri > 230)
                        surface.SetCharacter(w, h, (int)'#', newColor);
                    else if (sbri > 207)
                        surface.SetCharacter(w, h, (int)'&', newColor);
                    else if (sbri > 184)
                        surface.SetCharacter(w, h, (int)'$', newColor);
                    else if (sbri > 161)
                        surface.SetCharacter(w, h, (int)'X', newColor);
                    else if (sbri > 138)
                        surface.SetCharacter(w, h, (int)'x', newColor);
                    else if (sbri > 115)
                        surface.SetCharacter(w, h, (int)'=', newColor);
                    else if (sbri > 92)
                        surface.SetCharacter(w, h, (int)'+', newColor);
                    else if (sbri > 69)
                        surface.SetCharacter(w, h, (int)';', newColor);
                    else if (sbri > 46)
                        surface.SetCharacter(w, h, (int)':', newColor);
                    else if (sbri > 23)
                        surface.SetCharacter(w, h, (int)'.', newColor);
                    else
                        surface.SetCharacter(w, h, 0);
                }
            }
        }
    }
}
