using SadConsole.Components;

namespace SadConsole.Examples;

partial class RootScreen
{
    public class LineCharacterFade : Instructions.InstructionBase
    {
        private Effects.ICellEffect _effect;

        private ColoredString[] _lines;
        private Instructions.DrawString[] _lineDrawers;
        private TimeSpan _totalTimeToPrint;

        public LineCharacterFade(TimeSpan totalTimeToPrint)
        {
            RemoveOnFinished = true;
            _totalTimeToPrint = totalTimeToPrint;

            _effect = new Effects.Fade()
            {
                FadeForeground = true,
                DestinationForeground = new Gradient(new[] { Color.Violet, Color.Black, Color.Gray, Color.Purple }, new[] { 0.0f, 0.01f, 0.5f, 1.0f }),
                UseCellDestinationReverse = true,
                UseCellForeground = true,
                CloneOnAdd = true,
                FadeDuration = TimeSpan.FromMilliseconds(300),
                RemoveOnFinished = true,
                RestoreCellOnRemoved = true,
            };
        }

        public override void Update(IScreenObject componentHost, TimeSpan delta)
        {
            base.Update(componentHost, delta);

            IsFinished = true;

            foreach (var instruction in _lineDrawers)
            {
                if (instruction != null)
                {
                    instruction.Update(componentHost, delta);

                    IsFinished &= instruction.IsFinished;
                }
            }
        }

        public override void OnAdded(IScreenObject host)
        {
            if (host is not IScreenSurface obj) throw new ArgumentException($"Instruction can only be added to {nameof(IScreenSurface)}");

            _lines = new ColoredString[obj.Surface.Height];
            _lineDrawers = new Instructions.DrawString[obj.Surface.Height];

            for (int i = 0; i < _lines.Length; i++)
            {
                int stringLength = obj.Surface.GetString(0, i, obj.Surface.Width).Trim('\0').Length;

                if (stringLength == 0) continue;

                ColoredString coloredString = obj.Surface.GetStringColored(0, i, stringLength);
                coloredString.SetEffect(_effect);
                coloredString.IgnoreEffect = false;
                _lineDrawers[i] = new Instructions.DrawString(coloredString);
                _lineDrawers[i].TotalTimeToPrint = _totalTimeToPrint;
                _lineDrawers[i].Position = (0, i);
                _lineDrawers[i].Cursor = new Cursor(obj.Surface)
                {
                    AutomaticallyShiftRowsUp = false
                };
            }

            obj.Surface.Clear();

            base.OnAdded(host);
        }

        public override void OnRemoved(IScreenObject host)
        {
            _lines = Array.Empty<ColoredString>();
            _lineDrawers = Array.Empty<Instructions.DrawString>();
        }
    }
}
