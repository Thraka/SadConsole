using SadConsole.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Consoles
{
    public partial class Window : ControlsConsole
    {
        public static void Prompt(ColoredString message, string yesPrompt, string noPrompt, Action<bool> resultCallback)
        {
            Window window = new Window(message.ToString().Length + 4, 6);

            message.IgnoreBackground = true;

            window._cellData.Print(2, 2, message);

            Button yesButton = new Button(yesPrompt.Length + 2, 1);
            Button noButton = new Button(noPrompt.Length + 2, 1);

            yesButton.Position = new Microsoft.Xna.Framework.Point(2, window._cellData.Height - 2);
            noButton.Position = new Microsoft.Xna.Framework.Point(window._cellData.Width - noButton.Width - 2, window._cellData.Height - 2);

            yesButton.Text = yesPrompt;
            noButton.Text = noPrompt;

            yesButton.ButtonClicked += (o, e) => { window.DialogResult = true; window.Hide(); };
            noButton.ButtonClicked += (o, e) => { window.DialogResult = false; window.Hide(); };

            window.Add(yesButton);
            window.Add(noButton);

            window.Closed += (o, e) =>
                {
                    resultCallback(window.DialogResult);
                };

            window.Show(true);
            window.Center();
        }

        public static void Message(ColoredString message, string closeButtonText)
        {
            Window window = new Window(message.ToString().Length + 4, 6);

            message.IgnoreBackground = true;

            window._cellData.Print(2, 2, message);

            Button closeButton = new Button(closeButtonText.Length + 2, 1);

            closeButton.Position = new Microsoft.Xna.Framework.Point(2, window._cellData.Height - 2);

            closeButton.Text = closeButtonText;

            closeButton.ButtonClicked += (o, e) => { window.DialogResult = true; window.Hide(); };

            window.Add(closeButton);

            window.Show(true);
            window.Center();
        }
    }
}
