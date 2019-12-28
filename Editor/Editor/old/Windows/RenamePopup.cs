using SadConsole.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsoleEditor.Windows
{
    class RenamePopup : SadConsole.Window
    {
        private Button okButton;
        private Button cancelButton;
        private InputBox textBox;

        public string NewName { get { return textBox.Text; } }

        public RenamePopup(string text, string title = "Rename") : base(22, 7)
        {
            Title = title;

            okButton = new Button(8);
            cancelButton = new Button(8);
            textBox = new InputBox(textSurface.Width - 4);
            textBox.Text = text;

            okButton.Position = new Microsoft.Xna.Framework.Point(textSurface.Width - okButton.Width - 2, textSurface.Height - 3);
            cancelButton.Position = new Microsoft.Xna.Framework.Point(2, textSurface.Height - 3);
            textBox.Position = new Microsoft.Xna.Framework.Point(2, 2);

            okButton.Click += (o, e) => { DialogResult = true; Hide(); };
            cancelButton.Click += (o, e) => { DialogResult = false; Hide(); };

            okButton.Text = "Ok";
            cancelButton.Text = "Cancel";

            Add(okButton);
            Add(cancelButton);
            Add(textBox);

            FocusedControl = textBox;
        }
    }
}
