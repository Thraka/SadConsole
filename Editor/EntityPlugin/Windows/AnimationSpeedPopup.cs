using SadConsole.UI.Controls;

namespace EntityPlugin.Windows
{
    class AnimationSpeedPopup : SadConsole.UI.Window
    {
        private Button okButton;
        private Button cancelButton;
        private TextBox textBox;

        public float NewSpeed { get { return float.Parse(textBox.Text); } }

        public AnimationSpeedPopup(float speed) : base(22, 6)
        {
            Title = "Animation Speed";

            okButton = new Button(8, 1);
            cancelButton = new Button(8, 1);
            textBox = new TextBox(Width - 4);
            textBox.IsNumeric = true;
            textBox.AllowDecimal = true;
            textBox.Text = speed.ToString();

            okButton.Position = new SadRogue.Primitives.Point(Width - okButton.Width - 2, Height - 2);
            cancelButton.Position = new SadRogue.Primitives.Point(2, Height - 2);
            textBox.Position = new SadRogue.Primitives.Point(2, 2);

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
