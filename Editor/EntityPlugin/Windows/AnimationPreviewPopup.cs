using SadRogue.Primitives;
using SadConsole;
using SadConsole.UI.Controls;
using SadConsole.Entities;
using SadConsole.Input;

namespace EntityPlugin.Windows
{
    public class PreviewAnimationPopup : SadConsole.UI.Window
    {
        private Entity entity;
        private AnimatedScreenSurface animation;
        private Button restartAnimation;

        public PreviewAnimationPopup(AnimatedScreenSurface animation) : base(animation.Width + 2, animation.Height + 4)
        {

            Font = SadConsoleEditor.Config.Program.ScreenFont;

            this.animation = animation;

            CloseOnEscKey = true;
            entity = new Entity(1, 1);
            
            entity.Position = new Point(1, 1);
            entity.Animation = animation;
            animation.Restart();
            entity.Animation.Start();
            entity.Position = new Point(1, 1);
            Children.Add(entity);

            restartAnimation = new Button(animation.Width, 1);
            restartAnimation.Text = "Restart";
            restartAnimation.Position = new Point(1, Height - 2);
            restartAnimation.Click += (s, e) => this.animation.Restart();

            Add(restartAnimation);
        }

        protected override void OnInvalidated()
        {
            base.OnInvalidated();

            var themeColors = GetThemeColors();

            var fillStyle = new ColoredGlyph(themeColors.ControlHostFore, themeColors.ControlHostBack);
            var borderStyle = new ColoredGlyph(themeColors.Lines, fillStyle.Background, 0);

            // Draw bar
            for (int i = 1; i < Width - 1; i++)
            {
                borderStyle.CopyAppearanceTo(this[i, Height - 3]);
                this[i, Height - 3].Glyph = 205;
            }

            borderStyle.CopyAppearanceTo(this[0, Height - 3]);
            this[0, Height - 3].Glyph = 204;

            borderStyle.CopyAppearanceTo(this[Width - 1, Height - 3]);
            this[Width - 1, Height - 3].Glyph = 185;
        }

        public override bool ProcessKeyboard(Keyboard info)
        {
            if (info.IsKeyReleased(Keys.Space))
                animation.Restart();

            return base.ProcessKeyboard(info);
        }

        public override bool ProcessMouse(MouseScreenObjectState info)
        {
            base.ProcessMouse(info);

            if (info.Mouse.RightClicked)
                Hide();

            return true;
        }

        public override void Show(bool modal)
        {
            Center();
            base.Show(modal);
        }
    }
}
