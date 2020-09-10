using System;
using SadConsole;
using SadRogue.Primitives;

namespace FeatureDemo.CustomConsoles
{
    class FadingChild: ScreenSurface
    {
        private ScreenSurface _child;

        private SadConsole.Instructions.InstructionSet _set;

        public FadingChild(): base(80,23)
        {
            Surface.FillWithRandomGarbage(Font);

            _child = new ScreenSurface(30, 13);
            _child.Surface.DefaultForeground = Color.White;
            _child.Surface.DefaultBackground = Color.Black;
            _child.Surface.Clear();
            _child.Surface.DrawBox(_child.Surface.View, new ColoredGlyph(Color.MediumPurple, Color.Black), connectedLineStyle: ICellSurface.ConnectedLineThick);
            _child.Surface.Print(2, 2, "Second console");
            _child.Position = (25, 4);

            Children.Add(_child);

            _set = new SadConsole.Instructions.InstructionSet()

                .Instruct(new SadConsole.Instructions.AnimatedValue(TimeSpan.FromSeconds(1.5), 255, 0, new SadConsole.EasingFunctions.Linear()))
                .Instruct(new SadConsole.Instructions.AnimatedValue(TimeSpan.FromSeconds(1.5), 0, 255, new SadConsole.EasingFunctions.Linear()))
                ;

            _set.RepeatCount = -1;

            SadComponents.Add(_set);
            //_child.Tint = new Color(0, 127, 0, 127);
        }

        public override void Update(TimeSpan delta)
        {
            base.Update(delta);
            
            if (_set.CurrentInstruction is SadConsole.Instructions.AnimatedValue current)
                ((SadConsole.Renderers.ScreenSurfaceRenderer)_child.Renderer).Opacity = (byte)current.Value;
        }
    }
}
