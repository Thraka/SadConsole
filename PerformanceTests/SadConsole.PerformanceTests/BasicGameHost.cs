﻿using System;
using System.IO;
using SadConsole.Input;
using SadConsole.Renderers;
using SadRogue.Primitives;

namespace SadConsole.PerformanceTests
{
    /// <summary>
    /// Very basic (placeholder) GameHost that will fulfill the requirements of SadConsole's GameHost to allow us to test classes that depend on the host's functionality.
    /// </summary>
    class BasicGameHost : GameHost
    {
        public class RenderStep : IRenderStep
        {
            public string Name => "Nothing";
            public uint SortOrder { get => 1; set => throw new NotImplementedException(); }
            public void Composing(IRenderer renderer, IScreenSurface screenObject) => throw new NotImplementedException();
            public void Dispose() => throw new NotImplementedException();
            public bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced) => throw new NotImplementedException();
            public void Render(IRenderer renderer, IScreenSurface screenObject) => throw new NotImplementedException();
            public void Reset() => throw new NotImplementedException();
            public void SetData(object data) { }
        }

        public class Texture : ITexture
        {
            private SixLabors.ImageSharp.Image _graphic;

            public string ResourcePath { get; private set; }

            public int Height => _graphic.Height;

            public int Width => _graphic.Width;

            public int Size => Width * Height;

            public void Dispose()
            {
                _graphic.Dispose();
            }
            public Color GetPixel(Point position) => throw new NotImplementedException();
            public Color GetPixel(int index) => throw new NotImplementedException();
            public Color[] GetPixels() => throw new NotImplementedException();
            public void SetPixels(Color[] pixels) => throw new NotImplementedException();
            public void SetPixel(Point position, Color color) => throw new NotImplementedException();
            public void SetPixel(int index, Color color) => throw new NotImplementedException();
            public ICellSurface ToSurface(TextureConvertMode mode, int surfaceWidth, int surfaceHeight, TextureConvertBackgroundStyle backgroundStyle = TextureConvertBackgroundStyle.Pixel, TextureConvertForegroundStyle foregroundStyle = TextureConvertForegroundStyle.Block, Color[] cachedColorArray = null, ICellSurface cachedSurface = null) => throw new NotImplementedException();

            public void SetPixels(ReadOnlySpan<Color> colors)
            {
                throw new NotImplementedException();
            }

            public Texture(string path)
            {
                using (Stream fontStream = new FileStream(path, FileMode.Open))
                    _graphic = SixLabors.ImageSharp.Image.Load(fontStream);

                ResourcePath = path;
            }

            public Texture(Stream textureStream)
            {
                _graphic = SixLabors.ImageSharp.Image.Load(textureStream);
            }
        }


        public BasicGameHost()
        {
            Instance = this;
            base.LoadDefaultFonts("");
        }

        public override IKeyboardState GetKeyboardState()
        {
            throw new NotImplementedException();
        }

        public override IMouseState GetMouseState()
        {
            throw new NotImplementedException();
        }

        public override void GetDeviceScreenSize(out int width, out int height)
        {
            width = 1920;
            height = 1080;
        }

        public override IRenderer GetRenderer(string name)
        {
            return null;
        }

        public override ITexture GetTexture(string resourcePath)
        {
            return new Texture(resourcePath);
        }

        public override ITexture GetTexture(Stream textureStream)
        {
            return new Texture(textureStream);
        }

        public override IRenderStep GetRendererStep(string name)
        {
            return new RenderStep();
        }

        public override void Run()
        {
            throw new NotImplementedException();
        }

        public override ITexture CreateTexture(int width, int height)
        {
            throw new NotImplementedException();
        }

        public override void ResizeWindow(int width, int height, bool resizeOutputSurface = false)
        {
            throw new NotImplementedException();
        }
    }
}
