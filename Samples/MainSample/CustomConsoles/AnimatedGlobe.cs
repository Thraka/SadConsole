using System.Linq;
using SadConsole;
using SadConsole.Input;
using SadRogue.Primitives;

namespace FeatureDemo.CustomConsoles
{
    internal class AnimatedGlobe : Console
    {
        public AnimatedGlobe() : base(80, 23)
        {
            // random noise simulating stars
            var staticNoise = AnimatedScreenSurface.CreateStatic(80, 23, 48, 0.9d);
            staticNoise.AnimationDuration = 48 * 0.1f;
            Children.Add(staticNoise);

            // the animated globe
            var clip = new Clip(this);
            clip.Start();
        }

    }

    class Clip : AnimatedScreenSurface
    {
        static readonly (int Width, int Height, int Count, float Duration) _frame = (46, 46, 48, 0.17f);
        readonly Rectangle _frameArea;

        public Clip(SadConsole.Console c) : base("Animated Globe", _frame.Width, _frame.Height / 2)
        {
            Parent = c;
            int x = (c.Width - Surface.Width) / 2;
            Position = (x, 0);

            // used to advance the point on the film where frames are copied from 
            _frameArea = new(0, 0, _frame.Width, _frame.Height);

            // convert png
            using ITexture sadImage = GameHost.Instance.GetTexture("Res/Images/globe.png");
            var fontSize = Game.Instance.DefaultFont.GetFontSize(Game.Instance.DefaultFontSize);
            var fontSizeRatio = Game.Instance.DefaultFont.GetGlyphRatio(fontSize);
            var frames = sadImage.ToSurface(TextureConvertMode.Foreground, _frame.Width * _frame.Count, _frame.Height / 2);

            // create frames from the raw data
            for (int i = 0; i < _frame.Count; i++)
            {
                var frame = CreateFrame();
                frames.Copy(_frameArea, frame, 0, 0);

                // fill the black cells with full alpha
                foreach (var cell in Frames[i])
                {
                    if (cell.Foreground.GetBrightness() < 1) cell.Background = cell.Background.FillAlpha();
                }

                // move the view to the next frame
                _frameArea = _frameArea.ChangeX(_frame.Width);
            }

            AnimationDuration = _frame.Count * _frame.Duration;
            Repeat = true;
        }
    }
}
