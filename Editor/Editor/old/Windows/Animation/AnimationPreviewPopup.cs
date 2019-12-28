using Microsoft.Xna.Framework;
using SadConsole.Surfaces;
using SadConsole.Controls;
using SadConsoleEditor.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SadConsole.Input;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Entities;
using SadConsole;

namespace SadConsoleEditor.Windows
{
    public class PreviewAnimationPopup : Window
    {
        private Entity entity;
        private AnimatedSurface animation;
        private Button restartAnimation;

        public PreviewAnimationPopup(AnimatedSurface animation) : base(animation.Width + 2, animation.Height + 4)
        {
            textSurface.Font = Settings.Config.ScreenFont;
            this.animation = animation;

            CloseOnESC = true;
            entity = new Entity(1, 1, Settings.Config.ScreenFont);
            entity.Position = new Point(1, 1);
            entity.Animation = animation;
            animation.Restart();
            entity.Animation.Start();
            entity.Position = new Point(1);
            Children.Add(entity);

            restartAnimation = new Button(animation.Width, 1);
            restartAnimation.Text = "Restart";
            restartAnimation.Position = new Point(1, TextSurface.Height - 2);
            restartAnimation.Click += (s, e) => this.animation.Restart();
            Add(restartAnimation);
        }

        public override void Redraw()
        {
            base.Redraw();

            // Draw bar
            for (int i = 1; i < TextSurface.Width - 1; i++)
            {
                SadConsole.Themes.Library.Default.WindowTheme.BorderStyle.CopyAppearanceTo(textSurface.GetCell(i, TextSurface.Height - 3));
                textSurface.GetCell(i, TextSurface.Height - 3).Glyph = 205;
            }

            SadConsole.Themes.Library.Default.WindowTheme.BorderStyle.CopyAppearanceTo(textSurface.GetCell(0, TextSurface.Height - 3));
            textSurface.GetCell(0, TextSurface.Height - 3).Glyph = 204;

            SadConsole.Themes.Library.Default.WindowTheme.BorderStyle.CopyAppearanceTo(textSurface.GetCell(TextSurface.Width - 1, TextSurface.Height - 3));
            textSurface.GetCell(TextSurface.Width - 1, TextSurface.Height - 3).Glyph = 185;
        }

        public override bool ProcessKeyboard(Keyboard info)
        {
            if (info.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Space))
                animation.Restart();

            return base.ProcessKeyboard(info);
        }

        public override bool ProcessMouse(MouseConsoleState info)
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
