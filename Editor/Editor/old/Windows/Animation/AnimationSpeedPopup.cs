using SadConsole.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsoleEditor.Windows
{
    class AnimationSpeedPopup : SadConsole.Window
    {
        private Button okButton;
        private Button cancelButton;
        private InputBox textBox;

        public float NewSpeed { get { return float.Parse(textBox.Text); } }

        public AnimationSpeedPopup(float speed) : base(22, 6)
        {
            Title = "Animation Speed";

            okButton = new Button(8, 1);
            cancelButton = new Button(8, 1);
            textBox = new InputBox(TextSurface.Width - 4);
            textBox.IsNumeric = true;
            textBox.AllowDecimal = true;
            textBox.Text = speed.ToString();

            okButton.Position = new Microsoft.Xna.Framework.Point(TextSurface.Width - okButton.Width - 2, TextSurface.Height - 2);
            cancelButton.Position = new Microsoft.Xna.Framework.Point(2, TextSurface.Height - 2);
            textBox.Position = new Microsoft.Xna.Framework.Point(2, 2);

            okButton.Click += (o, e) => { DialogResult = true; Hide(); };
            cancelButton.Click += (o, e) => { DialogResult = false; Hide(); };

            okButton.Text = "Ok";
            cancelButton.Text = "Cancel";

            Add(okButton);
            Add(cancelButton);
            Add(textBox);
        }
    }
}
