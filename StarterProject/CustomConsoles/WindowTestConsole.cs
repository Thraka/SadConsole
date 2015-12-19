namespace StarterProject.CustomConsoles
{
    using System;
    using SadConsole;
    using SadConsole.Consoles;
    using Console = SadConsole.Consoles.Console;
    using Microsoft.Xna.Framework;

    class WindowTestConsole : ControlsConsole
    {
        private Window _window;

        public WindowTestConsole()
            : base(80, 25)
        {
            this.IsVisible = false;

            this.VirtualCursor.Position = new Point(3, 1);
            this.VirtualCursor.Print("This console can configure a window for popup. It also demonstrates some \r\n   basic UI controls.");

            // This is set becuase when the window pops up, this console loses focus and it shifts focus to the window.
            // Normally consoles do not receive focus when you click on them with the mouse, so you cannot interact
            // with this console after the window has been shown. This allos the console to get focus again if you click
            // behind the window.
            this.CanFocus = true;
            this.MouseCanFocus = true;

            // Configure our window button
            var button = new SadConsole.Controls.Button(8, 1)
            {
                Text = "Show",
                TextAlignment = System.Windows.HorizontalAlignment.Center,
                Position = new Point(3, 6),
            };
            button.ButtonClicked += button_ButtonClicked;

            // After it has been configured, add it to this controls console so it is rendered and can be interacted with.
            this.Add(button);

            // Create the window. It will not be shown after being created. We will use the button to do that.
            CreateWindow();
        }

        private void CreateWindow()
        {
            _window = new SadConsole.Consoles.Window(30, 10);
            _window.Title = " Popup Window 1 ";

            var closeButton = new SadConsole.Controls.Button(7, 1);
            closeButton.Text = "Close";
            closeButton.Position = new Point(_window.CellData.Width - 2 - closeButton.Width, _window.CellData.Height - 2);
            closeButton.ButtonClicked += (sender, e) => { _window.Hide(); };

            _window.Add(closeButton);
            _window.Center();
        }

        void button_ButtonClicked(object sender, EventArgs e)
        {
            _window.Show();
        }

    }
}
