using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SadConsole;
using SadConsole.Host;
using SadRogue.Primitives;
using Console = SadConsole.Console;
using MonoColor = Microsoft.Xna.Framework.Color;
using Settings = SadConsole.Settings;

namespace FeatureDemo.CustomConsoles
{
    internal class TextureManipulation : Console
    {
        public TextureManipulation() : base(Program.MainWidth, Program.MainHeight)
        {
            AddCentered(new Canvas());
        }

        private void AddCentered(Console c)
        {
            Children.Add(c);
            c.UsePixelPositioning = true;
            c.Position = (Settings.Rendering.RenderWidth / 2 - c.AbsoluteArea.Width / 2,
                Settings.Rendering.RenderHeight / 2 - c.AbsoluteArea.Height / 2 - 16);
        }
    }

    class Canvas : Console
    {
        const int MaxLines = 10;
        Line[] _lines = new Line[MaxLines];
        MonoColor[] _colors;
        int _frameNumberForNewLine = 0, _frameCounter = 0, _lineCounter = 0;

        public Canvas() : base(3, 3)
        {
            Font = GameHost.Instance.LoadFont("Res/Fonts/empty_font.font");
            FontSize = (110, 110);

            DefaultBackground = Color.DarkCyan;
            Surface.Clear();

            Children.Add(new OversizedFont());

            _lines[0] = new Line();

            var texture = Font.Image as GameTexture;

            if (texture is GameTexture t)
            {
                _colors = new MonoColor[t.Width * t.Height];
                t.Texture.GetData(_colors);

                for (int y = 0, i = 0; y < t.Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        Surface.SetGlyph(x, y, i++);
                    }
                }
            }
            else throw new NullReferenceException();

            SetNewFrameNumberTarget();
        }

        void SetNewFrameNumberTarget() => _frameNumberForNewLine = GameHost.Instance.Random.Next(20, 200);

        public override void Render(TimeSpan delta)
        {
            base.Render(delta);
        }

        public override void Update(TimeSpan delta)
        {
            base.Update(delta);

            Array.Clear(_colors, 0, _colors.Length);

            if (_lineCounter < MaxLines - 1 && _frameCounter++ > _frameNumberForNewLine)
            {
                _frameCounter = 0;
                SetNewFrameNumberTarget();

                _lines[++_lineCounter] = new Line();
            }

            foreach (var line in _lines)
            {
                if (line != null)
                    line.Draw(_colors);
            }

            if (Font.Image is GameTexture t)
                 t.Texture.SetData(_colors);
        }

        class OversizedFont : ScreenSurface
        {
            public OversizedFont() : base(1, 1)
            {
                FontSize = (110, 110);
                Surface.Print(0, 0, "A");
                Position = (1, 1);
            }
        }

        class Line
        {
            static readonly Rectangle s_surface = new Rectangle(0, 0, 96, 96);
            readonly int _maxIndex;
            readonly Dir _direction;
            MonoColor _color = RandomColor.ToMonoColor();
            Point _start;
            Point _end;
            Sides _side = Sides.None;

            public Line()
            {
                _maxIndex = s_surface.Width - 1;
                _direction = RandomDirection;
                _start = (0, 0);
                _end = GetOppositePoint(_start);
                EstablishSide();
            }

            static Color RandomColor => Color.White.GetRandomColor(GameHost.Instance.Random);

            static Dir RandomDirection => GameHost.Instance.Random.Next(0, 2) switch
            {
                0 => Dir.Clockwise,
                _ => Dir.Counteclockwise
            };

            Point GetOppositePoint(Point p) => (_maxIndex - p.X, _maxIndex - p.Y);

            void ChangeSide()
            {
                if (_direction == Dir.Clockwise)
                {
                    _side = _side switch
                    {
                        Sides.Top => Sides.Right,
                        Sides.Right => Sides.Bottom,
                        Sides.Bottom => Sides.Left,
                        _ => Sides.Top
                    };

                    _start = _side switch
                    {
                        Sides.Top => (0, 0),
                        Sides.Right => (_maxIndex, 0),
                        Sides.Bottom => (_maxIndex, _maxIndex),
                        _ => (0, _maxIndex)
                    };
                }
                else
                {
                    _side = _side switch
                    {
                        Sides.Top => Sides.Left,
                        Sides.Right => Sides.Top,
                        Sides.Bottom => Sides.Right,
                        _ => Sides.Bottom
                    };

                    _start = _side switch
                    {
                        Sides.Top => (_maxIndex, 0),
                        Sides.Right => (_maxIndex, _maxIndex),
                        Sides.Bottom => (0, _maxIndex),
                        _ => (0, 0)
                    };
                }
            }

            public void Move()
            {
                if (_direction == Dir.Clockwise)
                {
                    _start = _side switch
                    {
                        Sides.Top => _start + Direction.Right,
                        Sides.Right => _start + Direction.Down,
                        Sides.Bottom => _start + Direction.Left,
                        _ => _start + Direction.Up
                    };
                }
                else
                {
                    _start = _side switch
                    {
                        Sides.Top => _start + Direction.Left,
                        Sides.Right => _start + Direction.Up,
                        Sides.Bottom => _start + Direction.Right,
                        _ => _start + Direction.Down
                    };
                }

                if (!s_surface.Positions().Contains(_start))
                    ChangeSide();

                _end = GetOppositePoint(_start);
            }

            public void Draw(MonoColor[] surface)
            {
                Move();

                Algorithms.Line(_start.X, _start.Y, _end.X, _end.Y, (x, y) =>
                {
                    int i = new Point(x, y).ToIndex(s_surface.Width);
                    if (i >= 0 && i < surface.Length)
                        surface[i] = _color;
                    return false;
                });
            }

            void EstablishSide()
            {
                if (_direction == Dir.Clockwise)
                {
                    _side = _start == (0, 0) ? Sides.Top :
                            _start == (0, _maxIndex) ? Sides.Right :
                            _start == (_maxIndex, _maxIndex) ? Sides.Bottom :
                            Sides.Left;

                }
                else
                {
                    _side = _start == (0, 0) ? Sides.Left :
                            _start == (0, _maxIndex) ? Sides.Top :
                            _start == (_maxIndex, _maxIndex) ? Sides.Right :
                            Sides.Bottom;
                }

                if (_side == Sides.None)
                {
                    _side = s_surface.IsOnSide(_start, Direction.Up) ? Sides.Top :
                            s_surface.IsOnSide(_start, Direction.Right) ? Sides.Right :
                            s_surface.IsOnSide(_start, Direction.Down) ? Sides.Bottom :
                            Sides.Left;
                }
            }
        }

        public enum Sides
        {
            Top,
            Right,
            Bottom,
            Left,
            None
        }

        public enum Dir
        {
            Clockwise,
            Counteclockwise
        }
    }
}
