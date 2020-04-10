#if XNA
using Microsoft.Xna.Framework;
#endif

namespace SadConsole
{
    using System;
    using SadConsole.Controls;

    public partial class Window
    {
        /// <summary>
        /// Shows a window prompt with two buttons for the user to click.
        /// </summary>
        /// <param name="message">The text to display.</param>
        /// <param name="yesPrompt">The yes button's text.</param>
        /// <param name="noPrompt">The no button's text.</param>
        /// <param name="resultCallback">Callback with the yes (true) or no (false) result.</param>
        /// <param name="library">The library to theme the message box. If <see langword="null"/>, then the theme will be set to <see cref="Themes.Library.Default"/>.</param>
        public static void Prompt(string message, string yesPrompt, string noPrompt, Action<bool> resultCallback, Themes.Library library = null) => Prompt(new ColoredString(message), yesPrompt, noPrompt, resultCallback, library);

        /// <summary>
        /// Shows a window prompt with two buttons for the user to click.
        /// </summary>
        /// <param name="message">The text to display. (background color is ignored)</param>
        /// <param name="yesPrompt">The yes button's text.</param>
        /// <param name="noPrompt">The no button's text.</param>
        /// <param name="resultCallback">Callback with the yes (true) or no (false) result.</param>
        /// <param name="library">The library to theme the message box. If <see langword="null"/>, then the theme will be set to <see cref="Themes.Library.Default"/>.</param>
        public static void Prompt(ColoredString message, string yesPrompt, string noPrompt, Action<bool> resultCallback, Themes.Library library = null)
        {
            message.IgnoreBackground = true;

            if (library == null)
            {
                library = Themes.Library.Default;
            }

            var yesButton = new Button(yesPrompt.Length + 2, 1);
            var noButton = new Button(noPrompt.Length + 2, 1);

            yesButton.Theme = library.GetControlTheme(typeof(Button));
            noButton.Theme = library.GetControlTheme(typeof(Button));

            var window = new Window(message.ToString().Length + 4, 5 + yesButton.Surface.Height)
            {
                Theme = library.WindowTheme
            };

            var printArea = new DrawingSurface(window.Width, window.Height)
            {
                OnDraw = (ds) =>
                {
                    Cell appearance = ((Themes.DrawingSurfaceTheme)ds.Theme).Appearance;
                    ds.Surface.Fill(appearance.Foreground, appearance.Background, null);
                    ds.Surface.Print(2, 2, message);
                }
            };

            yesButton.Position = new Point(2, window.Height - 1 - yesButton.Surface.Height);
            noButton.Position = new Point(window.Width - noButton.Width - 2, window.Height - 1 - yesButton.Surface.Height);

            yesButton.Text = yesPrompt;
            noButton.Text = noPrompt;

            yesButton.Click += (o, e) => { window.DialogResult = true; window.Hide(); };
            noButton.Click += (o, e) => { window.DialogResult = false; window.Hide(); };

            yesButton.Theme = null;
            noButton.Theme = null;

            window.Add(printArea);
            window.Add(yesButton);
            window.Add(noButton);

            noButton.IsFocused = true;

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
        /// <param name="library">The library to theme the message box. If <see langword="null"/>, then the theme will be set to <see cref="Themes.Library.Default"/>.</param>
        public static void Message(string message, string closeButtonText, Action closedCallback = null, Themes.Library library = null) => Message(new ColoredString(message), closeButtonText, closedCallback, library);

        /// <summary>
        /// Displays a dialog to the user with a specific message.
        /// </summary>
        /// <param name="message">The message. (background color is ignored)</param>
        /// <param name="closeButtonText">The text of the dialog's close button.</param>
        /// <param name="closedCallback">A callback indicating the message was dismissed.</param>
        /// <param name="library">The library to theme the message box. If <see langword="null"/>, then the theme will be set to <see cref="Themes.Library.Default"/>.</param>
        public static void Message(ColoredString message, string closeButtonText, Action closedCallback = null, Themes.Library library = null)
        {
            int width = message.ToString().Length + 4;
            int buttonWidth = closeButtonText.Length + 2;

            if (library == null)
            {
                library = Themes.Library.Default;
            }

            if (buttonWidth < 9)
            {
                buttonWidth = 9;
            }

            if (width < buttonWidth + 4)
            {
                width = buttonWidth + 4;
            }

            var closeButton = new Button(buttonWidth, 1)
            {
                Text = closeButtonText,
                Theme = library.GetControlTheme(typeof(Button))
            };

            var window = new Window(width, 5 + closeButton.Surface.Height)
            {
                Theme = library.WindowTheme
            };

            message.IgnoreBackground = true;

            var printArea = new DrawingSurface(window.Width, window.Height)
            {
                OnDraw = (ds) =>
                {
                    Cell appearance = ((Themes.DrawingSurfaceTheme)ds.Theme).Appearance;
                    ds.Surface.Fill(appearance.Foreground, appearance.Background, null);
                    ds.Surface.Print(2, 2, message);
                }
            };
            window.Add(printArea);

            closeButton.Position = new Point(2, window.Height - 1 - closeButton.Surface.Height);
            closeButton.Click += (o, e) => { window.DialogResult = true; window.Hide(); closedCallback?.Invoke(); };
            closeButton.Theme = null;

            window.Add(closeButton);
            closeButton.IsFocused = true;
            window.CloseOnEscKey = true;
            window.Show(true);
            window.Center();
        }
    }
}
