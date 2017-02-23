using Microsoft.Xna.Framework;

using SadConsole.Controls;
using System;


namespace SadConsole
{
    public partial class Window : ControlsConsole
    {
        /// <summary>
        /// Shows a window prompt with two buttons for the user to click.
        /// </summary>
        /// <param name="message">The text to display.</param>
        /// <param name="yesPrompt">The yes button's text.</param>
        /// <param name="noPrompt">The no button's text.</param>
        /// <param name="resultCallback">Callback with the yes (true) or no (false) result.</param>
        public static void Prompt(string message, string yesPrompt, string noPrompt, Action<bool> resultCallback)
        {
            Prompt(new ColoredString(message), yesPrompt, noPrompt, resultCallback);
        }

        /// <summary>
        /// Shows a window prompt with two buttons for the user to click.
        /// </summary>
        /// <param name="message">The text to display. (background color is ignored)</param>
        /// <param name="yesPrompt">The yes button's text.</param>
        /// <param name="noPrompt">The no button's text.</param>
        /// <param name="resultCallback">Callback with the yes (true) or no (false) result.</param>
        public static void Prompt(ColoredString message, string yesPrompt, string noPrompt, Action<bool> resultCallback)
        {
            Window window = new Window(message.ToString().Length + 4, 6);

            message.IgnoreBackground = true;

            window.Print(2, 2, message);

            Button yesButton = new Button(yesPrompt.Length + 2);
            Button noButton = new Button(noPrompt.Length + 2);

            yesButton.Position = new Point(2, window.textSurface.Height - 2);
            noButton.Position = new Point(window.textSurface.Width - noButton.Width - 2, window.textSurface.Height - 2);

            yesButton.Text = yesPrompt;
            noButton.Text = noPrompt;

            yesButton.Click += (o, e) => { window.DialogResult = true; window.Hide(); };
            noButton.Click += (o, e) => { window.DialogResult = false; window.Hide(); };

            window.Add(yesButton);
            window.Add(noButton);

            window.Closed += (o, e) =>
                {
                    resultCallback(window.DialogResult);
                };

            window.Show(true);
            window.Center();
        }

        /// <summary>
        /// Displays a dialog to the user with a specific message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="closeButtonText">The text of the dialog's close button.</param>
        /// <param name="closedCallback">A callback indicating the message was dismissed.</param>
        public static void Message(string message, string closeButtonText, Action closedCallback = null)
        {
            Message(new ColoredString(message), closeButtonText, closedCallback);
        }

        /// <summary>
        /// Displays a dialog to the user with a specific message.
        /// </summary>
        /// <param name="message">The message. (background color is ignored)</param>
        /// <param name="closeButtonText">The text of the dialog's close button.</param>
        /// <param name="closedCallback">A callback indicating the message was dismissed.</param>
        public static void Message(ColoredString message, string closeButtonText, Action closedCallback = null)
        {
            Window window = new Window(message.ToString().Length + 4, 6);

            message.IgnoreBackground = true;

            window.Print(2, 2, message);

            Button closeButton = new Button(closeButtonText.Length + 2);

            closeButton.Position = new Point(2, window.textSurface.Height - 2);

            closeButton.Text = closeButtonText;

            closeButton.Click += (o, e) => { window.DialogResult = true; window.Hide(); closedCallback?.Invoke(); };

            window.Add(closeButton);
            window.CloseOnESC = true;
            window.Show(true);
            window.Center();
        }
    }
}
