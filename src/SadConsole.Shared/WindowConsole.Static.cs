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
            message.IgnoreBackground = true;

            Themes.Library.Default.ButtonTheme = new Themes.ButtonLinesTheme();

            Button yesButton = new Button(yesPrompt.Length + 2, 1);
            Button noButton = new Button(noPrompt.Length + 2, 1);

            Window window = new Window(message.ToString().Length + 4, 5 + yesButton.Surface.Height);

            window.Print(2, 2, message);
            
            yesButton.Position = new Point(2, window.Height - 1 - yesButton.Surface.Height);
            noButton.Position = new Point(window.Width - noButton.Width - 2, window.Height - 1 - yesButton.Surface.Height);

            yesButton.Text = yesPrompt;
            noButton.Text = noPrompt;

            yesButton.Click += (o, e) => { window.DialogResult = true; window.Hide(); };
            noButton.Click += (o, e) => { window.DialogResult = false; window.Hide(); };

            window.Add(yesButton);
            window.Add(noButton);

            window.Closed += (o, e) =>
                {
                    resultCallback?.Invoke(window.DialogResult);
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
            var width = message.ToString().Length + 4;
            var buttonWidth = closeButtonText.Length + 2;

            if (buttonWidth < 9)
                buttonWidth = 9;

            if (width < buttonWidth + 4)
                width = buttonWidth + 4;

            Button closeButton = new Button(buttonWidth, 1)
            {
                Text = closeButtonText
            };


            Window window = new Window(width, 5 + closeButton.Surface.Height);
            
            message.IgnoreBackground = true;

            window.Print(2, 2, message);
            
            closeButton.Position = new Point(2, window.Height - 1 - closeButton.Surface.Height);
            closeButton.Click += (o, e) => { window.DialogResult = true; window.Hide(); closedCallback?.Invoke(); };

            window.Add(closeButton);
            window.CloseOnESC = true;
            window.Show(true);
            window.Center();
        }
    }
}
